#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CMD
{
    public sealed class CatalogMasterGenerator : AssetPostprocessor
    {
        private const string ROOT = "CMDCatalog"; // <— КОРЕНЬ ресурсов
        private const string _configResPath = "CMDCatalog/CatalogMasterConfig";
        private const string _keysNamespace = "CatalogKeys";
        private static bool _isRebuilding;

        [InitializeOnLoadMethod]
        private static void InitOnce()
        {
            EditorApplication.delayCall += () => RebuildAll(force: false);
        }

        [MenuItem("Tools/Catalog/Rebuild All Keys & Registrars")]
        private static void MenuRebuild() => RebuildAll(force: true);

        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            if (_isRebuilding) return;

            // если модифицировали сам конфиг — делаем полную пересборку и выходим
            if (ConfigTouched(imported) || ConfigTouched(deleted) || ConfigTouched(moved) || ConfigTouched(movedFrom))
            {
                RebuildAll(force: true);
                return;
            }

            var cfg = LoadConfig();
            if (cfg == null) return;

            if (!AnyTouched(cfg, imported) &&
                !AnyTouched(cfg, deleted) &&
                !AnyTouched(cfg, moved) &&
                !AnyTouched(cfg, movedFrom))
                return;

            RebuildAll(force: false);
        }

        // ───────────── core ─────────────

        private static void RebuildAll(bool force)
        {
            var cfg = LoadConfig();
            if (cfg == null) return;

            if (_isRebuilding) return;
            _isRebuilding = true;
            try
            {
                int totalChanged = 0, totalKeys = 0;
                foreach (var e in cfg.entries.Where(e => !string.IsNullOrWhiteSpace(e.resourcesPath)
                             && !string.IsNullOrWhiteSpace(e.typeName)))
                {
                    var changed = RebuildOne(e, out int keysCount);
                    if (changed) totalChanged++;
                    totalKeys += keysCount;
                }
                if (totalChanged > 0)
                    Debug.Log($"[Catalog] Regenerated: {totalChanged} folder(s), {totalKeys} keys total.");
            }
            finally { _isRebuilding = false; }
        }

        private static bool RebuildOne(CatalogMasterConfig.Entry e, out int keysCount)
        {
            keysCount = 0;

            var resPath = $"{ROOT}/{e.resourcesPath}".Replace('\\', '/');

            var absDir = AbsResourcesDir(resPath);
            var genDir = absDir + "/__Generated";
            Directory.CreateDirectory(genDir);

            var search = e.recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var names = Directory.Exists(absDir)
                ? Directory.GetFiles(absDir, "*", search)
                    .Select(p => p.Replace('\\', '/'))
                    .Where(p => !p.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)
                        && !p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                        && !p.Contains("/__Generated/"))
                    .Select(Path.GetFileNameWithoutExtension)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToList()
                : new List<string>();

            keysCount = names.Count;

            // категория — первый сегмент после CMDCatalog/
            var category = e.resourcesPath.Split('/', '\\').FirstOrDefault() ?? "Root";
            var safeCategory = Sanitize(category);

            var keysPath = $"{genDir}/Keys_{safeCategory}.g.cs";
            var regPath = $"{genDir}/__AutoRegistrar_{safeCategory}.g.cs";

            bool changed = false;

            const bool STRONG_KEYS = true;

            var keysCode = BuildKeysCodeNested(
                rootClass: ROOT,
                categoryClass: safeCategory,
                typeCs: MakeGlobalType(e.typeName),
                names: names,
                strongKeys: STRONG_KEYS
            );
            changed |= WriteIfChanged(keysPath, keysCode);

            var regCode = BuildRegistrarCode($"__AutoRegistrar_{safeCategory}", e.typeName, resPath);
            changed |= WriteIfChanged(regPath, regCode);

            if (changed)
            {
                AssetDatabase.StartAssetEditing();
                try
                {
                    if (File.Exists(keysPath)) AssetDatabase.ImportAsset(RelAssets(keysPath), ImportAssetOptions.ForceUpdate);
                    if (File.Exists(regPath)) AssetDatabase.ImportAsset(RelAssets(regPath), ImportAssetOptions.ForceUpdate);
                }
                finally { AssetDatabase.StopAssetEditing(); }
            }

            return changed;
        }

        // ───────────── helpers ─────────────

        private static string MakeGlobalType(string typeName)
        {
            typeName = (typeName ?? "").Trim();
            return typeName.Contains(".") ? $"global::{typeName}" : typeName;
        }

        private static CatalogMasterConfig LoadConfig()
        {
            var cfg = Resources.Load<CatalogMasterConfig>(_configResPath);

            if (!cfg)
            {
                Debug.LogError(
                    "[Catalog] CatalogMasterConfig not found.\n" +
                    $"Create one via menu: Tools/Catalog/Create Default Config (Resources)\n" +
                    $"Config path: Assets/Resources/CMDCatalog/{_configResPath}.asset\n");
                return null;
            }
            return cfg;
        }

        private static bool ConfigTouched(string[] paths)
        {
            if (paths == null || paths.Length == 0) return false;

            // твой путь к ресурсу → путь ассета
            var assetPath = $"Assets/Resources/{_configResPath}.asset".Replace('\\', '/');

            foreach (var p in paths)
            {
                var n = p.Replace('\\', '/');
                if (string.Equals(n, assetPath, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static bool AnyTouched(CatalogMasterConfig cfg, string[] paths)
        {
            if (paths == null || paths.Length == 0) return false;

            foreach (var e in cfg.entries)
            {
                if (string.IsNullOrWhiteSpace(e.resourcesPath)) continue;
                var dir = AbsResourcesDir($"{ROOT}/{e.resourcesPath}");
                var gen = dir + "/__Generated";

                foreach (var p in paths)
                {
                    var abs = AbsFromMaybeAssets(p);
                    if (!abs.StartsWith(dir, StringComparison.OrdinalIgnoreCase)) continue;
                    if (abs.StartsWith(gen, StringComparison.OrdinalIgnoreCase)) continue;
                    if (abs.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)) continue;
                    return true;
                }
            }
            return false;
        }

        private static string BuildKeysCodeNested(string rootClass, string categoryClass, string typeCs, IEnumerable<string> names, bool strongKeys)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// AUTO-GENERATED. DO NOT EDIT");
            sb.AppendLine("#pragma warning disable");
            if (strongKeys) sb.AppendLine("using CMD.Services;");
            sb.AppendLine("namespace CatalogKeys {");
            sb.AppendLine($"  public static partial class {rootClass} {{");
            sb.AppendLine($"    public static class {categoryClass} {{");

            foreach (var n in names)
            {
                var id = Sanitize(n);
                if (strongKeys)
                    sb.AppendLine($"      public static readonly CatalogKey<{typeCs}> {id} = new(\"{n}\");");
                else
                    sb.AppendLine($"      public const string {id} = \"{n}\";");
            }

            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string BuildRegistrarCode(string className, string typeName, string resourcesPath)
        {
            // Генерим типизированный регистратор (без рефлексии в generic-уголках) + Preserve для IL2CPP.
            // typeName — строка, как вы её ввели в Config (например, "UnityEngine.AudioClip" или "BigHeart.ItemDefinition")
            // Если у типа есть namespace, пропишите его полностью.
            var typeCs = typeName.Contains(".") ? $"global::{typeName}" : typeName;
            return
                $@"// AUTO-GENERATED. DO NOT EDIT
#pragma warning disable
using CMD.Services;
using UnityEngine.Scripting;

[Preserve]
public sealed class {className} : ICatalogRegistrar
{{
    public void RegisterInto(ICatalog c)
    {{
        var folder = new ResourcesFolderCatalog<{typeCs}>(""{resourcesPath}"");
        c.RegisterFolder(folder);
    }}
}}
";
        }

        private static bool WriteIfChanged(string path, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (File.Exists(path))
            {
                var prev = File.ReadAllText(path);
                if (prev == content) return false;
            }
            File.WriteAllText(path, content, Encoding.UTF8);
            return true;
        }

        private static string Sanitize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "_";
            var sb = new StringBuilder();
            var c0 = s[0];
            sb.Append(char.IsLetter(c0) || c0 == '_' ? c0 : '_');
            for (int i = 1; i < s.Length; i++)
            {
                var c = s[i];
                sb.Append(char.IsLetterOrDigit(c) ? c : '_');
            }
            return sb.ToString();
        }

        private static string AbsResourcesDir(string resPath)
        {
            var data = Application.dataPath.Replace('\\', '/');
            return $"{data}/Resources/{resPath}".Replace('\\', '/');
        }

        private static string AbsFromMaybeAssets(string maybe)
        {
            var p = maybe.Replace('\\', '/');
            var data = Application.dataPath.Replace('\\', '/');
            if (p.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                return $"{data}/{p.Substring("Assets/".Length)}".Replace('\\', '/');
            return Path.IsPathRooted(p) ? p : $"{data}/{p}";
        }

        private static string RelAssets(string abs)
        {
            var data = Application.dataPath.Replace('\\', '/');
            var a = abs.Replace('\\', '/');
            return a.StartsWith(data, StringComparison.OrdinalIgnoreCase)
                ? "Assets/" + a[(data.Length + 1)..]
                : abs;
        }
    }
}
#endif

// #if UNITY_EDITOR
// using System.Collections.Generic;
// using System.IO;
// using System.Text.RegularExpressions;
// using Entities.Item.Effect.Base;
// using Entities.Item.Factory;
// using Entities.Item.Model;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEditorInternal;
// using UnityEngine;
// using UnityEngine.SceneManagement;
//
// namespace Entities.Item.Editor
// {
//     /// <summary>
//     /// Rev‑8: sprite thumbnails show the correct sub‑sprite even if packed into an atlas.
//     /// Includes Item Library + Save As… + shared settings from ItemToolsSettings.
//     /// </summary>
//     public class ItemDesignerWindow : EditorWindow
//     {
//         [SerializeField] private ItemDefinition _draft;
//         private SerializedObject _so;
//         private ReorderableList _effectsList;
//         private Vector2 _scroll;
//         private GameObject _genericPrefab;
//
//         // Library state
//         private readonly List<ItemDefinition> _library = new();
//         private string _itemSearch = string.Empty;
//         private Vector2 _libScroll;
//
//         [MenuItem("Tools/Item Designer %#i", priority = 100)] // Ctrl+Shift+I
//         private static void Open()
//         {
//             var wnd = GetWindow<ItemDesignerWindow>();
//             wnd.titleContent = new GUIContent("Item Designer");
//             wnd.minSize = new Vector2(560, 640);
//         }
//
//         private void OnEnable()
//         {
//             _genericPrefab = Resources.Load<GameObject>(ItemToolsSettings.S.genericPrefabPath);
//             RefreshLibrary();
//         }
//
//         private void OnFocus() => RefreshLibrary();
//
//         private void OnGUI()
//         {
//             DrawTopBar();
//             _scroll = EditorGUILayout.BeginScrollView(_scroll);
//
//             DrawLibraryBlock();
//             GUILayout.Space(8);
//             DrawDefinitionBlock();
//             GUILayout.Space(10);
//             DrawToolbar();
//
//             EditorGUILayout.EndScrollView();
//         }
//
//         /*──────────── Top bar ────────────*/
//         private void DrawTopBar()
//         {
//             using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
//             {
//                 if (GUILayout.Button("New Item", EditorStyles.toolbarButton, GUILayout.Width(90)))
//                     NewDraft();
//
//                 GUILayout.FlexibleSpace();
//                 if (GUILayout.Button("Settings", EditorStyles.toolbarButton, GUILayout.Width(70)))
//                     SettingsService.OpenProjectSettings("Project/Big-Heart/Item Tools");
//             }
//         }
//
//         private void NewDraft()
//         {
//             _draft = CreateInstance<ItemDefinition>();
//             _draft.name = "New Item (Draft)";
//             _draft.hideFlags = HideFlags.DontSave;
//             _so = new SerializedObject(_draft);
//             SetupEffectsList();
//         }
//
//         /*──────────── Item Library block ────────────*/
//         private void DrawLibraryBlock()
//         {
//             EditorGUILayout.LabelField("Item Library", EditorStyles.boldLabel);
//             using (new EditorGUILayout.VerticalScope("box"))
//             {
//                 using (new EditorGUILayout.HorizontalScope())
//                 {
//                     _itemSearch = EditorGUILayout.TextField(new GUIContent("Search"), _itemSearch);
//                     if (GUILayout.Button("Refresh", GUILayout.Width(80))) RefreshLibrary();
//                 }
//
//                 using (var sv = new EditorGUILayout.ScrollViewScope(_libScroll, GUILayout.MaxHeight(220)))
//                 {
//                     _libScroll = sv.scrollPosition;
//                     int shown = 0;
//                     foreach (var def in _library)
//                     {
//                         if (def == null) continue;
//                         if (!PassesSearch(def, _itemSearch)) continue;
//                         shown++;
//                         using (new EditorGUILayout.HorizontalScope())
//                         {
//                             // sprite thumbnail (handles atlases)
//                             var rect = GUILayoutUtility.GetRect(32, 32, GUILayout.Width(32), GUILayout.Height(32));
//                             DrawSpriteIcon(rect, def.icon);
//
//                             using (new EditorGUILayout.VerticalScope())
//                             {
//                                 EditorGUILayout.LabelField(string.IsNullOrWhiteSpace(def.displayName) ? def.name : def.displayName, EditorStyles.label);
//                                 EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(def), EditorStyles.miniLabel);
//                             }
//
//                             GUILayout.FlexibleSpace();
//
//                             if (GUILayout.Button("Spawn", GUILayout.Width(70)))
//                                 SpawnDefinition(def);
//                             if (GUILayout.Button("Load", GUILayout.Width(60)))
//                                 LoadAsDraft(def);
//                             if (GUILayout.Button("Ping", GUILayout.Width(50)))
//                                 EditorGUIUtility.PingObject(def);
//                         }
//                     }
//
//                     if (shown == 0)
//                     {
//                         EditorGUILayout.HelpBox(string.IsNullOrEmpty(_itemSearch)
//                             ? "No ItemDefinition assets found. Create one or click Refresh."
//                             : "No items match the search.", MessageType.Info);
//                     }
//                 }
//             }
//         }
//
//         private static bool PassesSearch(ItemDefinition def, string q)
//         {
//             if (string.IsNullOrWhiteSpace(q)) return true;
//             q = q.Trim();
//             var name = def.name ?? string.Empty;
//             var disp = def.displayName ?? string.Empty;
//             return name.IndexOf(q, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
//                    disp.IndexOf(q, System.StringComparison.OrdinalIgnoreCase) >= 0;
//         }
//
//         private void RefreshLibrary()
//         {
//             _library.Clear();
//             foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition"))
//             {
//                 var path = AssetDatabase.GUIDToAssetPath(guid);
//                 var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
//                 if (def != null) _library.Add(def);
//             }
//             _library.Sort((a,b) => string.Compare(a.displayName, b.displayName, true));
//         }
//
//         private void LoadAsDraft(ItemDefinition source)
//         {
//             _draft = Instantiate(source);
//             _draft.name = source.name + " (Draft)";
//             _draft.hideFlags = HideFlags.DontSave;
//             _so = new SerializedObject(_draft);
//             SetupEffectsList();
//         }
//
//         /*──────────── Item Definition block ────────────*/
//         private void DrawDefinitionBlock()
//         {
//             EditorGUILayout.LabelField("Item Definition", EditorStyles.boldLabel);
//             using (new EditorGUILayout.VerticalScope("box"))
//             {
//                 if (_draft == null)
//                 {
//                     EditorGUILayout.HelpBox("No draft loaded. Click 'New Item' at the top or use 'Load' from the library.", MessageType.Info);
//                     return;
//                 }
//                 }
//
//                 _so.Update();
//                 DrawProperty("displayName", "Display Name *");
//                 DrawProperty("description");
//                 DrawProperty("icon");
//                 DrawProperty("itemTag", "Item Tag");
//                 DrawProperty("slotType", "Slot Type");
//                 DrawEffectsReorderable();
//                 DrawProperty("overridePrefab");
//                 _so.ApplyModifiedProperties();
//             }
//         }
//
//         private void DrawProperty(string prop, string label = null)
//         {
//             var sp = _so.FindProperty(prop);
//             if (sp != null)
//                 EditorGUILayout.PropertyField(sp, new GUIContent(label ?? ObjectNames.NicifyVariableName(prop)), true);
//         }
//
//         /*──────────── Effects list ────────────*/
//         private void SetupEffectsList()
//         {
//             var listProp = _so.FindProperty("effects");
//             _effectsList = new ReorderableList(_so, listProp, true, true, true, true);
//
//             _effectsList.drawHeaderCallback = rect => GUI.Label(rect, "Effects");
//             _effectsList.drawElementCallback = (rect, index, _, _) =>
//             {
//                 var element = listProp.GetArrayElementAtIndex(index);
//                 EditorGUI.PropertyField(rect, element, GUIContent.none);
//             };
//             _effectsList.onAddDropdownCallback = (_, __) => ShowEffectDropdown(listProp);
//         }
//
//         private void ShowEffectDropdown(SerializedProperty listProp)
//         {
//             var found = FindAllEffectAssets();
//             var menu = new GenericMenu();
//             int count = 0;
//             foreach (var ea in found)
//             {
//                 count++;
//                 string label = $"{ea.GetType().Name}/{ea.name}";
//                 menu.AddItem(new GUIContent(label), false, obj =>
//                 {
//                     listProp.arraySize++;
//                     var el = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
//                     el.objectReferenceValue = obj as BaseEffectAsset;
//                     _so.ApplyModifiedProperties();
//                 }, ea);
//             }
//
//             if (count == 0)
//             {
//                 EditorUtility.DisplayDialog(
//                     "No Effects Found",
//                     $"No assets matched filter '{ItemToolsSettings.S.effectsSearchFilter}'.\nCreate an effect via Create → Effects.",
//                     "OK");
//             }
//             else menu.ShowAsContext();
//         }
//
//         private static IEnumerable<BaseEffectAsset> FindAllEffectAssets()
//         {
//             var guids = AssetDatabase.FindAssets(ItemToolsSettings.S.effectsSearchFilter);
//             foreach (var g in guids)
//             {
//                 var path = AssetDatabase.GUIDToAssetPath(g);
//                 var ea = AssetDatabase.LoadAssetAtPath<BaseEffectAsset>(path);
//                 if (ea != null) yield return ea;
//             }
//         }
//
//         private void DrawEffectsReorderable()
//         {
//             _effectsList ??= SetupAndReturn();
//             _effectsList.DoLayoutList();
//
//             ReorderableList SetupAndReturn()
//             {
//                 SetupEffectsList();
//                 return _effectsList;
//             }
//         }
//
//         /*──────────── Toolbar ────────────*/
//         private void DrawToolbar()
//         {
//             if (_draft == null) return;
//             using (new EditorGUILayout.HorizontalScope())
//             {
//                 GUI.enabled = !Application.isPlaying; // disable in Play Mode
//
//                 if (GUILayout.Button("Spawn", GUILayout.Height(28)))
//                     EditorApplication.delayCall += Spawn;
//
//                 if (GUILayout.Button("Save Asset", GUILayout.Height(28)))
//                     EditorApplication.delayCall += SaveAsset;
//
//                 if (GUILayout.Button("Save As…", GUILayout.Height(28)))
//                     EditorApplication.delayCall += SaveAssetAs;
//
//                 if (GUILayout.Button("Duplicate", GUILayout.Height(28)))
//                     DuplicateDraft();
//
//                 GUI.enabled = true;
//             }
//
//             if (Application.isPlaying)
//                 EditorGUILayout.HelpBox("Play Mode: window is read‑only", MessageType.Info);
//         }
//
//         /*──────────── Actions ────────────*/
//         private void Spawn()
//         {
//             if (!Validate(_draft, out var msg))
//             {
//                 EditorUtility.DisplayDialog("Item Designer – cannot spawn", msg, "OK");
//                 return;
//             }
//
//             SpawnDefinition(_draft);
//         }
//
//         private void SpawnDefinition(ItemDefinition def)
//         {
//             if (def == null) return;
//             if (!Validate(def, out var msg))
//             {
//                 EditorUtility.DisplayDialog("Item Designer – cannot spawn", msg, "OK");
//                 return;
//             }
//
//             var factory = new ItemFactory(_genericPrefab);
//             Vector3 pos = SceneView.lastActiveSceneView ? SceneView.lastActiveSceneView.pivot : Vector3.zero;
//             factory.Spawn(def, pos);
//
//             if (!Application.isPlaying)
//                 EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
//         }
//
//         private void SaveAsset()
//         {
//             if (_draft == null) return;
//             var baseFolder = ItemToolsSettings.GetSelectedFolderOrDefault();
//             ItemToolsSettings.EnsureFolderExists(baseFolder);
//
//             string fileName = BuildFileName(string.IsNullOrWhiteSpace(_draft.displayName)
//                 ? "NewItemDefinition" : _draft.displayName);
//             string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(baseFolder, fileName + ".asset"));
//
//             AssetDatabase.CreateAsset(_draft, path);
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//
//             EditorUtility.FocusProjectWindow();
//             Selection.activeObject = _draft;
//             _draft = null;
//             _so = null;
//             _effectsList = null;
//             RefreshLibrary();
//         }
//
//         private void SaveAssetAs()
//         {
//             if (_draft == null) return;
//             var baseFolder = ItemToolsSettings.GetSelectedFolderOrDefault();
//             string fileName = BuildFileName(string.IsNullOrWhiteSpace(_draft.displayName)
//                 ? "NewItemDefinition" : _draft.displayName);
//
//             var path = EditorUtility.SaveFilePanelInProject(
//                 "Save ItemDefinition",
//                 fileName + ".asset",
//                 "asset",
//                 "Pick location for the ItemDefinition asset",
//                 baseFolder);
//
//             if (string.IsNullOrEmpty(path)) return;
//
//             AssetDatabase.CreateAsset(_draft, path);
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//             EditorUtility.FocusProjectWindow();
//             Selection.activeObject = _draft;
//             _draft = null;
//             _so = null;
//             _effectsList = null;
//             RefreshLibrary();
//         }
//
//         private void DuplicateDraft()
//         {
//             var clone = Instantiate(_draft);
//             clone.name = _draft.name + "_Copy";
//             _draft = clone;
//             _so = new SerializedObject(_draft);
//             SetupEffectsList();
//         }
//
//         /*──────────── Validation ────────────*/
//         private bool Validate(ItemDefinition def, out string message)
//         {
//             var sb = new System.Text.StringBuilder();
//             if (def == null) sb.AppendLine("• ItemDefinition is null");
//             if (def != null)
//             {
//                 if (string.IsNullOrWhiteSpace(def.displayName))
//                     sb.AppendLine("• Display Name is empty");
//                 if (def.icon == null)
//                     sb.AppendLine("• Icon not set");
//                 if (def.effects == null || def.effects.Count == 0)
//                     sb.AppendLine("• Add at least one effect");
//             }
//             message = sb.ToString();
//             return message.Length == 0;
//         }
//
//         /*──────────── Helpers ────────────*/
//         private static string BuildFileName(string name)
//         {
//             if (string.IsNullOrWhiteSpace(name)) return "NewItemDefinition";
//             name = name.Trim();
//             name = Regex.Replace(name, "\\s+", " "); // collapse ws
//             foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
//             name = ItemToolsSettings.S.useHyphenInsteadOfUnderscore ? name.Replace(' ', '-') : name.Replace(' ', '_');
//             if (ItemToolsSettings.S.uppercaseFilenames) name = name.ToUpperInvariant();
//             return name;
//         }
//
//         // Draws a sprite thumbnail even if it's part of an atlas (cropped). Tries AssetPreview first.
//         private static void DrawSpriteIcon(Rect r, Sprite sprite)
//         {
//             if (sprite == null) return;
//             var preview = AssetPreview.GetAssetPreview(sprite) as Texture2D;
//             if (preview == null) preview = AssetPreview.GetMiniThumbnail(sprite) as Texture2D;
//             if (preview != null)
//             {
//                 GUI.DrawTexture(r, preview, ScaleMode.ScaleToFit, true);
//                 return;
//             }
//             var tex = sprite.texture;
//             if (tex == null) return;
//             var tr = sprite.textureRect; // pixels inside atlas
//             var uv = new Rect(tr.x / tex.width, tr.y / tex.height, tr.width / tex.width, tr.height / tex.height);
//             GUI.DrawTextureWithTexCoords(r, tex, uv, true);
//         }
//     }
// }
// #endif

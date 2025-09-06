using System;
using System.Linq;

namespace CMD.Services
{
   public static class CatalogAutoRegistrar
    {
        public static void Run(ICatalog catalog)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var t in asms.SelectMany(a => SafeTypes(a)))
            {
                if (t.IsAbstract) continue;
                if (!typeof(ICatalogRegistrar).IsAssignableFrom(t)) continue;
                try { ((ICatalogRegistrar)Activator.CreateInstance(t)).RegisterInto(catalog); }
                catch (Exception e) { UnityEngine.Debug.LogException(e); }
            }
        }
        private static Type[] SafeTypes(System.Reflection.Assembly a)
        {
            try { return a.GetTypes(); }
            catch { return Array.Empty<Type>(); }
        }
    }
}

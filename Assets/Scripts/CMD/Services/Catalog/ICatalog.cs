namespace CMD.Services
{
    public interface ICatalog
    {
        void RegisterFolder<T>(IFolderCatalog<T> folder) where T : UnityEngine.Object;

        T Get<T>(string key) where T : UnityEngine.Object;
        bool TryGet<T>(string key, out T asset) where T : UnityEngine.Object;

        // новые
        T Get<T>(CMD.Services.CatalogKey<T> catalogKey) where T : UnityEngine.Object;
        bool TryGet<T>(CMD.Services.CatalogKey<T> catalogKey, out T asset) where T : UnityEngine.Object;
    }

}

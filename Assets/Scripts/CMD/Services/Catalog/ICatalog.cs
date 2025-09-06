namespace CMD.Services
{
    public interface ICatalog
    {
        void RegisterFolder<T>(IFolderCatalog<T> folder) where T : UnityEngine.Object;
        T Get<T>(string key) where T : UnityEngine.Object;
        bool TryGet<T>(string key, out T asset) where T : UnityEngine.Object;
    }
}

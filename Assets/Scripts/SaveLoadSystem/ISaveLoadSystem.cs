namespace SaveLoadSystem
{
    public interface ISaveLoadSystem
    {
        public void Save(ISaveLoadObject objectToSave);
        public void Load(ISaveLoadObject objectToSave);
    }
}
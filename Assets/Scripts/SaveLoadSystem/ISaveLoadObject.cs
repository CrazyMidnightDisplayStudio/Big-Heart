namespace SaveLoadSystem
{
    public interface ISaveLoadObject
    {
        /// <summary>
        /// Имя секции в общем JSON-файле.
        /// </summary>
        string SaveKey { get; }

        /// <summary>
        /// Получает данные для сохранения.
        /// </summary>
        object GetSaveData();

        /// <summary>
        /// Восстанавливает объект из сохранённых данных.
        /// </summary>
        void LoadFromSaveData(object data);
    }
}
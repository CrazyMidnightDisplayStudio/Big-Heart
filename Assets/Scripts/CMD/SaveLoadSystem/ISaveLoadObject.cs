namespace CMD.SaveLoadSystem
{
    /// <summary>Участник системы сохранений.</summary>
    public interface ISaveLoadObject
    {
        /// <summary>Глобально уникальный id чанка. Пример: "entity:{guid}" или "go:{stableId}/comp:xyz"</summary>
        string ComponentSaveId { get; }

        /// <summary>Собрать данные для сейва.</summary>
        SaveLoadData Capture();

        /// <summary>Применить восстановленные данные.</summary>
        void RestoreData(SaveLoadData data);
    }
}

using System.Collections.Generic;
using CMD.SaveLoadSystem;

namespace CMD.Services
{
    /// <summary>Стратегия хранения сейва (файл, облако и т.п.).</summary>
    public interface ISaveLoadStrategy
    {
        /// <summary>Сохранить набор участников (стратегия сама соберёт чанки у каждого).</summary>
        void Save(IEnumerable<ISaveLoadObject> objectsToSave);

        /// <summary>Прочитать все чанки из хранилища. Пустой массив — если сейва нет.</summary>
        SaveLoadSystem.SaveLoadData[] Load();
    }
}

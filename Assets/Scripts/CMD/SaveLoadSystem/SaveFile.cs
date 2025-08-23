using System;
using System.Collections.Generic;

namespace CMD.SaveLoadSystem
{
    /// <summary>Файл сейва верхнего уровня.</summary>
    [Serializable]
    public sealed class SaveFile
    {
        public DateTime SaveTime { get; set; } = DateTime.Now;
        public List<SaveLoadData> Data { get; set; } = new();
    }
}

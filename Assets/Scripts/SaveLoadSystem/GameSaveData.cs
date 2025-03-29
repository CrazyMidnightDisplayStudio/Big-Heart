using System;
using System.Collections.Generic;

namespace SaveLoadSystem
{
    [Serializable]
    public class GameSaveData
    {
        public Dictionary<string, string> Data = new();
    }
}
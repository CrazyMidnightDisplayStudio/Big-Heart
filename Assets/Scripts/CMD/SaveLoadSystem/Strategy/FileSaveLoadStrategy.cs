using System;
using System.Collections.Generic;
using System.IO;
using CMD.Common;
using CMD.SaveLoadSystem;
using UnityEngine;

namespace CMD.Services
{
    /// <summary>JSON-файл в Application.persistentDataPath, сериализация через Odin.</summary>
    public sealed class FileSaveLoadStrategy : ISaveLoadStrategy
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public FileSaveLoadStrategy(string fileName = "GameSave.json", string subFolder = "Saves")
        {
            _folderPath = Path.Combine(Application.persistentDataPath, subFolder);
            _filePath = Path.Combine(_folderPath, fileName);
        }

        public void Save(IEnumerable<ISaveLoadObject> objectsToSave)
        {
            var list = new List<SaveLoadData>();
            foreach (var o in objectsToSave)
            {
                try
                {
                    var chunk = o?.Capture();
                    if (chunk != null)
                    {
                        list.Add(chunk);
                    }
                }
                catch (Exception ex) { Debug.LogException(ex); }
            }

            var save = new SaveFile
            {
                Data = list, SaveTime = DateTime.Now
            };

            Directory.CreateDirectory(_folderPath);
            File.WriteAllText(_filePath, OdinJson.ToJson(save));
            Debug.Log($"[Save] {list.Count} chunks → {_filePath}");
        }

        public SaveLoadData[] Load()
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning($"[Load] file not found: {_filePath}");
                return Array.Empty<SaveLoadData>();
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                var save = OdinJson.FromJson<SaveFile>(json);
                return save?.Data?.ToArray() ?? Array.Empty<SaveLoadData>();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return Array.Empty<SaveLoadData>();
            }
        }
    }
}

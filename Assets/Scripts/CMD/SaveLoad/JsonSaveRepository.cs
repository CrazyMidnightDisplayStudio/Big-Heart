using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CMD.Base
{
    [Serializable] internal class SaveList
    {
        public List<EntitySaveData> items = new();
    }

    public class JsonSaveRepository : ISaveRepository
    {
        readonly string _path;
        readonly Dictionary<string, EntitySaveData> _map = new();

        public JsonSaveRepository(string filePath)
        {
            _path = filePath;
            Load();
        }

        public void Upsert(EntitySaveData data) => _map[data.entityId] = data;
        public bool TryGet(string instanceId, out EntitySaveData data) => _map.TryGetValue(instanceId, out data);

        public IEnumerable<EntitySaveData> All() => _map.Values;

        public void Remove(string instanceId) => _map.Remove(instanceId);

        public bool Flush()
        {
            try
            {
                var list = new SaveList { items = new List<EntitySaveData>(_map.Values) };
                var json = JsonUtility.ToJson(list, prettyPrint: true);
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
                File.WriteAllText(_path, json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {_path}\n{e}");
                return false;
            }
        }

        private void Load()
        {
            _map.Clear();
            if (!File.Exists(_path)) return;
            var json = File.ReadAllText(_path);
            var list = JsonUtility.FromJson<SaveList>(json);
            if (list?.items == null) return;
            foreach (var item in list.items) _map[item.entityId] = item;
        }
    }
}

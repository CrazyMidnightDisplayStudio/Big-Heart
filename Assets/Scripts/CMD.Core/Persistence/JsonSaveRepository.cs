using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CMD.Core
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
            TryLoad();
        }

        public void Upsert(EntitySaveData data)
        {
            _map[data.instanceId] = data;
            TrySave();
        }
        public bool TryGet(string instanceId, out EntitySaveData data) => _map.TryGetValue(instanceId, out data);

        public IEnumerable<EntitySaveData> All() => _map.Values;

        public void Remove(string instanceId)
        {
            if (_map.Remove(instanceId))
            {
                TrySave();
            }
        }

        private void TrySave()
        {
            var list = new SaveList
            {
                items = new List<EntitySaveData>(_map.Values)
            };
            var json = JsonUtility.ToJson(list, prettyPrint: false);
            File.WriteAllText(_path, json);
        }

        private void TryLoad()
        {
            if (!File.Exists(_path))
            {
                return;
            }
            string json = File.ReadAllText(_path);
            var list = JsonUtility.FromJson<SaveList>(json);
            _map.Clear();
            if (list?.items != null)
            {
                foreach (var it in list.items)
                {
                    _map[it.instanceId] = it;
                }
            }
        }
    }
}

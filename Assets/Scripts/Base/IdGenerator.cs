using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Base
{
    public static class IdGenerator
    {
        private static int _counter = 0;
        private static readonly HashSet<string> _idSet = new();

        public static string NewId()
        {
            string id;
            do
            {
                id = $"{Interlocked.Increment(ref _counter)}";
            } while (!_idSet.Add(id));

            return id;
        }

        public static bool TryReserveId(string id)
        {
            if (!_idSet.Add(id))
            {
                Debug.LogError($"{id} is already reserved");
                return false;
            }

            return true;
        }

        public static bool TryReleaseId(string id)
        {
            if (!_idSet.Remove(id))
            {
                Debug.LogError($"{id} not found");
                return false;
            }

            return true;
        }

        public static void Reset()
        {
            Debug.LogWarning("IdGenerator reset");
            _idSet.Clear();
            _counter = 0;
        }
    }
}
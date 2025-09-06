#if UNITY_EDITOR
using System;
using BigHeart.Services;
using CMD.Core;
using CMD.GD;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace BigHeart.GD
{
    [Serializable]
    public sealed class SaveLoadTab : IToolsTab
    {
        [InfoBox("Работает только в Play Mode (нужен ServiceRegistry).", InfoMessageType.None)]
        [ReadOnly, LabelText("persistentDataPath")]
        public string p = Application.persistentDataPath;

        [PropertySpace(6)]
        [Button("Save (F5)"), GUIColor(0.2f, 0.7f, 1f)]
        private void Save()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Play Mode required", "Запусти игру.", "OK");
                return;
            }
            var svc = ServiceRegistry.Get<ISaveLoadService>();
            if (svc == null)
            {
                EditorUtility.DisplayDialog("Service missing", "ISaveLoadService недоступен.", "OK");
                return;
            }
            svc.SaveGame();
        }

        [Button("Load (F9)"), GUIColor(1f, 0.5f, 0.2f)]
        private void Load()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Play Mode required", "Запусти игру.", "OK");
                return;
            }
            var svc = ServiceRegistry.Get<ISaveLoadService>();
            if (svc == null)
            {
                EditorUtility.DisplayDialog("Service missing", "ISaveLoadService недоступен.", "OK");
                return;
            }
            svc.LoadGameFresh();
        }

        public void EnsureInit()
        { /* ничего */
        }
        public void SetGroup(string g) { }
    }

}
#endif

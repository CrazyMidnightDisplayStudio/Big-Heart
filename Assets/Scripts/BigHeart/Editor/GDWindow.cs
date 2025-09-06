#if UNITY_EDITOR
using CMD.GD;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace BigHeart.GD
{
    class GDWindow : BaseGDWindow
    {
        // ---------- Main window -----------
        [MenuItem("BigHeart/GD Window %#g")]
        private static void OpenWindow() => Open<GDWindow>("GD Window");

        // ---------- Tabs -----------
        [TabGroup("Tools", "Spawner"), InlineProperty, HideLabel]
        [SerializeReference] private SpawnerTab _spawner;

        [TabGroup("Tools", "Save & Load"), InlineProperty, HideLabel]
        [SerializeReference] private SaveLoadTab _saveLoad;

        protected override void OnEnable()
        {
            base.OnEnable();

            // лениво создаём инстансы
            _spawner ??= GetOrAddTab<SpawnerTab>("Spawner");
            _saveLoad ??= GetOrAddTab<SaveLoadTab>("Save & Load");
        }
    }
}

#endif

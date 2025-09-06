#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace CMD.GD
{
    /// <summary>
    /// Окно геймдизайнера - фасад инструментов геймдизайнера
    /// фичи создаются как IToolsTab - это могут быть спавнер / управление временем игры и т.д.
    ///
    /// НЕ ЗАБУДЬ ХРАНИТЬ ВСЕ EDITOR ИНСТРУМЕНТЫ В #if UNITY_EDITOR
    /// </summary>
    public abstract class BaseGDWindow : OdinEditorWindow
    {
        protected static T Open<T>(string title) where T : BaseGDWindow
            => GetWindow<T>(title);

        [SerializeReference, HideInInspector]
        private List<IToolsTab> _tabs = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            _tabs ??= new List<IToolsTab>();
            foreach (var t in _tabs) t?.EnsureInit();
        }

        protected T GetOrAddTab<T>(string group) where T : class, IToolsTab, new()
        {
            var t = _tabs.OfType<T>().FirstOrDefault();
            if (t == null)
            {
                t = new T();
                t.SetGroup(group);
                t.EnsureInit();
                _tabs.Add(t);
            }
            return t;
        }
    }
}
#endif

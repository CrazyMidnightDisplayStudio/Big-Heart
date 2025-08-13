using Core;
using Entities.Base;
using Entities.Item.Model;
using UnityEngine;

namespace Entities.Item.View
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ItemView : MonoBehaviour, IViewBinder<ItemDefinition>
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Кэш для тултипа/отладки
        private string _title;
        private string _description;

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>Первичное связывание с ассетом.</summary>
        public void Bind(ItemDefinition def)
        {
            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

            if (def == null)
            {
                spriteRenderer.sprite = null;
                _title = _description = string.Empty;
                return;
            }

            spriteRenderer.sprite = def.icon;
            _title       = string.IsNullOrWhiteSpace(def.displayName) ? def.name : def.displayName;
            _description = def.description;
        }

        /// <summary>
        /// Обновление визуала с учётом состояния экземпляра (если нужно).
        /// Сейчас просто переиспользует icon из Definition,
        /// но здесь можно переключать спрайты по state (надкусан/гнилой и т.п.).
        /// </summary>
        public void Refresh(ItemDefinition def, ItemState state)
        {
            if (!def) return;

            // TODO: если введёшь отдельные спрайты под состояния — выбери их здесь.
            var icon = def.icon;

            spriteRenderer.sprite = icon;
            _title       = string.IsNullOrWhiteSpace(def.displayName) ? def.name : def.displayName;
            _description = def.description;
        }

        // Опциональный тултип (оставь, если у тебя есть TooltipScreenSpaceUI)
        private void OnMouseOver()
        {
#if !UNITY_SERVER
            TooltipScreenSpaceUI.ShowTooltip_Static(() => $"{_title}\n{_description}");
#endif
        }

        private void OnMouseExit()
        {
#if !UNITY_SERVER
            TooltipScreenSpaceUI.HideTooltip_Static();
#endif
        }
    }
}

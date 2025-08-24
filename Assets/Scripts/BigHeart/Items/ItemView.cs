using CMD.Base;
using CMD.View;
using UnityEngine;
using UnityEngine.UI;

namespace BigHeart
{
    [RequireComponent(typeof(Image))]
    public sealed class ItemView : MonoBehaviour, IView<ItemRuntime>
    {
        [Header("UI refs")]
        [SerializeField] private Image iconImage; // иконка
        [SerializeField] private GameObject nameRoot; // контейнер имени (можно скрыть)
        [SerializeField] private Text nameText; // или TMP_Text
        [SerializeField] private GameObject descRoot; // контейнер описания
        [SerializeField] private Text descText; // или TMP_Text

        private void Reset()
        {
            iconImage = GetComponent<Image>();
        }

        public void Bind(ItemRuntime runtime)
        {
            var def = runtime.Definition;

            // Иконка
            var icon = (def as IHaveIcon)?.Icon;
            if (iconImage != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = icon != null;
            }

            // Имя
            var name = (def as IHaveDisplayName)?.DisplayName;
            if (nameRoot) nameRoot.SetActive(!string.IsNullOrEmpty(name));
            if (nameText) nameText.text = name ?? "";

            // Описание
            var desc = (def as IHaveDescription)?.Description;
            if (descRoot) descRoot.SetActive(!string.IsNullOrEmpty(desc));
            if (descText) descText.text = desc ?? "";

            // Для удобства в иерархии
            if (!string.IsNullOrEmpty(name))
                gameObject.name = $"ItemView:{name}";
        }
    }
}

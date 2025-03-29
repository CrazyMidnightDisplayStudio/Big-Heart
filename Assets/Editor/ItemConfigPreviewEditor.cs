using Configs;
using UnityEditor;
using UnityEngine;

namespace ItemSystem.Editor
{
    [CustomEditor(typeof(ItemConfig))]
    public class ItemConfigPreviewEditor : UnityEditor.Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            ItemConfig config = (ItemConfig)target;

            if (config.icon == null)
                return base.RenderStaticPreview(assetPath, subAssets, width, height);

            // Получаем спрайт
            Sprite sprite = config.icon;

            // Создаём новую текстуру только для области спрайта
            Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] pixels = sprite.texture.GetPixels(
                (int)sprite.rect.x, (int)sprite.rect.y,
                (int)sprite.rect.width, (int)sprite.rect.height
            );

            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            // Меняем размер, чтобы соответствовать предпросмотру
            Texture2D resizedTexture = new Texture2D(width, height);
            Graphics.ConvertTexture(croppedTexture, resizedTexture);

            return resizedTexture;
        }
    }
}

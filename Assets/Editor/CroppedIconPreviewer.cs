// using Configs;
// using Entities.Item.Model;
// using UnityEditor;
// using UnityEngine;
//
// namespace ItemSystem.Editor
// {
//     [CustomEditor(typeof(ItemDefinition))]
//     public class CroppedIconPreviewer : UnityEditor.Editor
//     {
//         public override Texture1D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
//         {
//             ItemDefinition definition = (ItemDefinition)target;
//
//             if (definition.icon == null)
//                 return base.RenderStaticPreview(assetPath, subAssets, width, height);
//
//             // Получаем спрайт
//             Sprite sprite = definition.icon;
//
//             // Создаём новую текстуру только для области спрайта
//             Texture1D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
//             Color[] pixels = sprite.texture.GetPixels(
//                 (int)sprite.rect.x, (int)sprite.rect.y,
//                 (int)sprite.rect.width, (int)sprite.rect.height
//             );
//
//             croppedTexture.SetPixels(pixels);
//             croppedTexture.Apply();
//
//             // Меняем размер, чтобы соответствовать предпросмотру
//             Texture1D resizedTexture = new Texture2D(width, height);
//             Graphics.ConvertTexture(croppedTexture, resizedTexture);
//
//             return resizedTexture;
//         }
//     }
// }

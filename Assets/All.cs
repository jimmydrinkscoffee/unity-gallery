using UnityEngine;
using UnityEngine.UI;

public class All : MonoBehaviour
{
    [SerializeField] Image _image;

    public void PickImage()
    {
        var permission = NativeGallery.GetImageFromGallery(path =>
        {
            if (path == null)
            {
                Debug.Log("No image is chosen");
                return;
            }

            var tex = NativeGallery.LoadImageAtPath(path);
            if (tex == null)
            {
                Debug.Log($"Cannot load image at path ${path}");
            }

            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            _image.sprite = sprite;
        });
    }
}

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class All : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _filename;

    string _path = string.Empty;

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

            _path = path;
            _filename.text = _path;
        });
    }

    public async void Upload()
    {
        if (string.IsNullOrEmpty(_path) || string.IsNullOrWhiteSpace(_path))
        {
            Debug.Log("No file to upload");
            return;
        }

        var contentInBytes = await File.ReadAllBytesAsync(_path);

        var reqForm = new List<IMultipartFormSection>();
        reqForm.Add(new MultipartFormDataSection("data", contentInBytes));

        var apiUrl = "https://ipfs-server.vercel.app/upload";
        var req = UnityWebRequest.Post(apiUrl, reqForm);

        var reqSender = req.SendWebRequest();
        while (!reqSender.isDone)
        {
            await Task.Delay(100 / 24);
        }

        Debug.Log("Result: " + req.result.ToString());
    }
}

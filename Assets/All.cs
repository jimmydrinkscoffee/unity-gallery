using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System.Collections;

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

    public void Upload()
    {
        StartCoroutine(IEUpload());
    }

    IEnumerator IEUpload()
    {
#if UNITY_EDITOR
        _path = $"{Application.dataPath}/sample-image.jpg";
#endif

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", File.ReadAllBytes(_path), "sample-image.jpg");

        var apiUrl = "https://ipfs-server.vercel.app/uploadFile";

        UnityWebRequest req = UnityWebRequest.Post(apiUrl, form);

        var reqSender = req.SendWebRequest();
        yield return reqSender;

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("No internet");
            yield break;
        }

        var res = req.downloadHandler.text;
        if (req.result != UnityWebRequest.Result.Success)
        {
            var log = new
            {
                errorCode = req.responseCode,
                error = req.result.ToString(),
                response = res
            };
            Debug.LogError(JsonConvert.SerializeObject(log));
            yield break;
        }

        Debug.Log("Upload successfully: " + res);
    }

    private async void NewStyledUpload()
    {
#if UNITY_EDITOR
        _path = $"{Application.dataPath}/sample-image.jpg";
#endif

        if (string.IsNullOrEmpty(_path) || string.IsNullOrWhiteSpace(_path))
        {
            Debug.Log("No file to upload");
            return;
        }

        if (!File.Exists(_path))
        {
            Debug.Log("File does not exist");
            return;
        }

        Debug.Log($"Start upload: {_path}");

        var contentInBytes = File.ReadAllBytes(_path);
        var filename = Path.GetFileName(_path);

        var reqForm = new List<IMultipartFormSection>();
        reqForm.Add(new MultipartFormFileSection("file", contentInBytes, filename, "image/jpeg"));

        Debug.Log("Form: " + JsonConvert.SerializeObject(reqForm));

        var apiUrl = "https://ipfs-server.vercel.app/uploadFile";

        var req = UnityWebRequest.Post(apiUrl, reqForm);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "multipart/form-data");

        // var body = new { data = contentInBytes };
        // var bodyInStr = JsonConvert.SerializeObject(body);

        // Debug.Log("Request body: " + bodyInStr);

        // var bodyInBytes = Encoding.UTF8.GetBytes(bodyInStr);

        // var req = new UnityWebRequest(apiUrl, "POST");

        // req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyInBytes);
        // req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        // req.SetRequestHeader("Content-Type", "image/jpg");

        // Debug.Log("Request header: " + req.GetRequestHeader("Content-Type"));

        var reqSender = req.SendWebRequest();
        while (!reqSender.isDone)
        {
            await Task.Delay(100 / 24);
        }

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("No internet");
            return;
        }

        var res = req.downloadHandler.text;
        if (req.result != UnityWebRequest.Result.Success)
        {
            var log = new
            {
                errorCode = req.responseCode,
                error = req.result.ToString(),
                response = res
            };
            Debug.LogError(JsonConvert.SerializeObject(log));
            return;
        }
    }
}

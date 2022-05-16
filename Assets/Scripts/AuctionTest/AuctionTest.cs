using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TempUser
{
    public string userId;
}
public class AuctionTest : MonoBehaviour
{
    private string URL = "http://127.0.0.1:5000";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateUserId();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(GetUsers());
        }
    
    }

    // Users
    IEnumerator GetUsers()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL + "/api/users");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Debug.Log(www.downloadHandler.text);
        }
    }

    private void CreateUserId()
    {
        // Dictionary<string, string> data = new Dictionary<string, string>();
        // string userId = System.Guid.NewGuid().ToString();
        // data.Add("id", userId);
        // List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        // form.Add(new MultipartFormDataSection(string.Format("id={0}", System.Guid.NewGuid().ToString())));

        TempUser user = new TempUser
        {
            userId = System.Guid.NewGuid().ToString()
        };

        StartCoroutine(Post(URL + "/api/users/create", JsonUtility.ToJson(user)));
    }

    IEnumerator Post(string url, string jsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Debug.Log("Status Code: " + request.responseCode);
        // Debug.Log(request.downloadHandler.text);
    }
}

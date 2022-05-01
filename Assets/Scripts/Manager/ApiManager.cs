using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

public class UserForAPI
{
    public int id;
    public string userId;
}

public class AuctionItemForAPI
{
    public AuctionItem[] result;
}

public class AuctionItem
{
    public string userId;
    public int itemId;
    public int price;
    public string rank;
    public string type;
    public int time;
}

public class ApiManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    private string URL = "http://127.0.0.1:5000";       // TEMP

    // Users
    public void CheckUser(string userId)
    {
        StartCoroutine(CheckUserById(userId, returnValue => 
        {
            if (returnValue == "error")
            {
                Debug.LogError("server error");
                return;
            }

            UserForAPI userInfo = JsonMapper.ToObject<UserForAPI>(returnValue);
            if (userInfo.id == 0)
            {
                // 유저 정보 생성
                TempUser user = new TempUser
                {
                    userId = userId
                };

                StartCoroutine(Post(URL + "/api/users/create", JsonUtility.ToJson(user)));

                // !!! 생성 확인 미구현
            }
            // else
            // {
            //     Debug.Log("유저 정보 있음!");
            //     // lastLogin 업데이트
            // }
        }));
    }

    IEnumerator CheckUserById(string userId, System.Action<string> callback = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL + "/api/users/" + userId);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            callback.Invoke("error");
        }
        else
        {
            callback.Invoke(www.downloadHandler.text);
        }
    }

    private void CreateUser()
    {
        // Dictionary<string, string> data = new Dictionary<string, string>();
        // string userId = System.Guid.NewGuid().ToString();
        // data.Add("id", userId);
        // List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        // form.Add(new MultipartFormDataSection(string.Format("id={0}", System.Guid.NewGuid().ToString())));

        UserForAPI user = new UserForAPI
        {
            userId = System.Guid.NewGuid().ToString()
        };

        StartCoroutine(Post(URL + "/api/users/create", JsonUtility.ToJson(user)));
    }

    // Auction
    public void GetAuctionItemList()
    {
        // gameManager.RequestAuctionItemList()에서 호출

        StartCoroutine(GetAuction(returnValue => 
        {
            if (returnValue == "error")
            {
                Debug.LogError("server error");
                return;
            }

            AuctionItemForAPI auctionItemList = JsonMapper.ToObject<AuctionItemForAPI>("{\"result\": " + returnValue + "}");
            
            gameManager.ResponseAuctionItemList(auctionItemList);
        }));
    }

    IEnumerator GetAuction(System.Action<string> callback = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL + "/api/auction");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            callback.Invoke("error");
        }
        else
        {
            callback.Invoke(www.downloadHandler.text);
        }
    }

    IEnumerator Post(string url, string jsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
    }
}

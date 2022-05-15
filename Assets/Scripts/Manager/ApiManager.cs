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
    public int id;
    public string userId;
    public int itemId;
    public int price;
    public string rank;
    public string type;
    public int time;
}

public class PushForAPI
{
    public PushItem[] result;
}

public class PushItem
{
    public int id;
    public string userId;
    public int itemId;
    public string message;
    public int time;
    public int gold;
}

public class ApiManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    // private string URL = "http://127.0.0.1:5000";       // TEMP

    // -------------------------------------------------------------
    // Users
    // -------------------------------------------------------------
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

                StartCoroutine(Post(gameManager.SaveManager.Save.URL + "/api/users/create", JsonUtility.ToJson(user)));

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
        UnityWebRequest www = UnityWebRequest.Get(gameManager.SaveManager.Save.URL + "/api/users/" + userId);
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

        StartCoroutine(Post(gameManager.SaveManager.Save.URL + "/api/users/create", JsonUtility.ToJson(user)));
    }

    // -------------------------------------------------------------
    // Auction
    // -------------------------------------------------------------
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
        UnityWebRequest www = UnityWebRequest.Get(gameManager.SaveManager.Save.URL + "/api/auction");
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

    public void AddItemToAuction(string _userId, int _itemId, int _price)
    {
        // !!! 임시로 rank, type 고정
        AuctionItem data = new AuctionItem
        {
            userId = _userId,
            itemId = _itemId,
            rank = "common",
            type = "weapon",
            price = _price
        };
        
        StartCoroutine(Post(gameManager.SaveManager.Save.URL + "/api/auction/add", JsonUtility.ToJson(data)));
    }

    public void DeleteAuctionItem(string _userId, int _auctionId)
    {
        
        AuctionItem data = new AuctionItem
        {
            userId = _userId,
            id = _auctionId
        };

        StartCoroutine(Post(gameManager.SaveManager.Save.URL + "/api/auction/delete", JsonUtility.ToJson(data)));
    }
    // -------------------------------------------------------------
    // Push
    // -------------------------------------------------------------
    public void GetPushList(string userId)
    {
        // gameManager.RequestPushList()에서 호출
        StartCoroutine(GetPush(userId, returnValue =>
        {
            if (returnValue == "error")
            {
                Debug.LogError("server error");
                return;
            }

            PushForAPI auctionItemList = JsonMapper.ToObject<PushForAPI>("{\"result\": " + returnValue + "}");
            
            gameManager.ResponsePushList(auctionItemList);
        }));
    }

    IEnumerator GetPush(string userId, System.Action<string> callback = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(gameManager.SaveManager.Save.URL + "/api/push/" + userId);
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

    public void AddPush(string _userId, int _itemId, string _message, int _gold = 0)
    {
        PushItem data = new PushItem
        {
            userId = _userId,
            itemId = _itemId,
            message = _message,
            gold = _gold
        };

        StartCoroutine(Post(gameManager.SaveManager.Save.URL + "/api/push/add", JsonUtility.ToJson(data)));
    }
    
    public void DeletePush(string _userId, int _pushId)
    {
        PushItem data = new PushItem
        {
            userId = _userId,
            id = _pushId
        };

        StartCoroutine(Post(gameManager.SaveManager.Save.URL + "/api/push/delete", JsonUtility.ToJson(data)));
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

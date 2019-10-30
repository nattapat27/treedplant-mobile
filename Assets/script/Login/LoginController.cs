using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;

using UnityEngine.Networking;

public class LoginController : MonoBehaviour
{
    public GameObject InputText;
    public GameObject Popup;
    public GameObject Loading;
    private User user;
    private readonly string basePath = "https://treedp.doge.in.th";
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        user = new User();
        Popup.SetActive(false);
        Loading.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Login()
    {
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log("id: "+aToken.UserId);
            user.Id = aToken.UserId;
            // Print current access token's granted permissions
            FB.API("/me?fields=name", HttpMethod.GET, DisplayUsername);
            FB.API("/me?fields=email", HttpMethod.GET, DisplayEmail);
            StartCoroutine(PostRequest("https://treedp.doge.in.th/searchByFacebook", "{ \"facebook\" : \""+user.Id+"\" }"));
        }
        else
        {
            Debug.Log("user cancel");
        }
    }
    private void DisplayUsername(IGraphResult result)
    {
        if (result.Error != null)
        {
            return;
        }
        Debug.Log(result.ResultDictionary["name"]);
        user.Name = result.ResultDictionary["name"].ToString();
    }
    private void DisplayEmail(IGraphResult result)
    {
        if (result.Error != null)
        {
            return;
        }
        Debug.Log(result.ResultDictionary["email"]);
        user.Email = result.ResultDictionary["email"].ToString();
    }
    private void ShowPopup()
    {

        Popup.SetActive(true);
    }
    public void SubmitLogin()
    {
        var text = InputText.GetComponent<Text>();
        Debug.Log(text.text);
        user.Phone = text.text.ToString();
        Debug.Log(user.Id);
        Debug.Log(user.Name);
        Debug.Log(user.Email);
        Debug.Log(user.Phone);
        SessionApp.user = user;
        string requestData = "{" +
            " \"name\" : \"" + user.Name + "\"," +
            " \"phone\" : \"" + user.Phone + "\"," +
            " \"email\" : \"" + user.Email + "\"," +
            " \"facebook\" : \"" + user.Id + "\"," +
            " \"password\" : \"1234567890\"" +
            "}";
        Debug.Log(requestData);
        StartCoroutine(AddNewUser("https://treedp.doge.in.th/save/token", requestData));
    }   


    IEnumerator PostRequest(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Loading.SetActive(true);
        yield return request.Send();
        Loading.SetActive(false);
        string respone = request.downloadHandler.text;
        if(respone.Equals("[]"))
            ShowPopup();
        else
        {
            Debug.Log(respone);
            SessionApp.userId = Helper.toJsonData(respone)["data"][0]["profileId"].ToString();
            SessionApp.user.Name = Helper.toJsonData(respone)["data"][0]["name"].ToString();
            SessionApp.user.Phone = Helper.toJsonData(respone)["data"][0]["phoneNumber"].ToString();
            SessionApp.user.Email = Helper.toJsonData(respone)["data"][0]["email"].ToString();
            SessionApp.user.Id = Helper.toJsonData(respone)["data"][0]["facebook"].ToString();
            SceneManager.LoadScene("main", LoadSceneMode.Single);
        }
    }
    IEnumerator AddNewUser(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Loading.SetActive(true);
        yield return request.Send();
        Loading.SetActive(false);
        string respone = request.downloadHandler.text;
        Debug.Log(respone);
        SessionApp.userId = JsonMapper.ToObject(respone)["profileId"].ToString();
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }
}

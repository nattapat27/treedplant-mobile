﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public GameObject InputText;
    public GameObject Popup;
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
        Popup.SetActive(false);
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
            // Print current access token's granted permissions
            FB.API("/me?fields=name", HttpMethod.GET, DisplayUsername);
            FB.API("/me?fields=email", HttpMethod.GET, DisplayEmail);
            ShowPopup();
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
    }
    private void DisplayEmail(IGraphResult result)
    {
        if (result.Error != null)
        {
            return;
        }
        Debug.Log(result.ResultDictionary["email"]);
    }
    private void ShowPopup()
    {
        Popup.SetActive(true);
    }
    public void SubmitLogin()
    {
        var text = InputText.GetComponent<Text>();
        Debug.Log(text.text);
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }
}

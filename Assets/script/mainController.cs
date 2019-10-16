using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System;
using LitJson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainController : MonoBehaviour
{
    public GameObject ButtonArea;
    public GameObject ListModel;
    public GameObject List;

    private ModelListButton prefab;
    private bool m_IsQuitting = false;
    private bool status = false;
    public void Start()
    {
        ButtonArea.SetActive(true);
        ListModel.SetActive(false);
        StartCoroutine(ConnectRestApi.sendGetMethod("/asset/getAllAsset/tree"));
    }

    public void Update()
    {
        _UpdateApplicationLifecycle();
    }

    private void _UpdateApplicationLifecycle()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }
        
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    private void _DoQuit()
    {
        Application.Quit();
    }

    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    public void showList()
    {
        ButtonArea.SetActive(false);
        ListModel.SetActive(true);
        if (!status)
        {
            string response = ConnectRestApi.getRespone();
            JsonData assetAndTrees = Helper.toJsonData(response);
            Debug.Log(response);
            
            ShowData(assetAndTrees);
            status = true;
        }
    }

    public void quitList()
    {
        ButtonArea.SetActive(true);
        ListModel.SetActive(false);
    }

    private void ShowData(JsonData data)
    {
        
        {
            for (int i = 0; i < data["data"].Count; i++)
            {
                string item = data["data"][i]["assetName"].GetString();
                item = item.Replace(" ", "");
                string returnValue = item;
                item = "list/" + item;
                GameObject spawnedGameObject = Resources.Load(item) as GameObject;
                GameObject child = Instantiate<GameObject>(spawnedGameObject, new Vector3(0, 0, 0), Quaternion.identity);
                child.transform.SetParent(List.transform);
                child.transform.localScale = new Vector3(1, 1, 1);
                var button = child.GetComponent<Button>();
                button.onClick.AddListener(() => GetButtonValue(returnValue, i));
            }
        } 
    }

    private void GetButtonValue(string input, int id)
    {
        GameObject model = Resources.Load("object/" + input) as GameObject;
        Debug.Log(model);
        ModelGenerator.modelList[input] = model;
        ModelGenerator.id = input;
        ModelGenerator.Index = id;
        quitList();
    }               

    public void GoToCart()
    {
        SceneManager.LoadScene("Cart", LoadSceneMode.Single);
    }
}

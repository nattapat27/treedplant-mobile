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
using GoogleARCore.Examples.ObjectManipulation;

public class mainController : MonoBehaviour
{
    public GameObject ButtonArea;
    public GameObject ListModel;
    public GameObject List;
    
    private ModelListButton prefab;
    private bool m_IsQuitting = false;
    private static bool status = false;
    public void Start()
    {
        status = false;
        ModelGenerator.cart = new Dictionary<string, Cart>();
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
        Debug.Log(status);
        if (!status)
        {
            status = true;
            string response = ConnectRestApi.getRespone();
            JsonData assetAndTrees = Helper.toJsonData(response);
            Debug.Log(response);
            
            ShowData(assetAndTrees);
        }
    }

    public void quitList()
    {
        ButtonArea.SetActive(true);
        ListModel.SetActive(false);
    }
    private void ShowData(JsonData data)
    {
        for (int i = 0; i < data["data"].Count; i++)
        {
            try
            {
                string item = data["data"][i]["assetName"].GetString();
                string assetId = data["data"][i]["assetId"].ToString();
                item = item.Replace(" ", "");
                string returnValue = item;
                item = "list/" + item;
                Debug.Log(item);
                GameObject spawnedGameObject = Resources.Load(item) as GameObject;
                GameObject child = Instantiate<GameObject>(spawnedGameObject, new Vector3(0, 0, 0), Quaternion.identity);
                child.transform.SetParent(List.transform);
                child.transform.localScale = new Vector3(1, 1, 1);
                var button = child.GetComponent<Button>();
                button.onClick.AddListener(() => GetButtonValue(returnValue, assetId));
                Debug.Log(i + ": " + returnValue);
                if (!SessionApp.index.ContainsKey(returnValue))
                {
                    SessionApp.index[returnValue] = i;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Data);
            }
        }
    }

    private void GetButtonValue(string input, string assetId)
    {
        GameObject model = Resources.Load("object/" + input) as GameObject;
        ModelGenerator.modelList[input] = model;
        ModelGenerator.id = input;
        ModelGenerator.Index = SessionApp.index[input];
        ModelGenerator.AssetId = assetId;
        Debug.Log(ModelGenerator.AssetId);
        quitList();
    }               

    public void GoToCart()
    {
        SceneManager.LoadScene("Cart", LoadSceneMode.Single);
    }

    public void DelectModel()
    {
        GameObject obj = ManipulationSystem.Instance.SelectedObject;
        Debug.Log(obj.tag);
        if (obj != null)
        {
            var cart = ModelGenerator.GetCart();
            int number = cart[obj.tag.ToString()].GetNumber();
            if (number - 1 <= 0)
                cart.Remove(obj.tag.ToString());
            else
                cart[obj.tag.ToString()].SetNumber(number - 1);
            Destroy(obj);
        }
    }

}

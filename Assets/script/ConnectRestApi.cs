﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectRestApi : MonoBehaviour
{
    private static string URL = "https://treedp.doge.in.th";
    private static string response;
    public static IEnumerator sendGetMethod(string option)
    {
        string request = URL + option;

        using (UnityWebRequest www = UnityWebRequest.Get(request))
        {
            // www.chunkedTransfer = false;
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                // Debug.Log("++++++++++++++");
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                }
            }
        }
    }
    public static string getRespone()
    {
        return response;
    }
    public static IEnumerator Wait(float waitTime)
    {
        while(true)
            yield return new WaitForSeconds(waitTime);
    }
}

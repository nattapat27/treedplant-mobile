using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectRestApi : MonoBehaviour
{
    private static string URL = "https://treedp.doge.in.th";
    public IEnumerator getMethod(string option)
    {
        string request = URL + option;
        using (UnityWebRequest www = UnityWebRequest.Get(request))
        {
            // www.chunkedTransfer = false;
#pragma warning disable CS0618 // Type or member is obsolete
            yield return www.Send();
#pragma warning restore CS0618 // Type or member is obsolete
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    string result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log(result);
                    
                }
            }
        }
    }
}

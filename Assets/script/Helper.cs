using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Helper : MonoBehaviour
{
    public void LoadImage(string url, Image image)
    {
        
    }
    public IEnumerator WaitNextFrame()
    {
        yield return new WaitForEndOfFrame();
    }
    public static JsonData toJsonData(string json)
    {
        string newJson = "{ \"data\" : " + json + " }";
        JsonData list = JsonMapper.ToObject(newJson);
        return list;
    }
}

public static class TextureExtentions
{
    public static Texture2D ToTexture2D(this Texture texture)
    {
        return Texture2D.CreateExternalTexture(
            texture.width,
            texture.height,
            TextureFormat.RGB24,
            false, false,
            texture.GetNativeTexturePtr());
    }
}
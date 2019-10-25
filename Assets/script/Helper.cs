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
    public static GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
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

public class Cart
{
    private int id;
    private string name;
    private int number;
    public Cart(int id, string name, int number)
    {
        this.id = id;
        this.name = name;
        this.number = number;
    }
    public void SetId(int id)
    {
        this.id = id;
    }
    public void SetName(string  name)
    {
        this.name = name;
    }
    public void SetNumber(int number)
    {
        this.number = number;
    }

    public int GetId()
    {
        return id;
    }
    public string GetName()
    {
        return name;
    }
    public int GetNumber()
    {
        return number;
    }

}
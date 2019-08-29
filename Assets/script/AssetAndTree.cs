using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using LitJson;

public class JsonHelper
{
    public static JsonData toJsonData(string json)
    {
        string newJson = "{ \"data\" : " + json + " }";
        JsonData list = JsonMapper.ToObject(newJson);
        return list;
    }
}
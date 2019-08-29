using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class JsonHelper
{
    public static T[] toAssetAndTree<T>(string json)
    {
        string newJson = "{ \"array\" : " + json + " }";
        Debug.Log(newJson);
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = JsonUtility.FromJson<Wrapper<T>>(newJson).array;
        Debug.Log(wrapper.array);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

}
public class TreeId
{
    public int treeId { get; set; }
    public string model { get; set; }
    public double width { get; set; }
    public double height { get; set; }
}

public class TypeId
{
    public int typeId { get; set; }
    public string typeName { get; set; }
}

public class AssetAndTree
{
    public int assetId { get; set; }
    public int price { get; set; }
    public string assetName { get; set; }
    public string asssetImage { get; set; }
    public string assetDetail { get; set; }
    public TreeId treeId { get; set; }
    public TypeId typeId { get; set; }
}

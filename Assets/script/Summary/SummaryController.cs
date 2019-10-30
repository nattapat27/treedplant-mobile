using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SummaryController : MonoBehaviour
{
    public GameObject Content;
    public GameObject TextPrice;
    public GameObject TextTotalPrice;
    public GameObject AddressShipping;
    public GameObject PriceInFooter;
    public GameObject Loading;
    private JsonData data;
    private Dictionary<string, Cart> items;
    private int totalPrice = 0;

    // Start is called before the first frame update
    void Start()
    {
        Loading.SetActive(false);
        data = Helper.toJsonData(ConnectRestApi.getRespone());
        items = ModelGenerator.GetCart();
        
        List<Cart> carts = items.Values.ToList<Cart>();
        Debug.Log(carts);
        foreach (Cart cart in carts)
        {
            string objectCart = "Cart/" + cart.GetName();
            GameObject spawnedGameObject = Resources.Load(objectCart) as GameObject;
            int number = Convert.ToInt32(cart.GetNumber());
            GameObject Number = Helper.GetChildWithName(spawnedGameObject, "Number");
            Number.GetComponent<Text>().text = number.ToString();
            int price = Convert.ToInt32(data["data"][Convert.ToInt32(cart.GetId().ToString())]["price"].ToString());
            GameObject Price = Helper.GetChildWithName(spawnedGameObject, "Price");
            Price.GetComponent<Text>().text = (price * number).ToString();
            var obj = Instantiate(spawnedGameObject, new Vector3(0, 0, 0), Quaternion.identity);
            obj.transform.SetParent(Content.transform);
            totalPrice += price * number;
        }
        TextPrice.GetComponent<Text>().text = "฿"+totalPrice.ToString();
        TextTotalPrice.GetComponent<Text>().text = "฿" + (totalPrice + 100).ToString();
        PriceInFooter.GetComponent<Text>().text = "฿" + (totalPrice + 100).ToString();

        string address = "";
        address += SessionApp.user.Name+" "+SessionApp.user.Phone+"\n";
        address += SessionApp.address.Detail + "\n";
        address += SessionApp.address.District + " ";
        address += SessionApp.address.Province + " ";
        address += SessionApp.address.Zipcode;

        AddressShipping.GetComponent<Text>().text = address;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Back() 
    {
        SceneManager.UnloadScene("summary product");
    }

    public void Order()
    {
        string requestBody = "{" +
            " \"total\" : \"" + totalPrice + "\"," +
            " \"addressId\" : \"" + SessionApp.addressId + "\"," +
            " \"profileId\" : \"" + SessionApp.userId + "\""+
            "}";
        StartCoroutine(AddOrder("https://treedp.doge.in.th/add/order", requestBody));
    }
    IEnumerator AddOrder(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Loading.SetActive(true);
        yield return request.Send();
        string respone = request.downloadHandler.text;
        Debug.Log(respone);
        string orderId = JsonMapper.ToObject(respone)["orderId"].ToString();
        //SceneManager.LoadScene("main", LoadSceneMode.Single);
        List<Cart> carts = items.Values.ToList<Cart>();
        for(int i=0; i<carts.Count; i++)
        {
            string requestBody = "{" +
                " \"cart\" : \"" + carts[i].GetAssetId() + "\"," +
                " \"order\" : \"" + orderId +"\"" +
                "}";
            StartCoroutine(EditCart("https://treedp.doge.in.th/add/orderId", requestBody, i, carts.Count));
        }
    }

    IEnumerator EditCart(string url, string bodyJsonString, int  i, int count)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.Send();
        Loading.SetActive(false);
        string respone = request.downloadHandler.text;
        Debug.Log(respone);
        if(i >= count - 1)
        {
            Loading.SetActive(false);
            SceneManager.LoadScene("main", LoadSceneMode.Single);
        }

    }
}

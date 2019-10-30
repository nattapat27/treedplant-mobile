using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public GameObject Content;
    public GameObject TextPrice;
    public GameObject TextTotalPrice;
    public GameObject Loading;
    private JsonData data;
    private Dictionary<string, Cart> items;
    
    // Start is called before the first frame update
    void Start()
    {
        Loading.SetActive(false);
        Debug.Log(ConnectRestApi.getRespone());
        data = Helper.toJsonData(ConnectRestApi.getRespone());
        items = ModelGenerator.GetCart();
        int totalPrice = 0;
        List<Cart> carts = items.Values.ToList<Cart>();
        Debug.Log(carts);
        foreach (Cart cart in carts)
        {
            Debug.Log(cart.GetName());


            string objectCart = "Cart/"+cart.GetName();
            GameObject spawnedGameObject = Resources.Load(objectCart) as GameObject;
            int number = Convert.ToInt32(cart.GetNumber());
            GameObject Number = Helper.GetChildWithName(spawnedGameObject, "Number");
            Number.GetComponent<Text>().text = number.ToString();
            Debug.Log("index "+ cart.GetId());

            int price = Convert.ToInt32(data["data"][cart.GetId()]["price"].ToString());            
            GameObject Price = Helper.GetChildWithName(spawnedGameObject, "Price");
            Price.GetComponent<Text>().text = (price * number).ToString();
            Debug.Log("price");

            var obj = Instantiate(spawnedGameObject, new Vector3(0,0,0), Quaternion.identity);
            obj.transform.SetParent(Content.transform);
            totalPrice += price * number;
        }
        TextPrice.GetComponent<Text>().text = totalPrice.ToString();
        TextTotalPrice.GetComponent<Text>().text = (totalPrice + 100).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToDesign()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single);

    }

    public void GoToAddress()
    {
        List<Cart> carts = items.Values.ToList<Cart>();
        if(carts.Count != 0)
        {
            for (int i = 0; i < carts.Count; i++)
            {
                string requestBody = "{" +
                " \"amount\" : \"" + carts[i].GetNumber() + "\"," +
                " \"assetID\" : \"" + carts[i].GetAssetId() + "\"," +
                " \"profileId\" : \"" + SessionApp.userId + "\"" +
                "}";
                StartCoroutine(AddToCart("https://treedp.doge.in.th/cart/save", requestBody, i, carts.Count));
            }
        }
        //SceneManager.LoadScene("add address", LoadSceneMode.Additive);
    }

    IEnumerator AddToCart(string url, string bodyJsonString, int i, int count)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if(i == 0)
            Loading.SetActive(true);
        
        yield return request.Send();

        string respone = request.downloadHandler.text;
        Debug.Log(respone);
        SessionApp.cartId.Add(JsonMapper.ToObject(respone)["cartId"].ToString());
        
        if (i >= count - 1)
        {
            Loading.SetActive(false);
            SceneManager.LoadScene("add address", LoadSceneMode.Additive);
        }

    }
}

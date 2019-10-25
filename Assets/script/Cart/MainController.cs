using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public GameObject Content;
    public GameObject TextPrice;
    public GameObject TextTotalPrice;
    private JsonData data;
    private Dictionary<string, Cart> items;
    
    // Start is called before the first frame update
    void Start()
    {
        data = Helper.toJsonData(ConnectRestApi.getRespone());
        items = ModelGenerator.GetCart();
        int totalPrice = 0;
        List<Cart> carts = items.Values.ToList<Cart>();
        Debug.Log(carts);
        foreach (Cart cart in carts)
        {
            Debug.Log(cart);
            string objectCart = "Cart/"+cart.GetName();
            GameObject spawnedGameObject = Resources.Load(objectCart) as GameObject;
            int number = Convert.ToInt32(cart.GetNumber());
            Debug.Log(number);
            GameObject Number = Helper.GetChildWithName(spawnedGameObject, "Number");
            Number.GetComponent<Text>().text = number.ToString();
            int price = Convert.ToInt32(data["data"][Convert.ToInt32(cart.GetId().ToString())]["price"].ToString());
            GameObject Price = Helper.GetChildWithName(spawnedGameObject, "Price");
            Price.GetComponent<Text>().text = (price * number).ToString();
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
        SceneManager.LoadScene("add address", LoadSceneMode.Additive);
    }
}

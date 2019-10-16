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

    private JsonData data;
    private Dictionary<string, Cart> items;
    
    // Start is called before the first frame update
    void Start()
    {
        data = Helper.toJsonData(ConnectRestApi.getRespone());
        items = ModelGenerator.GetCart();
        // List<Cart> carts = items.Values.ToList<Cart>();
        List<Cart> carts = new List<Cart>();
        carts.Add(new Cart(0, "FlamingoFlower", 5));
        foreach (Cart cart in carts)
        {
            string objectCart = "Cart/"+cart.GetName();
            GameObject spawnedGameObject = Resources.Load(objectCart) as GameObject;

            var obj = Instantiate(spawnedGameObject, new Vector3(0,0,0), Quaternion.identity);
            obj.transform.SetParent(Content.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToDesign()
    {
        SceneManager.LoadScene("main");

    }

    public static void SetData(JsonData pass)
    {
        //data = pass;
    }
}

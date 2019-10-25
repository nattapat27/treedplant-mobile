using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddressController : MonoBehaviour
{
    public GameObject Name;
    public GameObject Phone;
    public GameObject Address;
    public GameObject Detail;
    public GameObject Subdistrict;
    public GameObject District;
    public GameObject Province;
    public GameObject Zipcode;
    // Start is called before the first frame update
    void Start()
    {
        if(SessionApp.user != null)
        {
            Name.GetComponent<Text>().text = SessionApp.user.Name;
            Phone.GetComponent<Text>().text = SessionApp.user.Phone;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddAddress()
    {
        string temp = "";
        temp += Address.GetComponent<Text>().text.ToString() + " ";
        temp += Detail.GetComponent<Text>().text.ToString() + " ";
        temp += Subdistrict.GetComponent<Text>().text.ToString();
        Address address = new Address();
        address.Zipcode = Zipcode.GetComponent<Text>().text.ToString();
        address.Province = Province.GetComponent<Text>().text.ToString();
        address.District = District.GetComponent<Text>().text.ToString();
        address.Detail = temp;
        SceneManager.LoadScene("summary product", LoadSceneMode.Single);
    }

}

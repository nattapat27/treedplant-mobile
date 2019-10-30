using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
    public GameObject Loading;
    // Start is called before the first frame update
    void Start()
    {
        Loading.SetActive(false);
        Debug.Log("Start");
        if(SessionApp.user != null)
        {
            Name.GetComponent<Text>().text = SessionApp.user.Name;
            Phone.GetComponent<Text>().text = SessionApp.user.Phone;
        }
        Debug.Log("Start");
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
        SessionApp.address.Zipcode = Zipcode.GetComponent<Text>().text.ToString();
        SessionApp.address.Province = Province.GetComponent<Text>().text.ToString();
        SessionApp.address.District = District.GetComponent<Text>().text.ToString();
        SessionApp.address.Detail = temp;
        Debug.Log("address");
        SessionApp.user.Name = Name.GetComponent<Text>().text.ToString();
        SessionApp.user.Phone = Phone.GetComponent<Text>().text.ToString();
        Debug.Log("user");
        GoNext();
    }
    private void GoNext()
    {
        if (SessionApp.address.Detail.Equals("") ||
            SessionApp.address.District.Equals("") ||
            SessionApp.address.Province.Equals("") ||
            SessionApp.address.Zipcode.Equals(""))
            return;
        string requestBody = "{" +
            " \"detail\" : \"" + SessionApp.address.Detail + "\","+ 
            " \"district\" : \"" + SessionApp.address.District + "\"," +
            " \"province\" : \"" + SessionApp.address.Province + "\"," +
            " \"zipcode\" : \"" + SessionApp.address.Zipcode + "\"," +
            " \"profileID\" : \"" + SessionApp.userId + "\"" +
            "}";
        StartCoroutine(PostAddress("https://treedp.doge.in.th/address/save", requestBody));
    }

    public void Back()
    {       
        SceneManager.UnloadScene("add address");
    }

    IEnumerator PostAddress(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Loading.SetActive(true);
        yield return request.Send();
        Loading.SetActive(false);
        string respones = request.downloadHandler.text;
        Debug.Log(respones);
        SessionApp.addressId = JsonMapper.ToObject(respones)["addressId"].ToString();
        SceneManager.LoadScene("summary product", LoadSceneMode.Additive);
    }
}
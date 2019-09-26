using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ModelListButton : MonoBehaviour
{
    public GameObject background;
    public GameObject name;
    public GameObject image;

    private GameObject modelList;
    private GameObject buttonList;
    private string id;

    public void Start()
    {
        //background.onClick.AddListener(DoSomeThing);
    }

    public void Setup(string name, string url, string id, GameObject modelList, GameObject buttonList)
    {
        Helper helper = new Helper();
        this.name = new GameObject("Text");
        var temp = this.name.AddComponent<Text>();
        temp.name = name;
        Debug.Log(url);
        this.image = new GameObject("Image");
        var temp1 = this.image.AddComponent<Image>();
        Debug.Log(temp1.sprite);
        //StartCoroutine(helper.LoadImage(url, temp1));
        StartCoroutine(helper.WaitNextFrame());
        Debug.Log("hello");
        this.id = id;
        this.modelList = modelList;
        this.buttonList = buttonList;

        /*
        Outline outline;
        Image bg;
        RectTransform rectTransform;
        rectTransform = background.GetComponentInChildren<RectTransform>();
        rectTransform.sizeDelta = new Vector2(350, 450);
        bg = background.GetComponentInChildren<Image>();
        bg.color = Color.white;
        outline = background.GetComponentInChildren<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance.Set(5, -5);
        */

    }

    public void DoSomeThing()
    {
        ModelGenerator.id = this.id;
        buttonList.SetActive(true);
        modelList.SetActive(false);
    }
}

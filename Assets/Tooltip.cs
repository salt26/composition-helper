using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string text;
    public Vector2 LD;
    public Vector2 RU;
    GameObject o;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        o = Instantiate(Manager.manager.tooltip, Manager.manager.GetCanvas().transform);
        o.GetComponentInChildren<Text>().text = text;
        o.GetComponent<RectTransform>().anchorMin = LD;
        o.GetComponent<RectTransform>().anchorMax = RU;
        o.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        Destroy(o);
    }
}

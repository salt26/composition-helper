using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LongPanel : MonoBehaviour {

    public Slider slider;
    public Button decideButton;
    public GameObject devPanel;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (slider.value < 2f && decideButton.interactable)
        {
            decideButton.interactable = false;
        }
        else if (slider.value >= 2f && !decideButton.interactable)
        {
            decideButton.interactable = true;
        }

        if (!Input.GetMouseButton(0))
        {
            slider.value = Mathf.Round(slider.value);
        }
	}

    public void Decide()
    {
        if (Manager.manager != null)
        {
            /*
            // TODO
            if (Mathf.Round(slider.value) > 2f)
            {
                devPanel.SetActive(true);
            }
            else
            {
                Manager.manager.SetMeasureAndChangeScene(slider.value);
            }
            */
            Manager.manager.SetMeasureAndChangeScene(slider.value);
        } else
        {
            Debug.LogError("Decide Error! Manager is null!");
        }
    }
}

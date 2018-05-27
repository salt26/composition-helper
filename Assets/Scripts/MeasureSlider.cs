using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeasureSlider : MonoBehaviour {

    public Slider slider;
    public Button decideButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (slider.value < 2 && decideButton.interactable)
        {
            decideButton.interactable = false;
        }
        else if (slider.value >= 2 && !decideButton.interactable)
        {
            decideButton.interactable = true;
        }
	}
}

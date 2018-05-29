using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piano : MonoBehaviour {

    public GameObject press;
    Image pressImage;

    public Sprite piano3, piano_1, piano_1R, piano_2, piano_2L, piano_2R, piano_3, piano_3L;

    static float[] xpos = { 0f, 0f, 45.5f, 45.5f, 45.5f, 91f, 91f, 131f, 131f, 176.5f, 176.5f, 176.5f, 222.2f, 222.2f, 222.2f, 267.9f, 267.9f };

	// Use this for initialization
	void Start () {
        pressImage = press.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
    }
 
    private void OnMouseDown()
    {
        float x = Input.mousePosition.x / Screen.width, y = Input.mousePosition.y / (Screen.height / 5f);
        if (y > 1) return;
        int key = (int)Mathf.Floor(x * 29);
        int tone = key / 7 * 17;
        switch (key % 7)
        {
            case 1:
                tone += 3;
                break;
            case 2:
                tone += 6;
                break;
            case 3:
                tone += 7;
                break;
            case 4:
                tone += 10;
                break;
            case 5:
                tone += 13;
                break;
            case 6:
                tone += 16;
                break;
        }
        if (y > 0.5f)
        {
            x = x * 29 - key;
            key %= 7;
            if (x < 0.4f && key != 0 && key != 3) tone--;
            if (x > 0.6f && key != 2 && key != 6) tone++;
        }
        switch (tone % 17)
        {
            case 0:
                if (tone == 68) pressImage.sprite = piano3;
                else pressImage.sprite = piano_1;
                break;
            case 1:
                pressImage.sprite = piano_1R;
                break;
            case 2:
                pressImage.sprite = piano_2L;
                break;
            case 3:
                pressImage.sprite = piano_2;
                break;
            case 4:
                pressImage.sprite = piano_2R;
                break;
            case 5:
                pressImage.sprite = piano_3L;
                break;
            case 6:
                pressImage.sprite = piano_3;
                break;
            case 7:
                pressImage.sprite = piano_1;
                break;
            case 8:
                pressImage.sprite = piano_1R;
                break;
            case 9:
                pressImage.sprite = piano_2L;
                break;
            case 10:
                pressImage.sprite = piano_2;
                break;
            case 11:
                pressImage.sprite = piano_2R;
                break;
            case 12:
                pressImage.sprite = piano_2L;
                break;
            case 13:
                pressImage.sprite = piano_2;
                break;
            case 14:
                pressImage.sprite = piano_2R;
                break;
            case 15:
                pressImage.sprite = piano_3L;
                break;
            case 16:
                pressImage.sprite = piano_3;
                break;
        }
        press.transform.position = new Vector2(tone / 17 * 308.9f + xpos[tone % 17], 0f);
        press.SetActive(true);
        Manager.manager.Play(Note.NoteToMidi(tone));
    }

    private void OnMouseUp()
    {
        press.SetActive(false);
    }
}

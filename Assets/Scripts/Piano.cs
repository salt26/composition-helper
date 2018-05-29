using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano : MonoBehaviour {

    public GameObject press;

	// Use this for initialization
	void Start () {
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
                press.transform.position = new Vector2(key / 7 * 308.9f, 0f);
                break;
            case 7:
                press.transform.position = new Vector2(key / 7 * 308.9f + 131f, 0f);
                break;
        }
        press.SetActive(true);
        Manager.manager.Play(Note.NoteToMidi(tone));
        Generator.GenerateNotes();
    }

    private void OnMouseUp()
    {
        press.SetActive(false);
    }
}

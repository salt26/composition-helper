using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano : MonoBehaviour {

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
            if (x < 0.4f) tone--;
            if (x > 0.6f) tone++;
        }
        Manager.manager.Play(Note.NoteToMidi(tone));
        Generator.GenerateNotes();
    }
}

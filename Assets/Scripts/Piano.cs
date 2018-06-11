using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piano : MonoBehaviour {

    public GameObject press;
    Image pressImage;

    public Sprite piano3, piano_1, piano_1R, piano_2, piano_2L, piano_2R, piano_3, piano_3L;

    static float[] xpos = { 0f, 0f, 45.5f, 45.5f, 45.5f, 91f, 91f, 131f, 131f, 176.5f, 176.5f, 176.5f, 222.2f, 222.2f, 222.2f, 267.7f, 267.7f };
    static Color whiteGray = new Color(0.6f, 0.6f, 0.6f, 1f);
    static Color blackGray = new Color(0.3f, 0.3f, 0.3f, 1f);

    List<GameObject> buttons = new List<GameObject>();
    public int mode = 0;   // 0: disable all, 1: bass clef, 2: treble clef, 3: enable all

	// Use this for initialization
	void Start () {
        pressImage = press.GetComponent<Image>();
        for (int tone = 0; tone <= 68; tone++)
        {
            GameObject p = Instantiate(press, GetComponent<Transform>());
            p.transform.position = new Vector2(tone / 17 * 308.9f + xpos[tone % 17], 0f);
            p.SetActive(true);
            Image pi = p.GetComponent<Image>();
            switch (tone % 17)
            {
                case 0:
                    if (tone == 68) pi.sprite = piano3;
                    else pi.sprite = piano_1;
                    break;
                case 1:
                    pi.sprite = piano_1R;
                    break;
                case 2:
                    pi.sprite = piano_2L;
                    break;
                case 3:
                    pi.sprite = piano_2;
                    break;
                case 4:
                    pi.sprite = piano_2R;
                    break;
                case 5:
                    pi.sprite = piano_3L;
                    break;
                case 6:
                    pi.sprite = piano_3;
                    break;
                case 7:
                    pi.sprite = piano_1;
                    break;
                case 8:
                    pi.sprite = piano_1R;
                    break;
                case 9:
                    pi.sprite = piano_2L;
                    break;
                case 10:
                    pi.sprite = piano_2;
                    break;
                case 11:
                    pi.sprite = piano_2R;
                    break;
                case 12:
                    pi.sprite = piano_2L;
                    break;
                case 13:
                    pi.sprite = piano_2;
                    break;
                case 14:
                    pi.sprite = piano_2R;
                    break;
                case 15:
                    pi.sprite = piano_3L;
                    break;
                case 16:
                    pi.sprite = piano_3;
                    break;
            }

            if (mode == 0 || (mode == 1 && tone > 40) || (mode == 2 && tone < 29))
            {
                if (IsBlackKey(tone)) pi.color = blackGray;
                else pi.color = whiteGray;
            }
            else
            {
                if (IsBlackKey(tone)) pi.color = Color.black;
                else pi.color = Color.white;
            }
            buttons.Add(p);
        }
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
        if (mode == 0 || (mode == 1 && tone > 40) || (mode == 2 && tone < 29))
        {
            return;
        }
        if (tone == 69) tone = 68;      // prevent IndexOutOfRangeException
        if (tone > 28)
        {
            Measure m = Manager.manager.GetStaff(0).GetMeasure(0);
            foreach (Note n in m.GetNotes())
            {
                if (n.GetIsRecommended())
                {
                    Manager.manager.WriteNote(0, 0, tone, Note.RhythmToName(n.GetRhythm()), n.GetTiming());
                    m.RemoveNote(n);
                    break;
                }
            }
        }
        /*
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
        */
        buttons[tone].GetComponent<Image>().color = new Color(0.8352f, 0.1686f, 0.1686f);
        Manager.manager.Play(Note.NoteToMidi(tone));
    }

    private void OnMouseUp()
    {
        press.SetActive(false);
        for (int tone = 0; tone <= 68; tone++)
        {
            if (mode == 0 || (mode == 1 && tone > 40) || (mode == 2 && tone < 29))
            {
                if (IsBlackKey(tone)) buttons[tone].GetComponent<Image>().color = blackGray;
                else buttons[tone].GetComponent<Image>().color = whiteGray;
            } else
            {
                if (IsBlackKey(tone)) buttons[tone].GetComponent<Image>().color = Color.black;
                else buttons[tone].GetComponent<Image>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// 검은 건반이면 true, 흰 건반이면 false를 반환합니다.
    /// </summary>
    /// <param name="tone"></param>
    /// <returns></returns>
    private bool IsBlackKey(int tone)
    {
        switch (tone % 17)
        {
            case 0:
            case 3:
            case 6:
            case 7:
            case 10:
            case 13:
            case 16:
                return false;
            default:
                return true;
        }
    }
}

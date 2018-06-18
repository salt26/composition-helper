using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piano : MonoBehaviour {

    public GameObject press;
    //Image pressImage;

    public Sprite piano3, piano_1, piano_1R, piano_2, piano_2L, piano_2R, piano_3, piano_3L;

    static float[] xpos = { 0f, 0f, 45.5f, 45.5f, 45.5f, 91f, 91f, 131f, 131f, 176.5f, 176.5f, 176.5f, 222.2f, 222.2f, 222.2f, 267.7f, 267.7f };
    static Color whiteGray = new Color(0.6f, 0.6f, 0.6f, 1f);
    static Color blackGray = new Color(0.3f, 0.3f, 0.3f, 1f);
    static Color highlighted = new Color(1f, 0.6899f, 0.2405f, 1f); // new Color(1f, 0.7607f, 0.5047f, 1f);
    static Color grayHighlight = new Color(0.755f, 0.5210f, 0.1806f, 1f); // new Color(0.56f, 0.3620f, 0.1512f, 1f);
    static Color whiteChord = new Color(0.9531f, 0.5058f, 1f, 1f);
    static Color blackChord = new Color(0.5178f, 0.1496f, 0.5566f, 1f);
    static Color clicked = new Color(0.8352f, 0.1686f, 0.1686f);
    static Color playing = new Color(0f, 0.839f, 0.5312847f, 1f);

    List<GameObject> buttons = new List<GameObject>();
    static List<bool> keyEnable = new List<bool>();     // [index]: tone, false: disable, true: enable
    static List<bool> keyChord = new List<bool>();      // [index]: tone, true: chord helper
    static List<bool> keyHighlight = new List<bool>();  // [index]: tone, true: highlight(selected)
    static List<bool> keyClick = new List<bool>();      // [index]: tone, true: clicked
    static List<int> keyPlaying = new List<int>();    // [index]: tone, value: playing count
    public int mode = 0;   // 0: disable all, 1: bass clef, 2: treble clef, 3: enable all

    static bool isFirstTime = true;

	// Use this for initialization
	void Start () {
        //pressImage = press.GetComponent<Image>();
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
            keyEnable.Add(true);
            keyChord.Add(false);
            keyHighlight.Add(false);
            keyClick.Add(false);
            keyPlaying.Add(0);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    void FixedUpdate()
    {
        object cur = Manager.manager.GetCursor();
        if (cur == null)
        {
            mode = 3;
        }
        else if (cur.GetType() == typeof(Note))
        {
            Note n = (Note)cur;
            if (n.GetComponentInParent<Staff>().staffName.Equals("Chord"))
            {
                mode = 0;
            }
            else if (n.GetComponentInParent<Staff>().staffName.Equals("Accompaniment"))
            {
                // TODO 반주를 입력 가능하게 하면 mode = 1로 바꾸기!
                mode = 1;
            }
            else // 멜로디 보표
            {
                mode = 2;
            }
        }
        else     // measure(마디) 선택 시
        {
            Measure m = (Measure)cur;
            if (m.GetComponentInParent<Staff>().staffName.Equals("Chord"))
            {
                mode = 0;
            }
            else if (m.GetComponentInParent<Staff>().staffName.Equals("Accompaniment"))
            {
                // TODO 반주를 입력 가능하게 하면 mode = 1로 바꾸기!
                mode = 1;
            }
            else // 멜로디 보표
            {
                bool b = false;
                foreach (Note n in m.GetNotes())
                {
                    if (n.GetIsRecommended())
                    {
                        b = true;
                        break;
                    }
                }
                if (b) mode = 2;
                else mode = 0;
            }
        }

        for (int tone = 0; tone <= 68; tone++)
        {
            if (keyClick[tone]) // click
            {
                buttons[tone].GetComponent<Image>().color = clicked;
            }
            else if (keyPlaying[tone] > 0) // playing
            {
                buttons[tone].GetComponent<Image>().color = playing;
            }
            else if (keyHighlight[tone]) // highlight
            {
                Debug.Log("HI");
                if (mode == 0 || (mode == 1 && tone > 40) || (mode == 2 && tone < 29) || !keyEnable[tone])
                    buttons[tone].GetComponent<Image>().color = grayHighlight;
                else buttons[tone].GetComponent<Image>().color = highlighted;
            }
            else if (mode == 0 || (mode == 1 && (tone > 40 || !keyChord[tone])) || (mode == 2 && tone < 29) || !keyEnable[tone]) // disable
            {
                if (IsBlackKey(tone)) buttons[tone].GetComponent<Image>().color = blackGray;
                else buttons[tone].GetComponent<Image>().color = whiteGray;
            }
            else if (keyChord[tone]) // chord helper
            {
                if (IsBlackKey(tone)) buttons[tone].GetComponent<Image>().color = blackChord;
                else buttons[tone].GetComponent<Image>().color = whiteChord;
            }
            else if (keyEnable[tone]) // enable
            {
                if (IsBlackKey(tone)) buttons[tone].GetComponent<Image>().color = Color.black;
                else buttons[tone].GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void OnMouseDown()
    {
        if (Manager.manager == null || Manager.manager.GetIsPlaying()
            || Finder.finder == null || Finder.finder.HasPopupOn()) return;
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
        if (mode == 0 || (mode == 1 && (tone > 40 || !keyChord[tone])) || (mode == 2 && tone < 29))
        {
            return;
        }
        if (tone == 69) tone = 68;      // prevent IndexOutOfRangeException
        if (mode == 2 && tone >= 29)
        {
            object cur = Manager.manager.GetCursor();
            if (cur != null && cur.GetType() == typeof(Note))
            {
                Note n = (Note)cur;
                if (n.GetIsTreble())
                {
                    Measure m = n.GetComponentInParent<Measure>();
                    Staff s = m.GetComponentInParent<Staff>();
                    if (s == Manager.manager.GetStaff(0))
                    {
                        m.RemoveNote(n);
                        Manager.manager.WriteNote(0, s.GetMeasureNum(m), tone, Note.RhythmToName(n.GetRhythm()), n.GetTiming());
                        Note nextcur = null, newnote = null;
                        foreach (Note note in m.GetNotes())
                        {
                            if (note.GetTiming() == n.GetTiming()) newnote = note;
                            if (note.GetTiming() > n.GetTiming() && (nextcur == null || note.GetTiming() < nextcur.GetTiming())) nextcur = note;
                        }
                        if (nextcur != null && nextcur.GetIsRecommended())
                            nextcur.Selected();
                        else if (nextcur == null && s.GetMeasureNum(m) + 1 < Manager.manager.GetMaxMeasureNum()
                            && Manager.manager.GetStaff(0).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes().Count > 0
                            && Manager.manager.GetStaff(0).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].GetIsRecommended())
                        {
                            // 다음 마디의 첫 음표를 확인해보자.
                            Manager.manager.GetStaff(0).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].Selected();
                        }
                        else
                        {
                            newnote.Selected();
                        }
                    }
                    Destroy(n);
                }
            }
            else if (cur != null) // Measure(마디) 선택 시
            {
                Measure m = (Measure)cur;
                Staff s = m.GetComponentInParent<Staff>();
                if (s == Manager.manager.GetStaff(0) && m.GetNotes().Count > 0 && m.GetNotes()[0].GetIsRecommended())
                {
                    Note n = m.GetNotes()[0];
                    m.RemoveNote(n);
                    Manager.manager.WriteNote(0, s.GetMeasureNum(m), tone, Note.RhythmToName(n.GetRhythm()), n.GetTiming());
                    Note nextcur = null, newnote = null;
                    foreach (Note note in m.GetNotes())
                    {
                        if (note.GetTiming() == n.GetTiming()) newnote = note;
                        if (note.GetTiming() > n.GetTiming() && (nextcur == null || note.GetTiming() < nextcur.GetTiming())) nextcur = note;
                    }
                    if (nextcur != null && nextcur.GetIsRecommended())
                        nextcur.Selected();
                    else if (nextcur == null && s.GetMeasureNum(m) + 1 < Manager.manager.GetMaxMeasureNum()
                        && Manager.manager.GetStaff(0).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes().Count > 0
                        && Manager.manager.GetStaff(0).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].GetIsRecommended())
                    {
                        // 다음 마디의 첫 음표를 확인해보자.
                        Manager.manager.GetStaff(0).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].Selected();
                    }
                    else
                    {
                        newnote.Selected();
                    }
                }
            }
        }
        else if (mode == 1 && keyChord[tone])
        {
            object cur = Manager.manager.GetCursor();
            if (cur != null && cur.GetType() == typeof(Note))
            {
                Note n = (Note)cur;
                if (!n.GetIsTreble())
                {
                    Measure m = n.GetComponentInParent<Measure>();
                    Staff s = m.GetComponentInParent<Staff>();
                    if (s == Manager.manager.GetStaff(1))
                    {
                        foreach (Note tn in m.GetNotes())
                        {
                            if (tn.GetTiming() == n.GetTiming()) m.RemoveNote(tn);
                        }
                        Manager.manager.WriteNote(1, s.GetMeasureNum(m), tone, Note.RhythmToName(n.GetRhythm()), n.GetTiming());
                        Note nextcur = null, newnote = null;
                        foreach (Note note in m.GetNotes())
                        {
                            if (note.GetTiming() == n.GetTiming()) newnote = note;
                            if (note.GetTiming() > n.GetTiming() && (nextcur == null || note.GetTiming() < nextcur.GetTiming())) nextcur = note;
                        }
                        if (nextcur != null && nextcur.GetIsRecommended())
                            nextcur.Selected();
                        else if (nextcur == null && s.GetMeasureNum(m) + 1 < Manager.manager.GetMaxMeasureNum()
                            && Manager.manager.GetStaff(1).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes().Count > 0
                            && Manager.manager.GetStaff(1).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].GetIsRecommended())
                        {
                            // 다음 마디의 첫 음표를 확인해보자.
                            Manager.manager.GetStaff(1).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].Selected();
                        }
                        else
                        {
                            newnote.Selected();
                        }
                    }
                    Destroy(n);
                    if (m.GetNotes().Count == 4)
                    {
                        m.HighlightOff();
                        Manager.manager.GetStaff(0).GetMeasure(Manager.manager.GetCursorMeasureNum()).InteractionOn();
                    }
                }
                if (isFirstTime)
                {
                    isFirstTime = false;
                    Finder.finder.instructionPanel2.SetActive(true);
                    Finder.finder.darkPanel.SetActive(true);
                }
            }
            else if (cur != null) // Measure(마디) 선택 시
            {
                Measure m = (Measure)cur;
                Staff s = m.GetComponentInParent<Staff>();
                if (s == Manager.manager.GetStaff(1) && m.GetNotes().Count > 0 && m.GetNotes()[0].GetIsRecommended())
                {
                    Note n = m.GetNotes()[0];
                    foreach (Note tn in m.GetNotes())
                    {
                        if (tn.GetTiming() == n.GetTiming()) m.RemoveNote(tn);
                    }
                    Manager.manager.WriteNote(1, s.GetMeasureNum(m), tone, Note.RhythmToName(n.GetRhythm()), n.GetTiming());
                    Note nextcur = null, newnote = null;
                    foreach (Note note in m.GetNotes())
                    {
                        if (note.GetTiming() == n.GetTiming()) newnote = note;
                        if (note.GetTiming() > n.GetTiming() && (nextcur == null || note.GetTiming() < nextcur.GetTiming())) nextcur = note;
                    }
                    if (nextcur != null && nextcur.GetIsRecommended())
                        nextcur.Selected();
                    else if (nextcur == null && s.GetMeasureNum(m) + 1 < Manager.manager.GetMaxMeasureNum()
                        && Manager.manager.GetStaff(1).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes().Count > 0
                        && Manager.manager.GetStaff(1).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].GetIsRecommended())
                    {
                        // 다음 마디의 첫 음표를 확인해보자.
                        Manager.manager.GetStaff(1).GetMeasure(s.GetMeasureNum(m) + 1).GetNotes()[0].Selected();
                    }
                    else
                    {
                        newnote.Selected();
                    }
                    if (m.GetNotes().Count == 4)
                    {
                        m.HighlightOff();
                        Manager.manager.GetStaff(0).GetMeasure(Manager.manager.GetCursorMeasureNum()).InteractionOn();
                    }
                }
                if (isFirstTime)
                {
                    isFirstTime = false;
                    Finder.finder.instructionPanel2.SetActive(true);
                    Finder.finder.darkPanel.SetActive(true);
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
        //buttons[tone].GetComponent<Image>().color = new Color(0.8352f, 0.1686f, 0.1686f);
        if (!keyClick[tone])
            keyClick[tone] = true;   // clicked state
        Manager.manager.PlayTone(Note.NoteToMidi(tone), 0);
    }

    private void OnMouseUp()
    {
        //press.SetActive(false);
        for (int tone = 0; tone <= 68; tone++)
        {
            if (keyClick[tone])
                keyClick[tone] = false;   // unclicked state
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

    /// <summary>
    /// tone 음을 내는 건반의 상태를 설정합니다.
    /// enable이 false이면 회색으로 누를 수 없게 되고, true이면 흰색 또는 검은색으로 누를 수 있게 됩니다.
    /// </summary>
    /// <param name="tone"></param>
    /// <param name="enable"></param>
    public static void SetKeyEnable(int tone, bool enable)
    {
        if (tone < 0 || tone > 68) return;
        keyEnable[tone] = enable;
    }

    /// <summary>
    /// tone 음을 내는 건반의 상태를 설정합니다.
    /// on이 true이면 보라색으로 화음에 맞는 음이라고 표시됩니다.
    /// </summary>
    /// <param name="tone"></param>
    /// <param name="on"></param>
    public static void SetKeyChord(int tone, bool on)
    {
        if (tone < 0 || tone > 68) return;
        keyChord[tone] = on;
    }

    /// <summary>
    /// tone 음을 내는 건반을 현재 재생 중인 상태로 표시합니다.
    /// on이 true이면 초록색으로 재생 중인 음이라고 표시됩니다.
    /// countVariation은 한꺼번에 같은 건반을 몇 번 켜거나 끌지 결정하며, 기본값은 1입니다.
    /// 같은 음을 여러 보표에서 동시에 연주할 수 있어서, 각 건반마다 켜진 개수로 관리합니다.
    /// </summary>
    /// <param name="tone"></param>
    /// <param name="on"></param>
    /// <param name="countVariation"></param>
    public static void SetKeyPlaying(int tone, bool on, int countVariation = 1)
    {
        if (tone < 0 || tone > 68) return;
        if (countVariation < 0) countVariation *= -1;
        if (on)
            keyPlaying[tone] += countVariation;
        else if (keyPlaying[tone] >= countVariation)
            keyPlaying[tone] -= countVariation;
        else
            keyPlaying[tone] = 0;
    }

    /// <summary>
    /// tone 음을 내는 건반의 상태를 설정합니다.
    /// on이 true이면 주황색으로 커서 위치의 음표의 음이라고 표시됩니다.
    /// 한 번에 최대 하나의 건반만 주황색으로 하이라이트됩니다.
    /// </summary>
    /// <param name="tone"></param>
    /// <param name="on"></param>
    public static void SetKeyHighlight(int tone, bool on)
    {
        //Debug.Log("SetKeyHighlight " + tone);
        if (tone < 0 || tone > 68) return;
        if (keyHighlight[tone] != on)
        {
            for (int i = 0; i <= 68; i++)
            {
                if (i == tone)
                    keyHighlight[tone] = on;
                else
                    keyHighlight[i] = false;
            }
        }
    }

    /// <summary>
    /// 모든 건반의 주황색 하이라이트를 해제합니다.
    /// </summary>
    public static void SetAllKeyHighlightOff()
    {
        for (int i = 0; i <= 68; i++)
        {
            keyHighlight[i] = false;
        }
    }

    /// <summary>
    /// 모든 건반의, 화음에 어울리는 음에 대한 보라색 표시를 해제합니다.
    /// </summary>
    public static void SetAllKeyChordOff()
    {
        for (int i = 0; i <= 68; i++)
        {
            keyChord[i] = false;
        }
    }
}

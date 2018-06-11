using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

    SpriteRenderer noteObject;
    Transform noteTransform;
    int pitch = -1;  // 음높이(우리의 음표 인코딩, 0 <= pitch <= 68)
    int rhythm = 0; // 음표 박자(한 마디를 16개로 쪼갰을 때의 길이, 0 < rhythm <= 16)
                    // 16: 온음표, 4: 4분음표, 1: 16분음표
    int timing = 0; // 음표가 등장하는 x좌표 위치(0 <= timing < 16)
    bool isTreble = true;
    bool isRecommended = false;
    static Color color = Color.black;
    static Color selectedColor = new Color(1f, 0.6899f, 0.2405f, 1f);

    void Awake()
    {
        noteObject = GetComponent<SpriteRenderer>();
        noteTransform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (Manager.manager != null && this.Equals(Manager.manager.GetCursor()))
        {
            SetColor(selectedColor);
            if (pitch >= 0)
            {
                Piano.SetKeyHighlight(pitch, true);
                //Debug.Log(pitch);
            }
            noteTransform.localPosition = new Vector3(noteTransform.localPosition.x, noteTransform.localPosition.y, -1f);
        }
        else if (Manager.manager != null && isRecommended)
        {
            SetColor(Manager.manager.recommendColor);
            noteTransform.localPosition = new Vector3(noteTransform.localPosition.x, noteTransform.localPosition.y, 0f);
        }
        else
        {
            SetColor(color = Color.black);
            noteTransform.localPosition = new Vector3(noteTransform.localPosition.x, noteTransform.localPosition.y, 0f);
        }
    }

    void OnMouseDown()
    {
        Selected();
    }

    public void Selected()
    {
        Manager.manager.SetCursor(this);
    }

    public void Initialize(bool isTreble, int pitch, string rhythm, int timing)
    {
        SetIsTreble(isTreble);
        SetPitch(pitch);
        SetRhythm(rhythm);
        SetTiming(timing);
    }

    public void Initialize(bool isTreble, int pitch, string rhythm, int timing, Color color)
    {
        SetIsTreble(isTreble);
        SetPitch(pitch);
        SetRhythm(rhythm);
        SetTiming(timing);
        SetColor(color);
    }

    public void Initialize(bool isTreble, int pitch, string rhythm, int timing, Color color, bool isRecommended)
    {
        SetIsTreble(isTreble);
        SetPitch(pitch);
        SetRhythm(rhythm);
        SetTiming(timing);
        SetColor(color);
        this.isRecommended = isRecommended;
        Debug.Log(isRecommended);
    }

    /// <summary>
    /// 높은음자리표의 음표인지 낮은음자리표의 음표인지 설정합니다.
    /// 높은음자리표이면 true를 인자로 넣습니다.
    /// </summary>
    /// <param name="t"></param>
    public void SetIsTreble(bool t)
    {
        isTreble = t;
    }

    /// <summary>
    /// 우리의 음표 인코딩에 따라 이 음표의 음높이를 설정합니다.
    /// 음높이가 너무 높거나 낮으면 추가 선을 그려줍니다.
    /// 반드시 이 메서드보다 SetIsTreble을 먼저 호출해야 합니다.
    /// </summary>
    /// <param name="p"></param>
    public void SetPitch(int p)
    {
        if (p < 0 || p > 68) pitch = -1;
        else pitch = p;

        noteTransform.localPosition = new Vector3(noteTransform.localPosition.x, NoteToScore(pitch, isTreble));

        /* Additional Line */
        GameObject temp1, temp2;
        switch ((int)(NoteToScore(pitch, isTreble) * 8f))
        {
            case 8:
                temp1 = Instantiate(Manager.manager.additionalLineObject, noteTransform);
                temp2 = Instantiate(Manager.manager.additionalLineObject, noteTransform);
                temp2.GetComponent<Transform>().localPosition = new Vector3(0f, -1f, 0f);
                break;
            case 7:
                temp2 = Instantiate(Manager.manager.additionalLineObject, noteTransform);
                temp2.GetComponent<Transform>().localPosition = new Vector3(0f, -0.5f, 0f);
                break;
            case 6:
            case -6:
                temp1 = Instantiate(Manager.manager.additionalLineObject, noteTransform);
                break;
            case -7:
                temp2 = Instantiate(Manager.manager.additionalLineObject, noteTransform);
                temp2.GetComponent<Transform>().localPosition = new Vector3(0f, 0.5f, 0f);
                break;
            case -8:
                temp1 = Instantiate(Manager.manager.additionalLineObject, noteTransform);
                temp2 = Instantiate(Manager.manager.additionalLineObject, noteTransform);
                temp2.GetComponent<Transform>().localPosition = new Vector3(0f, 1f, 0f);
                break;
        }

        /* Accidental */
        switch (NoteToAccidental(pitch))
        {
            case 1:
                temp1 = Instantiate(Manager.manager.accidentalObject, noteTransform);
                temp1.GetComponent<SpriteRenderer>().sprite =
                    Resources.Load("sharp", typeof(Sprite)) as Sprite;
                break;
            case 2:
                temp1 = Instantiate(Manager.manager.accidentalObject, noteTransform);
                temp1.GetComponent<SpriteRenderer>().sprite =
                    Resources.Load("flat", typeof(Sprite)) as Sprite;
                break;
            default:
                /*
                temp1 = Instantiate(Manager.manager.accidentalObject, noteTransform);
                temp1.GetComponent<SpriteRenderer>().sprite =
                    Resources.Load("natural", typeof(Sprite)) as Sprite;
                */
                break;
        }
    }

    /// <summary>
    /// "온음표", "점2분음표", "2분음표", "점4분음표", "4분음표", "점8분음표", "8분음표", "16분음표"
    /// 중 하나를 입력받아 이 음표의 박자를 설정합니다.
    /// 이 메서드를 호출하기 전에 반드시 SetIsTreble과 SetPitch를 먼저 호출해야 합니다.
    /// </summary>
    /// <param name="r"></param>
    public void SetRhythm(string r)
    {
        bool isTailDown = true;
        if (pitch == -1)
        {
            rhythm = 0;
            return;
        }
        else if (pitch != -1 && NoteToScore(pitch, isTreble) < 0f) isTailDown = false;
        switch (r)
        {
            case "온음표":
                rhythm = 16;
                noteObject.sprite = Resources.Load("Note1", typeof(Sprite)) as Sprite;
                break;
            case "점2분음표":
                rhythm = 12;
                if (!isTailDown)
                {
                    noteObject.sprite = Resources.Load("Note2", typeof(Sprite)) as Sprite;
                    GameObject dot = Instantiate(Manager.manager.dotObject, noteTransform);
                    dot.GetComponent<Transform>().localPosition =
                        DotPosition((int)(NoteToScore(pitch, isTreble) * 8f) % 2 == 1);
                }
                else
                {
                    noteObject.sprite = Resources.Load("Note2_", typeof(Sprite)) as Sprite;
                    GameObject dot = Instantiate(Manager.manager.dotObject, noteTransform);
                    dot.GetComponent<Transform>().localPosition =
                        DotPosition((int)(NoteToScore(pitch, isTreble) * 8f) % 2 == 1);
                }
                break;
            case "2분음표":
                rhythm = 8;
                if (!isTailDown)
                    noteObject.sprite = Resources.Load("Note2", typeof(Sprite)) as Sprite;
                else
                    noteObject.sprite = Resources.Load("Note2_", typeof(Sprite)) as Sprite;
                break;
            case "점4분음표":
                rhythm = 6;
                if (!isTailDown)
                {
                    noteObject.sprite = Resources.Load("Note4", typeof(Sprite)) as Sprite;
                    GameObject dot = Instantiate(Manager.manager.dotObject, noteTransform);
                    dot.GetComponent<Transform>().localPosition =
                        DotPosition((int)(NoteToScore(pitch, isTreble) * 8f) % 2 == 1);
                }
                else
                {
                    noteObject.sprite = Resources.Load("Note4_", typeof(Sprite)) as Sprite;
                    GameObject dot = Instantiate(Manager.manager.dotObject, noteTransform);
                    dot.GetComponent<Transform>().localPosition =
                        DotPosition((int)(NoteToScore(pitch, isTreble) * 8f) % 2 == 1);
                }
                break;
            case "4분음표":
                rhythm = 4;
                if (!isTailDown)
                    noteObject.sprite = Resources.Load("Note4", typeof(Sprite)) as Sprite;
                else
                    noteObject.sprite = Resources.Load("Note4_", typeof(Sprite)) as Sprite;
                break;
            case "점8분음표":
                rhythm = 3;
                if (!isTailDown)
                {
                    noteObject.sprite = Resources.Load("Note8", typeof(Sprite)) as Sprite;
                    GameObject dot = Instantiate(Manager.manager.dotObject, noteTransform);
                    dot.GetComponent<Transform>().localPosition =
                        DotPosition((int)(NoteToScore(pitch, isTreble) * 8f) % 2 == 1, true);
                }
                else
                {
                    noteObject.sprite = Resources.Load("Note8_", typeof(Sprite)) as Sprite;
                    GameObject dot = Instantiate(Manager.manager.dotObject, noteTransform);
                    dot.GetComponent<Transform>().localPosition =
                        DotPosition((int)(NoteToScore(pitch, isTreble) * 8f) % 2 == 1);
                }
                break;
            case "8분음표":
                rhythm = 2;
                if (!isTailDown)
                    noteObject.sprite = Resources.Load("Note8", typeof(Sprite)) as Sprite;
                else
                    noteObject.sprite = Resources.Load("Note8_", typeof(Sprite)) as Sprite;
                break;
            case "16분음표":
                rhythm = 1;
                if (!isTailDown)
                    noteObject.sprite = Resources.Load("Note16", typeof(Sprite)) as Sprite;
                else
                    noteObject.sprite = Resources.Load("Note16_", typeof(Sprite)) as Sprite;
                break;
            default:
                rhythm = 0;
                break;
        }
    }

    /// <summary>
    /// 음표가 시간적으로(악보의 x좌표상으로) 언제 나오는지 설정하는 메서드입니다.
    /// </summary>
    /// <param name="t"></param>
    public void SetTiming(int t)
    {
        if (t < 0 || t >= 16)
        {
            t = -1;
            return;
        }
        timing = t;
        noteTransform.localPosition = new Vector3(-5.16f + (timing * 0.66f), noteTransform.localPosition.y);

    }

    /// <summary>
    /// 음표의 색을 설정하는 함수입니다.
    /// </summary>
    /// <param name="c"></param>
    public void SetColor(Color c)
    {
        color = c;
        noteObject.color = color;
        foreach (Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().color = c;
        }
    }

    public void SetIsRecommended(bool r)
    {
        isRecommended = r;
    }

    public bool GetIsRecommended()
    {
        return isRecommended;
    }

    public bool GetIsTreble()
    {
        return isTreble;
    }

    public int GetRhythm()
    {
        return rhythm;
    }

    public int GetTiming()
    {
        return timing;
    }

    public int GetPitch()
    {
        return pitch;
    }

    /// <summary>
    /// 음표에 점이 찍힐 좌표를 결정하는 함수입니다.
    /// </summary>
    /// <param name="isKan"></param>
    /// <param name="isTailUpEighth"></param>
    /// <returns></returns>
    Vector3 DotPosition(bool isKan, bool isTailUpEighth = false)
    {
        if (isTailUpEighth && !isKan)
        {
            return new Vector3(1.7f, 0.5f);
        }
        else if (isKan)
        {
            return new Vector3(1f, 0f);
        }
        else
        {
            return new Vector3(1f, 0.5f);
        }
    }

    /// <summary>
    /// 우리의 음표 인코딩에서 미디 음 번호로 바꿔줍니다.
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    public static int NoteToMidi(int note)
    {
        int a = (note / 17) * 12;
        int b = note % 17;
        switch (b)
        {
            case 0: return a + 36;
            case 1:
            case 2: return a + 37;
            case 3: return a + 38;
            case 4:
            case 5: return a + 39;
            case 6: return a + 40;
            case 7: return a + 41;
            case 8:
            case 9: return a + 42;
            case 10: return a + 43;
            case 11:
            case 12: return a + 44;
            case 13: return a + 45;
            case 14:
            case 15: return a + 46;
            default: return a + 47;
        }
    }

    /// <summary>
    /// 우리의 음표 인코딩에서 계이름으로 바꿔줍니다.
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    public static string NoteToName(int note)
    {
        int b = note % 17;
        switch (b)
        {
            case 0: return "도";
            case 1: return "도#";
            case 2: return "레b";
            case 3: return "레";
            case 4: return "레#";
            case 5: return "미b";
            case 6: return "미";
            case 7: return "파";
            case 8: return "파#";
            case 9: return "솔b";
            case 10: return "솔";
            case 11: return "솔#";
            case 12: return "라b";
            case 13: return "라";
            case 14: return "라#";
            case 15: return "시b";
            default: return "시";
        }
    }

    /// <summary>
    /// 우리의 음표 인코딩에서 악보에 넣을 y좌표로 바꿔줍니다.
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    public static float NoteToScore(int note, bool isTreble)
    {
        float y = 0f;
        if (note < 0 || note > 68)
        {
            Debug.LogWarning("NoteToScore Warning!");
            return -100f;
        }
        else if ((isTreble && note < 29) || (!isTreble && note > 40))
        {
            Debug.LogWarning("NoteToScore Warning!");
            return -200f;
        }
        if (isTreble) y = -1.5f;
        switch (note)
        {
            case 0:
            case 1: return -1f;     // 도
            case 2:
            case 3:
            case 4: return -0.875f; // 레
            case 5:
            case 6: return -0.75f;  // 미
            case 7:
            case 8: return -0.625f; // 파
            case 9:
            case 10:
            case 11: return -0.5f;  // 솔
            case 12:
            case 13:
            case 14: return -0.375f; // 라
            case 15:
            case 16: return -0.25f; // 시
            case 17:
            case 18: return -0.125f; // 도
            case 19:
            case 20:
            case 21: return 0f;
            case 22:
            case 23: return 0.125f;
            case 24:
            case 25: return 0.25f;
            case 26:
            case 27:
            case 28: return 0.375f;
            case 29:
            case 30:
            case 31: return y + 0.5f;   // 라
            case 32:
            case 33: return y + 0.625f;
            case 34:
            case 35: return y + 0.75f;
            case 36:
            case 37:
            case 38: return y + 0.875f;
            case 39:
            case 40: return y + 1f;     // 미
            case 41:
            case 42: return -0.375f;
            case 43:
            case 44:
            case 45: return -0.25f;
            case 46:
            case 47:
            case 48: return -0.125f;
            case 49:
            case 50: return 0f;     // 시
            case 51:
            case 52: return 0.125f;
            case 53:
            case 54:
            case 55: return 0.25f;
            case 56:
            case 57: return 0.375f;
            case 58:
            case 59: return 0.5f;
            case 60:
            case 61:
            case 62: return 0.625f;
            case 63:
            case 64:
            case 65: return 0.75f;
            case 66:
            case 67: return 0.875f; // 시
            case 68: return 1f;     // 도
            default: return -2f;
        }
    }

    /// <summary>
    /// 우리의 음표 인코딩으로부터 어떤 임시표가 붙는지 알려줍니다.
    /// 0은 내추럴, 1은 샵(#), 2는 플랫(b)이다.
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    public static int NoteToAccidental(int note)
    {
        int a = note % 17;
        switch (a)
        {
            case 1:
            case 4:
            case 8:
            case 11:
            case 14: return 1;
            case 2:
            case 5:
            case 9:
            case 12:
            case 15: return 2;
            default: return 0;
        }
    }

    /// <summary>
    /// 미디 음 번호에서 우리의 음표 인코딩으로 변환합니다.
    /// </summary>
    /// <param name="midi"></param>
    /// <returns></returns>
    public static int MidiToNote(int midi)
    {
        int a = ((midi / 12) - 3) * 17;
        int b = midi % 12;
        switch (b)
        {
            case 0:
                return a;
            case 1:
                return a + 1;
            case 2:
                return a + 3;
            case 3:
                return a + 4;
            case 4:
                return a + 6;
            case 5:
                return a + 7;
            case 6:
                return a + 8;
            case 7:
                return a + 10;
            case 8:
                return a + 11;
            case 9:
                return a + 13;
            case 10:
                return a + 14;
            default:
                return a + 16;
        }
    }

    public static string RhythmToName(int rhythm)
    {
        switch (rhythm)
        {
            case 1:
                return "16분음표";
            case 2:
                return "8분음표";
            case 3:
                return "점8분음표";
            case 4:
                return "4분음표";
            case 6:
                return "점4분음표";
            case 8:
                return "2분음표";
            case 12:
                return "점2분음표";
            case 16:
                return "온음표";
            default:
                return "";
        }
    }
}

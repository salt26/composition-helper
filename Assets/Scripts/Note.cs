using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

    SpriteRenderer noteObject;
    Transform noteTransform;
    int pitch = -1;  // 음높이(우리의 음표 인코딩, 0 <= pitch <= 68)
    int rhythm = 0; // 음표 박자(한 마디를 16개로 쪼갰을 때의 길이, 0 < rhythm <= 16)
    bool isTreble = true;

    void Awake()
    {
        noteObject = GetComponent<SpriteRenderer>();
        noteTransform = GetComponent<Transform>();
    }
    
    void FixedUpdate () {
        if (pitch == -1 || rhythm == 0) return;

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
	}

    /// <summary>
    /// 우리의 음표 인코딩에서 미디 음표 번호로 바꿔줍니다.
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
            return -2f;
        }
        else if ((isTreble && note < 29) || (!isTreble && note > 40))
        {
            Debug.LogWarning("NoteToScore Warning!");
            return 2f;
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
}

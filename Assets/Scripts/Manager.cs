using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Sanford.Multimedia.Midi;

public class Manager : MonoBehaviour {

    public static Manager manager;
    
    public GameObject measureNumCanvas;
    public GameObject noteObject;
    public GameObject additionalLineObject;
    public GameObject accidentalObject;
    public GameObject dotObject;

    /*
     * staffs[0] : MelodyStaff
     * staffs[1] : AccompanimentStaff
     * staffs[2] : ChordStaff
     */
    List<Staff> staffs = new List<Staff>();
    List<Chord> tempChords = new List<Chord>(); // TODO This variable is only for demo.
    GameObject melodyPanel;
    GameObject mainCamera;
    Scrollbar scrollbar;
    bool isScoreScene;

    bool isChordDriven;
    int measureNum = 0;

    OutputDevice outDevice = new OutputDevice(0);

    // Use this for initialization
    void Awake () {
        manager = this;
        DontDestroyOnLoad(this);
        isScoreScene = false;
        for (int i = 0; i < 3; i++)
        {
            staffs.Add(null);
        }
    }

    void FixedUpdate()
    {
        GameObject g;

        if ((melodyPanel == null || !melodyPanel.name.Equals("MelodyPanel")) && SceneManager.GetActiveScene().name.Equals("Start"))
        {
            melodyPanel = GameObject.Find("MelodyPanel");
        }

        if ((staffs[0] == null || !staffs[0].name.Equals("MelodyStaff")) && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            staffs[0] = GameObject.Find("MelodyStaff").GetComponent<Staff>();
        }
        if ((staffs[1] == null || !staffs[1].name.Equals("AccompanimentStaff")) && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            staffs[1] = GameObject.Find("AccompanimentStaff").GetComponent<Staff>();
        }
        if ((staffs[2] == null || !staffs[2].name.Equals("ChordStaff")) && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            staffs[2] = GameObject.Find("ChordStaff").GetComponent<Staff>();
        }
        if (scrollbar == null && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            scrollbar = GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
        }
        if ((mainCamera == null || !mainCamera.name.Equals("Main Camera")) && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            mainCamera = GameObject.Find("Main Camera");
        }

        if (!isScoreScene && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            isScoreScene = true;
            foreach (Staff s in staffs)
            {
                if (s == null)
                {
                    Debug.LogError("staffs are null!");
                    continue;
                }
                s.CreateMeasure(measureNum);
            }
            if (scrollbar != null)
            {
                scrollbar.value = 0f;
                if (measureNum <= 1) scrollbar.size = 1f;
                else scrollbar.size = 1f / ((measureNum - 1) * 11f - 5f);
            }
            for (int i = 0; i < measureNum; i++)
            {
                g = Instantiate(measureNumCanvas, new Vector3(-1.45f + (11f * i), 4.7f, 0f), Quaternion.identity);
                g.GetComponentInChildren<Text>().text = (i + 1).ToString();
            }
            RecommendChords(null);
        }

        if (scrollbar != null && mainCamera != null)
        {
            mainCamera.GetComponent<Transform>().SetPositionAndRotation(
                new Vector3((scrollbar.value * ((measureNum - 1) * 11f - 5f)), 0f, -10f), Quaternion.identity);
        }
    }

    /// <summary>
    /// isChord가 true이면 화음 우선 작곡 모드로, false이면 멜로디 우선 작곡 모드로 설정합니다.
    /// 그리고 마디 길이를 묻는 창으로 넘어갑니다.
    /// </summary>
    /// <param name="isChord"></param>
    public void SetDriven(bool isChord)
    {
        isChordDriven = isChord;
        melodyPanel.SetActive(false);
    } 
	
    /// <summary>
    /// 슬라이더 값에 따라 마디 수를 설정하고 악보 화면으로 씬을 전환합니다.
    /// </summary>
    /// <param name="sliderVal"></param>
    public void SetMeasureAndChangeScene(float sliderVal)
    {
        float i = Mathf.Round(sliderVal);
        if (i >= 2f)
        {
            measureNum = 8 * (int)i;
            SceneManager.LoadScene("Score");
        }
        // TODO i == 5f일 때 마디 수 자유
        else
        {
            Debug.LogError("SetMeasure Error!");
        }
    }
    
    public void RecommendChords(Chord prevChord)
    {
        if (prevChord == null)
        {
            tempChords.Clear();
            
            // This is only for demo!
            tempChords.Add(new Chord(Note.MidiToNote(49)+1, Note.MidiToNote(53), Note.MidiToNote(56)+1));
            tempChords.Add(new Chord(Note.MidiToNote(54), Note.MidiToNote(58), Note.MidiToNote(61)));
            tempChords.Add(new Chord(Note.MidiToNote(47), Note.MidiToNote(50), Note.MidiToNote(54)));
            tempChords.Add(new Chord(Note.MidiToNote(51), Note.MidiToNote(54), Note.MidiToNote(57)));
            tempChords.Add(new Chord(Note.MidiToNote(47), Note.MidiToNote(49), Note.MidiToNote(54)));
            tempChords.Add(new Chord(Note.MidiToNote(51)+1, Note.MidiToNote(55), Note.MidiToNote(58)+1, Note.MidiToNote(61)+1));
        }
        else
        {
            tempChords.Clear();

            // TODO
        }
    }

    public Chord GetTempChord(int i)
    {
        if (i >= 0 && i < tempChords.Count)
        {
            return tempChords[i];
        }
        else return null;
    }

    /// <summary>
    /// 특정된 종류의 보표(Staff)를 반환합니다.
    /// kind가 0이면 멜로디, 1이면 반주, 2이면 화음 보표를 반환합니다.
    /// 어느 것도 아니면 null이 반환됩니다.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public Staff GetStaff(int kind)
    {
        if (kind >= 0 && kind < 3)
        {
            return staffs[kind];
        }
        else return null;
    }

    /// <summary>
    /// 화음을 연주합니다.
    /// </summary>
    /// <param name="tone"></param>
    public void Play(int tone)
    {
        if (tone >= 0 && tone < 128)
        {
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, tone, 127));
        }
    }

    /// <summary>
    /// 화음을 연주합니다.
    /// </summary>
    /// <param name="fundamentalTone"></param>
    /// <param name="second"></param>
    /// <param name="third"></param>
    /// <param name="fourth"></param>
    public void PlayChord(int fundamentalTone, int second, int third, int fourth = -1)
    {
        for (int i = 3; i <= 6; i++)
        {
            Play(fundamentalTone + i * 12);
            Play(fundamentalTone + second + i * 12);
            Play(fundamentalTone + third + i * 12);
            if (fourth != -1) Play(fundamentalTone + fourth + i * 12);
        }
    }

    /// <summary>
    /// 화음을 연주합니다.
    /// </summary>
    /// <param name="chord"></param>
    public void PlayChord(string chord)
    {
        int fundamentalTone = 0;
        int second = 4, third = 7, fourth = -1;
        foreach (char c in chord)
        {
            switch (c)
            {
                case 'A':
                    fundamentalTone = 9;
                    break;
                case 'B':
                    fundamentalTone = 11;
                    break;
                case 'C':
                    break;
                case 'D':
                    fundamentalTone = 2;
                    break;
                case 'E':
                    fundamentalTone = 4;
                    break;
                case 'F':
                    fundamentalTone = 5;
                    break;
                case 'G':
                    fundamentalTone = 7;
                    break;
                case '#':
                    fundamentalTone++;
                    break;
                case 'b':
                    fundamentalTone--;
                    break;
                case 'd':
                    third = 6;
                    break;
                case 'm':
                    second = 3;
                    break;
                case '2':
                    second = 2;
                    break;
                case '4':
                    second = 5;
                    break;
                case '6':
                    fourth = 9;
                    break;
                case '7':
                    fourth = 10;
                    break;
            }
        }
        PlayChord(fundamentalTone, second, third, fourth);
    }

    /// <summary>
    /// 프로그램을 종료합니다.
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else 
		Application.Quit();
#endif
    }
}

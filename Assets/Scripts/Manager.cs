﻿using System.Collections;
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
    Button chordRecommendButton;
    object cursor;
    bool isScoreScene;

    bool isChordDriven;
    int measureNum = 0;

    OutputDevice outDevice;

    // Use this for initialization
    void Awake ()
    {
        outDevice = new OutputDevice(0);
        manager = this;
        DontDestroyOnLoad(this);
        isScoreScene = false;
        for (int i = 0; i < 3; i++)
        {
            staffs.Add(null);
        }
        cursor = null;
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
        if (chordRecommendButton == null && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            chordRecommendButton = GameObject.Find("ChordRecommendButton").GetComponent<Button>();
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
            staffs[0].InteractionAllOff();
            staffs[1].InteractionAllOff();
            staffs[2].InteractionAllOff();
            staffs[2].GetMeasure(0).InteractionOn();
            staffs[2].GetMeasure(0).HighlightOn();
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

    public void SetCursor(object thing)
    {
        cursor = thing;
    }

    public Camera GetMainCamera()
    {
        return mainCamera.GetComponent<Camera>();
    }

    public Button GetChordRecommendButton()
    {
        return chordRecommendButton;
    }

    public void RecommendChords(Chord prevChord)
    {
        if (prevChord == null)
        {
            tempChords.Clear();

            // This is only for demo!
            tempChords.Add(new Chord(Note.MidiToNote(49) + 1, Note.MidiToNote(53), Note.MidiToNote(56) + 1));
            tempChords.Add(new Chord(Note.MidiToNote(54), Note.MidiToNote(58), Note.MidiToNote(61)));
            tempChords.Add(new Chord(Note.MidiToNote(47), Note.MidiToNote(50), Note.MidiToNote(54)));
            tempChords.Add(new Chord(Note.MidiToNote(51), Note.MidiToNote(54), Note.MidiToNote(57)));
            tempChords.Add(new Chord(Note.MidiToNote(47), Note.MidiToNote(49)+1, Note.MidiToNote(54)+1));
            tempChords.Add(new Chord(Note.MidiToNote(51) + 1, Note.MidiToNote(55), Note.MidiToNote(58) + 1, Note.MidiToNote(61) + 1));
        }
        else
        {
            tempChords.Clear();

            // TODO
        }
    }

    public void RecommendRhythm()
    {
        List<int> rhythms = Generator.GenerateNotes();
        // TODO 생성된 리듬에 따라 해당 마디에 박자 만들고 악보에 보여주기
        GetStaff(0).GetMeasure(0).ClearMeasure();
        int sum = 0;
        foreach (int rhythm in rhythms)
        {
            Manager.manager.WriteNote(0, 0, 51,
                rhythm == 1 ? "16분음표" :
                rhythm == 2 ? "8분음표" :
                rhythm == 3 ? "점8분음표" :
                rhythm == 4 ? "4분음표" :
                rhythm == 6 ? "점4분음표" :
                rhythm == 8 ? "2분음표" :
                rhythm == 12 ? "점2분음표" : "온음표", sum, new Color(0, 0, 0, 0.3f));
            sum += rhythm;
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
    /// 음을 연주합니다.
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
    /// 특정 악보의 원하는 위치에 원하는 음표를 하나 그리는 메서드입니다.
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="measure"></param>
    /// <param name="pitch"></param>
    /// <param name="rhythm"></param>
    /// <param name="timing"></param>
    public void WriteNote(int staff, int measure, int pitch, string rhythm, int timing)
    {
        Staff st = GetStaff(staff);
        if (st == null) return;
        st.WriteNote(measure, pitch, rhythm, timing);
    }

    public void WriteNote(int staff, int measure, int pitch, string rhythm, int timing, Color color)
    {
        Staff st = GetStaff(staff);
        if (st == null) return;
        st.WriteNote(measure, pitch, rhythm, timing, color);
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

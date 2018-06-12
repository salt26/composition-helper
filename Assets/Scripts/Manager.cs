﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Sanford.Multimedia.Midi;

public class Manager : MonoBehaviour
{

    public static Manager manager;

    public GameObject InstructionPanel;
    public GameObject tooltip;
    public GameObject measureNumCanvas;
    public GameObject noteObject;
    public GameObject additionalLineObject;
    public GameObject accidentalObject;
    public GameObject dotObject;
    public Color recommendColor = new Color(0f, 0f, 0f, 0.3f);

    /*
     * staffs[0] : MelodyStaff
     * staffs[1] : AccompanimentStaff
     * staffs[2] : ChordStaff
     */
    List<Staff> staffs = new List<Staff>();
    List<Chord> tempChords = new List<Chord>(); // TODO This variable is only for demo.
    GameObject melodyPanel;
    GameObject mainCamera;
    GameObject canvas;
    Scrollbar scrollbar;
    Button chordRecommendButton;
    Button rhythmRecommendButton;
    object cursor;
    int cursorMeasureNum;
    bool isScoreScene;
    bool isChordDriven;
    int measureNum = 0;
    bool isFirstTime = true;

    OutputDevice outDevice;
    IEnumerator play;

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
        cursorMeasureNum = -1;
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
        if ((canvas == null || !canvas.name.Equals("Canvas")) && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            canvas = GameObject.Find("Canvas");
        }   
        if (rhythmRecommendButton == null && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            rhythmRecommendButton = GameObject.Find("RhythmRecommendButton").GetComponent<Button>();
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
            //staffs[2].GetMeasure(0).HighlightOn();
        }

        if (scrollbar != null && mainCamera != null)
        {
            //Debug.Log(Input.mouseScrollDelta); // (0, 0), (0, -1), (0, -2), (0, 1), (0, 2)
            scrollbar.value -= Input.mouseScrollDelta.y / (measureNum * 2 + 1);
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

    public void SetCursor(object thing, int measureNum)
    {
        cursor = thing;
        cursorMeasureNum = measureNum;
    }

    /// <summary>
    /// 커서가 아무것도 가리키지 않게 합니다.
    /// </summary>
    public void SetCursorToNull()
    {
        if (!manager.GetStaff(0).chordPanel.activeInHierarchy &&
                                !manager.GetStaff(0).developingPanel.activeInHierarchy)
        {
            Piano.SetAllKeyHighlightOff();
            Piano.SetAllKeyChordOff();
            manager.GetChordRecommendButton().GetComponent<Highlighter>().HighlightOff();
            manager.GetRhythmRecommendButton().GetComponent<Highlighter>().HighlightOff();
            manager.GetChordRecommendButton().interactable = false;
            manager.GetRhythmRecommendButton().interactable = false;
            cursor = null;
            cursorMeasureNum = -1;
        }
    }

    public Camera GetMainCamera()
    {
        return mainCamera.GetComponent<Camera>();
    }

    public Button GetChordRecommendButton()
    {
        return chordRecommendButton;
    }

    public Button GetRhythmRecommendButton()
    {
        return rhythmRecommendButton;
    }

    public object GetCursor()
    {
        return cursor;
    }

    public int GetCursorMeasureNum()
    {
        return cursorMeasureNum;
    }

    public int GetMaxMeasureNum()
    {
        return measureNum;
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
        int mn = manager.GetCursorMeasureNum();
        if (mn < 0) return;
        List<int> rhythms = Generator.GenerateNotes();
        // TODO 생성된 리듬에 따라 해당 마디에 박자 만들고 악보에 보여주기
        manager.GetStaff(0).GetMeasure(mn).ClearMeasure();
        int sum = 0;
        foreach (int rhythm in rhythms)
        {
            manager.WriteNote(0, mn, 51,
                rhythm == 1 ? "16분음표" :
                rhythm == 2 ? "8분음표" :
                rhythm == 3 ? "점8분음표" :
                rhythm == 4 ? "4분음표" :
                rhythm == 6 ? "점4분음표" :
                rhythm == 8 ? "2분음표" :
                rhythm == 12 ? "점2분음표" : "온음표", sum, recommendColor, true);
            sum += rhythm;
        }
        if (manager.isFirstTime)
        {
            manager.isFirstTime = false;
            Instantiate(manager.InstructionPanel, manager.canvas.transform);
        }
    }

    public Chord GetTempChord(int i)
    {
        if (i >= 0 && i < manager.tempChords.Count)
        {
            return manager.tempChords[i];
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

    public GameObject GetCanvas()
    {
        return canvas;
    }

    /// <summary>
    /// 음을 연주합니다.
    /// </summary>
    /// <param name="tone"></param>
    public void Play(int tone, int staff)
    {
        if (tone >= 0 && tone < 128)
        {
            manager.outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, staff, tone, 127));
        }
    }

    /// <summary>
    /// 음을 멈춥니다.
    /// </summary>
    /// <param name="tone"></param>
    public void Stop(int tone, int staff)
    {
        if (tone >= 0 && tone < 128)
        {
            manager.outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, staff, tone, 127));
        }
    }

    List<KeyValuePair<float, int>> ToMidi()
    {
        List<KeyValuePair<float, int>> list = new List<KeyValuePair<float, int>>();
        for (int i = 0; i < 3; i++)
        {
            foreach (KeyValuePair<float, int> p in manager.staffs[i].ToMidi())
            {
                list.Add(new KeyValuePair<float, int>(p.Key, p.Value < 0 ? -(-p.Value | i << 16) : p.Value | i << 16));
            }
        }
        list.Sort(delegate (KeyValuePair<float, int> p1, KeyValuePair<float, int> p2)
        {
            return p1.Key < p2.Key ? -1 : p1.Key > p2.Key ? 1 : p1.Value < p2.Value ? -1 : p1.Value > p2.Value ? 1 : 0;
        });
        return list;
    }

    IEnumerator __PlayAll(List<KeyValuePair<float, int>> list)
    {
        float last = 0;
        foreach (KeyValuePair<float, int> p in list)
        {
            if (last != p.Key)
            {
                yield return new WaitForSecondsRealtime((p.Key - last) / 2);
                last = p.Key;
            }
            if (p.Value > 0) Play(p.Value & 65535, p.Value >> 16);
            else Stop(-p.Value & 65535, -p.Value >> 16);
        }
    }

    public void PlayAll()
    {
        List<KeyValuePair<float, int>> list = ToMidi();
        StopAll();
        play = __PlayAll(list);
        manager.StartCoroutine(play);
    }

    public void StopAll()
    {
        int i, j;
        if (play != null) manager.StopCoroutine(play);
        for (i = 0; i < 128; i++) for (j = 0; j < 3; j++) Stop(i, j);
    }

    public void SaveAll()
    {
        List<KeyValuePair<float, int>> list = ToMidi();
        Sequence seq = new Sequence();
        Track tr = new Track();
        float last = 0;
        foreach (KeyValuePair<float, int> p in list)
        {
            Debug.Log((int)(p.Key * 28 + .5) + " " + p.Value);
            tr.Insert((int)(p.Key * 28 + .5), new ChannelMessage(p.Value > 0 ? ChannelCommand.NoteOn : ChannelCommand.NoteOff, p.Value < 0 ? -p.Value >> 16 : p.Value >> 16, p.Value < 0 ? -p.Value & 65535 : p.Value & 65535, 127));
            last = p.Key;
        }
        seq.Add(tr);
        seq.Save("Awesome.midi");
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
        Staff st = manager.GetStaff(staff);
        if (st == null) return;
        st.WriteNote(measure, pitch, rhythm, timing);
    }

    public void WriteNote(int staff, int measure, int pitch, string rhythm, int timing, Color color)
    {
        Staff st = manager.GetStaff(staff);
        if (st == null) return;
        st.WriteNote(measure, pitch, rhythm, timing, color);
    }

    public void WriteNote(int staff, int measure, int pitch, string rhythm, int timing, Color color, bool isRecommended)
    {
        Staff st = manager.GetStaff(staff);
        if (st == null) return;
        st.WriteNote(measure, pitch, rhythm, timing, color, isRecommended);
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

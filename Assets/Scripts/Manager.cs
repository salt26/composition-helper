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
    GameObject backgroundCollider;
    Scrollbar scrollbar;
    Button chordRecommendButton;
    Button rhythmRecommendButton;
    Button saveButton;
    object cursor;
    int cursorMeasureNum;
    bool isScoreScene;
    bool isChordDriven;
    bool isPlaying;
    int measureNum = 0;
    static bool isFirstTime = true;

    OutputDevice outDevice;
    IEnumerator play;

    // Use this for initialization
    void Awake ()
    {
        outDevice = new OutputDevice(0);
        manager = this;
        DontDestroyOnLoad(this);
        isScoreScene = false;
        isPlaying = false;
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
        if (backgroundCollider == null && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            backgroundCollider = ScoreBackground.sb.gameObject;
        }
        if (saveButton == null && SceneManager.GetActiveScene().name.Equals("Score"))
        {
            saveButton = SaveButton.sb.GetComponent<Button>();
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
            /*
           if (manager.GetCursorMeasureNum() > 0)
            {
                List<int> tempNotes = new List<int>();
                foreach (Note n in manager.GetStaff(2).GetMeasure(manager.GetCursorMeasureNum() - 1).GetNotes())
                {
                    tempNotes.Add(n.GetPitch());
                }
                RecommendChords(new Chord(tempNotes));
            }
            */
            staffs[0].InteractionAllOff();
            staffs[1].InteractionAllOff();
            staffs[2].InteractionAllOff();
            staffs[2].GetMeasure(0).InteractionOn();
            //staffs[2].GetMeasure(0).HighlightOn();
        }

        if (scrollbar != null && mainCamera != null)
        {
            //Debug.Log(Input.mouseScrollDelta); // (0, 0), (0, -1), (0, -2), (0, 1), (0, 2)
            //scrollbar.value -= Input.mouseScrollDelta.y / (measureNum * 2 + 1);
            mainCamera.GetComponent<Transform>().SetPositionAndRotation(
                new Vector3((scrollbar.value * ((measureNum - 1) * 11f - 5f)), 0f, -10f), Quaternion.identity);
            backgroundCollider.GetComponent<Transform>().SetPositionAndRotation(
                mainCamera.GetComponent<Transform>().position + new Vector3(0.7f, 0.9f, 15f), Quaternion.identity);
        }

        if (isScoreScene && (GetCursor() == null || !IsThereAnyNote(GetCursorMeasureNum())) && Finder.finder.playButton.GetComponent<Button>().interactable)
        {
            //Debug.LogWarning("playButton inactive");
            Finder.finder.playButton.GetComponent<Button>().interactable = false;
        }
        else if (isScoreScene && GetCursor() != null && IsThereAnyNote(GetCursorMeasureNum()) && !Finder.finder.playButton.GetComponent<Button>().interactable)
        {
            //Debug.LogWarning("playButton active");
            Finder.finder.playButton.GetComponent<Button>().interactable = true;
        }

        if (isScoreScene && GetCursorMeasureNum() < 1 && Finder.finder.playPrevChordButton.GetComponent<Button>().interactable)
        {
            Finder.finder.playPrevChordButton.GetComponent<Button>().interactable = false;
        }
        else if (isScoreScene && GetCursorMeasureNum() >= 1 && !Finder.finder.playPrevChordButton.GetComponent<Button>().interactable)
        {
            Finder.finder.playPrevChordButton.GetComponent<Button>().interactable = true;
        }

        if (isScoreScene && saveButton.interactable && !IsThereAnyNote())
        {
            //Debug.LogWarning("saveButton inactive");
            saveButton.interactable = false;
        }
        else if (isScoreScene && !saveButton.interactable && IsThereAnyNote())/*GetStaff(2).GetMeasure(0).GetNotes().Count > 0 
            && (GetStaff(0).GetHasPlay() || GetStaff(1).GetHasPlay() || GetStaff(2).GetHasPlay()))*/
        {
            //Debug.LogWarning("saveButton active");
            saveButton.interactable = true;
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
            measureNum = 8 * (int)(i - 1);
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
        if (!Finder.finder.HasPopupOn() && manager != null && !manager.GetIsPlaying())
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

    /// <summary>
    /// 이전 마디의 화음을 보고 현재 마디의 화음을 추천합니다.
    /// (코드가 아주 깁니다.)
    /// </summary>
    /// <param name="prevChord"></param>
    public void RecommendChords(Chord prevChord)
    {
        int midiBass = -1;
        if (prevChord == null)
        {
            manager.tempChords.Clear();

            for (int i = 0; i < 6; i++)
            {
                Chord ch = Generator.GenerateChord();
                // 첫 화음으로는 Major, minor, Major7, minor7만 온다.
                while (ch.GetChordName().Equals("suspension4")
                    || ch.GetChordName().Equals("augmented") || ch.GetChordName().Equals("diminished"))
                    ch = Generator.GenerateChord();
                bool b = false;
                for (int j = 0; j < i; j++)
                {
                    if (ch.GetNotes()[0] == tempChords[j].GetNotes()[0]
                        && ch.GetNotes()[1] == tempChords[j].GetNotes()[1]
                        && ch.GetNotes()[2] == tempChords[j].GetNotes()[2])
                    {
                        b = true;
                        break;
                    }
                }
                if (b)
                {
                    i--;
                    continue;
                }
                Debug.Log("Manager RecommendChords " + ch.GetBass());
                tempChords.Add(ch);
            }
        }
        else if (prevChord.GetChordName().Equals("Major"))
        {
            Debug.Log("Major Recommended");
            manager.tempChords.Clear();
            midiBass = Note.NoteToMidi(prevChord.GetBass());
            Debug.Log(midiBass);
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 7),
                Note.MidiToNote(midiBass + 11), Note.MidiToNote(midiBass + 14)), false));   // 5 Major
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 9), Note.MidiToNote(midiBass + 12)), false));   // 4 Major
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 2),
                Note.MidiToNote(midiBass + 6), Note.MidiToNote(midiBass + 9),
                Note.MidiToNote(midiBass + 12)), false));                                   // 2 Major7
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 3), Note.MidiToNote(midiBass + 7)), false));   // 1 minor
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 4), Note.MidiToNote(midiBass + 8)), false));   // 1 aug

            manager.tempChords[0].SetChordName("Major");
            manager.tempChords[1].SetChordName("Major");
            manager.tempChords[2].SetChordName("Major7");
            manager.tempChords[3].SetChordName("minor");
            manager.tempChords[4].SetChordName("augmented");

            bool b;
            Chord ch;
            do
            {
                ch = Generator.GenerateChord();
                b = false;
                for (int j = 0; j < 5; j++)
                {
                    if (ch.GetBass() == tempChords[j].GetBass()
                        && ch.GetChordName() == tempChords[j].GetChordName())
                    {
                        b = true;
                        break;
                    }
                }
            } while (b);
            manager.tempChords.Add(ch); // random

            manager.tempChords[0].SetChordText(Note.NoteToName2(tempChords[0].GetBass()) + "\n밝음·긴장");
            manager.tempChords[1].SetChordText(Note.NoteToName2(tempChords[1].GetBass()) + "\n밝음·편안");
            manager.tempChords[2].SetChordText(Note.NoteToName2(tempChords[2].GetBass()) + "7\n긴장·변화");
            manager.tempChords[3].SetChordText(Note.NoteToName2(tempChords[3].GetBass()) + "m\n어두움");
            manager.tempChords[4].SetChordText(Note.NoteToName2(tempChords[4].GetBass()) + "aug\n불안");

            /*
            manager.tempChords.Add(new Chord(Note.MidiToNote(49) + 1, Note.MidiToNote(53), Note.MidiToNote(56) + 1));
            manager.tempChords.Add(new Chord(Note.MidiToNote(54), Note.MidiToNote(58), Note.MidiToNote(61)));
            manager.tempChords.Add(new Chord(Note.MidiToNote(47), Note.MidiToNote(50), Note.MidiToNote(54)));
            manager.tempChords.Add(new Chord(Note.MidiToNote(51), Note.MidiToNote(54), Note.MidiToNote(57)));
            manager.tempChords.Add(new Chord(Note.MidiToNote(47), Note.MidiToNote(49) + 1, Note.MidiToNote(54) + 1));
            manager.tempChords.Add(new Chord(Note.MidiToNote(51) + 1, Note.MidiToNote(55), Note.MidiToNote(58) + 1, Note.MidiToNote(61) + 1));
            manager.tempChords[0].SetChordText("기호 1번");
            manager.tempChords[1].SetChordText("기호 2번");
            manager.tempChords[2].SetChordText("기호 3번");
            manager.tempChords[3].SetChordText("기호 4번");
            manager.tempChords[4].SetChordText("기호 5번");
            manager.tempChords[5].SetChordText("기호 6번");
            manager.tempChords[0].SetChordText("레b 파 라b\n(Db)");
            manager.tempChords[1].SetChordText("파# 라# 도#\n(F#)");
            manager.tempChords[2].SetChordText("시 레 파#\n(Bm)");
            manager.tempChords[3].SetChordText("레# 파# 라\n(D#dim)");
            manager.tempChords[4].SetChordText("시 레b 솔b\n(Bsus2)");
            manager.tempChords[5].SetChordText("미b 솔 시b 레b\n(Eb7)");
            */
        }
        else if (prevChord.GetChordName().Equals("minor"))
        {
            //Debug.Log("Major Recommended");
            manager.tempChords.Clear();
            midiBass = Note.NoteToMidi(prevChord.GetBass());
            Debug.Log(midiBass);
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 7),
                Note.MidiToNote(midiBass + 10), Note.MidiToNote(midiBass + 14)), false));   // 5 minor
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 8), Note.MidiToNote(midiBass + 12)), false));   // 4 minor
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 3),
                Note.MidiToNote(midiBass + 7), Note.MidiToNote(midiBass + 10)), false));                                   // 2 Major7
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 4), Note.MidiToNote(midiBass + 7)), false));   // 1 Major
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 3), Note.MidiToNote(midiBass + 6)), false));   // 1 dim

            manager.tempChords[0].SetChordName("minor");
            manager.tempChords[1].SetChordName("minor");
            manager.tempChords[2].SetChordName("Major");
            manager.tempChords[3].SetChordName("Major");
            manager.tempChords[4].SetChordName("diminished");

            bool b;
            Chord ch;
            do
            {
                ch = Generator.GenerateChord();
                b = false;
                for (int j = 0; j < 5; j++)
                {
                    if (ch.GetBass() == tempChords[j].GetBass()
                        && ch.GetChordName() == tempChords[j].GetChordName())
                    {
                        b = true;
                        break;
                    }
                }
            } while (b);
            manager.tempChords.Add(ch); // random

            manager.tempChords[0].SetChordText(Note.NoteToName2(tempChords[0].GetBass()) + "m\n어두움·긴장");
            manager.tempChords[1].SetChordText(Note.NoteToName2(tempChords[1].GetBass()) + "m\n어두움·편안");
            manager.tempChords[2].SetChordText(Note.NoteToName2(tempChords[2].GetBass()) + "\n밝음·비슷함");
            manager.tempChords[3].SetChordText(Note.NoteToName2(tempChords[3].GetBass()) + "\n밝음");
            manager.tempChords[4].SetChordText(Note.NoteToName2(tempChords[4].GetBass()) + "dim\n불안");
        }
        else if (prevChord.GetChordName().Equals("diminished"))
        {
            //Debug.Log("Major Recommended");
            manager.tempChords.Clear();
            midiBass = Note.NoteToMidi(prevChord.GetBass());
            Debug.Log(midiBass);
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 4), Note.MidiToNote(midiBass + 7)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass - 2),
                Note.MidiToNote(midiBass + 2), Note.MidiToNote(midiBass + 5)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 3), Note.MidiToNote(midiBass + 7),
                Note.MidiToNote(midiBass + 10)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass - 2),
                Note.MidiToNote(midiBass + 2), Note.MidiToNote(midiBass + 6)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 3),
                Note.MidiToNote(midiBass + 6), Note.MidiToNote(midiBass + 9)), false));

            manager.tempChords[0].SetChordName("Major");
            manager.tempChords[1].SetChordName("Major");
            manager.tempChords[2].SetChordName("Major7");
            manager.tempChords[3].SetChordName("augmented");
            manager.tempChords[4].SetChordName("diminished");

            bool b;
            Chord ch;
            do
            {
                ch = Generator.GenerateChord();
                b = false;
                for (int j = 0; j < 5; j++)
                {
                    if (ch.GetBass() == tempChords[j].GetBass()
                        && ch.GetChordName() == tempChords[j].GetChordName())
                    {
                        b = true;
                        break;
                    }
                }
            } while (b);
            manager.tempChords.Add(ch); // random

            manager.tempChords[0].SetChordText(Note.NoteToName2(tempChords[0].GetBass()) + "\n진행");
            manager.tempChords[1].SetChordText(Note.NoteToName2(tempChords[1].GetBass()) + "\n편안·해결");
            manager.tempChords[2].SetChordText(Note.NoteToName2(tempChords[2].GetBass()) + "m7\n어두움·긴장");
            manager.tempChords[3].SetChordText(Note.NoteToName2(tempChords[3].GetBass()) + "aug\n여전히 불안");
            manager.tempChords[4].SetChordText(Note.NoteToName2(tempChords[4].GetBass()) + "dim\n비슷한 불안");
        }
        else if (prevChord.GetChordName().Equals("augmented"))
        {
            //Debug.Log("Major Recommended");
            manager.tempChords.Clear();
            midiBass = Note.NoteToMidi(prevChord.GetBass());
            Debug.Log(midiBass);
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 9), Note.MidiToNote(midiBass + 12)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 8),
                Note.MidiToNote(midiBass + 12), Note.MidiToNote(midiBass + 15)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 1),
                Note.MidiToNote(midiBass + 4), Note.MidiToNote(midiBass + 8)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass - 3),
                Note.MidiToNote(midiBass), Note.MidiToNote(midiBass + 4)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 4),
                Note.MidiToNote(midiBass + 8), Note.MidiToNote(midiBass + 11),
                Note.MidiToNote(midiBass + 14)), false));

            manager.tempChords[0].SetChordName("Major");
            manager.tempChords[1].SetChordName("Major");
            manager.tempChords[2].SetChordName("minor");
            manager.tempChords[3].SetChordName("minor");
            manager.tempChords[4].SetChordName("Major7");

            bool b;
            Chord ch;
            do
            {
                ch = Generator.GenerateChord();
                b = false;
                for (int j = 0; j < 5; j++)
                {
                    if (ch.GetBass() == tempChords[j].GetBass()
                        && ch.GetChordName() == tempChords[j].GetChordName())
                    {
                        b = true;
                        break;
                    }
                }
            } while (b);
            manager.tempChords.Add(ch); // random

            manager.tempChords[0].SetChordText(Note.NoteToName2(tempChords[0].GetBass()) + "\n밝음·편안");
            manager.tempChords[1].SetChordText(Note.NoteToName2(tempChords[1].GetBass()) + "\n밝음·변화");
            manager.tempChords[2].SetChordText(Note.NoteToName2(tempChords[2].GetBass()) + "m\n어두움");
            manager.tempChords[3].SetChordText(Note.NoteToName2(tempChords[3].GetBass()) + "m\n어두움·변화");
            manager.tempChords[4].SetChordText(Note.NoteToName2(tempChords[4].GetBass()) + "7\n밝음·긴장");
        }
        else if (prevChord.GetChordName().Equals("suspension4"))
        {
            //Debug.Log("Major Recommended");
            manager.tempChords.Clear();
            midiBass = Note.NoteToMidi(prevChord.GetBass());
            Debug.Log(midiBass);
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 4), Note.MidiToNote(midiBass + 7)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 9), Note.MidiToNote(midiBass + 12)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass - 5),
                Note.MidiToNote(midiBass - 1), Note.MidiToNote(midiBass + 2),
                Note.MidiToNote(midiBass + 5)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass - 5),
                Note.MidiToNote(midiBass - 2), Note.MidiToNote(midiBass + 2),
                Note.MidiToNote(midiBass + 5)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 2), Note.MidiToNote(midiBass + 7)), false));

            manager.tempChords[0].SetChordName("Major");
            manager.tempChords[1].SetChordName("Major");
            manager.tempChords[2].SetChordName("Major7");
            manager.tempChords[3].SetChordName("minor7");
            manager.tempChords[4].SetChordName("suspension4");

            bool b;
            Chord ch;
            do
            {
                ch = Generator.GenerateChord();
                b = false;
                for (int j = 0; j < 5; j++)
                {
                    if (ch.GetBass() == tempChords[j].GetBass()
                        && ch.GetChordName() == tempChords[j].GetChordName())
                    {
                        b = true;
                        break;
                    }
                }
            } while (b);
            manager.tempChords.Add(ch); // random

            manager.tempChords[0].SetChordText(Note.NoteToName2(tempChords[0].GetBass()) + "\n밝음·해결");
            manager.tempChords[1].SetChordText(Note.NoteToName2(tempChords[1].GetBass()) + "\n밝음·해결");
            manager.tempChords[2].SetChordText(Note.NoteToName2(tempChords[2].GetBass()) + "7\n밝음·긴장");
            manager.tempChords[3].SetChordText(Note.NoteToName2(tempChords[3].GetBass()) + "m7\n어두움·긴장");
            manager.tempChords[4].SetChordText(Note.NoteToName2(tempChords[4].GetBass()) + "sus4\n여전히 걸림");
        }
        else if (prevChord.GetChordName().Equals("Major7"))
        {
            //Debug.Log("Major Recommended");
            manager.tempChords.Clear();
            midiBass = Note.NoteToMidi(prevChord.GetBass());
            Debug.Log(midiBass);
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 7),
                Note.MidiToNote(midiBass + 10), Note.MidiToNote(midiBass + 14)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 9), Note.MidiToNote(midiBass + 12)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass - 2),
                Note.MidiToNote(midiBass + 2), Note.MidiToNote(midiBass + 5)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 10), Note.MidiToNote(midiBass + 12)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 4), Note.MidiToNote(midiBass + 8)), false));

            manager.tempChords[0].SetChordName("minor");
            manager.tempChords[1].SetChordName("Major");
            manager.tempChords[2].SetChordName("Major");
            manager.tempChords[3].SetChordName("suspension4");
            manager.tempChords[4].SetChordName("augmented");

            bool b;
            Chord ch;
            do
            {
                ch = Generator.GenerateChord();
                b = false;
                for (int j = 0; j < 5; j++)
                {
                    if (ch.GetBass() == tempChords[j].GetBass()
                        && ch.GetChordName() == tempChords[j].GetChordName())
                    {
                        b = true;
                        break;
                    }
                }
            } while (b);
            manager.tempChords.Add(ch); // random

            manager.tempChords[0].SetChordText(Note.NoteToName2(tempChords[0].GetBass()) + "m\n어두움·긴장");
            manager.tempChords[1].SetChordText(Note.NoteToName2(tempChords[1].GetBass()) + "\n밝음·편안");
            manager.tempChords[2].SetChordText(Note.NoteToName2(tempChords[2].GetBass()) + "\n밝음·변화");
            manager.tempChords[3].SetChordText(Note.NoteToName2(tempChords[3].GetBass()) + "sus4\n걸림·긴장");
            manager.tempChords[4].SetChordText(Note.NoteToName2(tempChords[4].GetBass()) + "aug\n불안");
        }
        else if (prevChord.GetChordName().Equals("minor7"))
        {
            //Debug.Log("Major Recommended");
            manager.tempChords.Clear();
            midiBass = Note.NoteToMidi(prevChord.GetBass());
            Debug.Log(midiBass);
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 7),
                Note.MidiToNote(midiBass + 10), Note.MidiToNote(midiBass + 14)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 8), Note.MidiToNote(midiBass + 12)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 4), Note.MidiToNote(midiBass + 7),
                Note.MidiToNote(midiBass + 10)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass + 5),
                Note.MidiToNote(midiBass + 10), Note.MidiToNote(midiBass + 12)), false));
            manager.tempChords.Add(Chord.ReviseScoreNotation(new Chord(Note.MidiToNote(midiBass),
                Note.MidiToNote(midiBass + 3), Note.MidiToNote(midiBass + 6)), false));

            manager.tempChords[0].SetChordName("minor");
            manager.tempChords[1].SetChordName("minor");
            manager.tempChords[2].SetChordName("Major7");
            manager.tempChords[3].SetChordName("suspension4");
            manager.tempChords[4].SetChordName("diminished");

            bool b;
            Chord ch;
            do
            {
                ch = Generator.GenerateChord();
                b = false;
                for (int j = 0; j < 5; j++)
                {
                    if (ch.GetBass() == tempChords[j].GetBass()
                        && ch.GetChordName() == tempChords[j].GetChordName())
                    {
                        b = true;
                        break;
                    }
                }
            } while (b);
            manager.tempChords.Add(ch); // random

            manager.tempChords[0].SetChordText(Note.NoteToName2(tempChords[0].GetBass()) + "m\n어두움·긴장");
            manager.tempChords[1].SetChordText(Note.NoteToName2(tempChords[1].GetBass()) + "m\n어두움·편안");
            manager.tempChords[2].SetChordText(Note.NoteToName2(tempChords[2].GetBass()) + "7\n긴장");
            manager.tempChords[3].SetChordText(Note.NoteToName2(tempChords[3].GetBass()) + "sus4\n걸림·긴장");
            manager.tempChords[4].SetChordText(Note.NoteToName2(tempChords[4].GetBass()) + "dim\n불안");
        }
        else
        {
            manager.tempChords.Clear();

            for (int i = 0; i < 6; i++)
            {
                Chord ch = Generator.GenerateChord();
                bool b = false;
                for (int j = 0; j < i; j++)
                {
                    if (ch.GetNotes()[0] == tempChords[j].GetNotes()[0]
                        && ch.GetNotes()[1] == tempChords[j].GetNotes()[1]
                        && ch.GetNotes()[2] == tempChords[j].GetNotes()[2])
                    {
                        b = true;
                        break;
                    }
                }
                if (b)
                {
                    i--;
                    continue;
                }
                tempChords.Add(ch);
            }
        }
    }

    public void CaveatRhythm()
    {
        bool b = false;
        if (manager.GetCursorMeasureNum() < 0) return;
        foreach (Note n in manager.GetStaff(0).GetMeasure(manager.GetCursorMeasureNum()).GetNotes())
        {
            if (!n.GetIsRecommended())
            {
                b = true;
                break;
            }
        }
        if (b)
        {
            Finder.finder.rhythmCaveatPanel.SetActive(true);
            Finder.finder.darkPanel.SetActive(true);
        }
        else
        {
            RecommendRhythm();
        }
    }

    public void RecommendRhythm()
    {
        int mn = manager.GetCursorMeasureNum();
        if (mn < 0) return;
        manager.SetCursor(manager.GetStaff(0).GetMeasure(mn), mn);
        List<int> rhythms = Generator.GenerateNotes();
        // 생성된 리듬에 따라 해당 마디에 박자 만들고 악보에 보여주기
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
        if (isFirstTime)
        {
            isFirstTime = false;
            Finder.finder.instructionPanel.SetActive(true);
            Finder.finder.darkPanel.SetActive(true);
        }
    }

    public Chord GetTempChord(int i)
    {
        if (i >= 0 && i < manager.tempChords.Count)
        {
            //Debug.Log(manager.tempChords[i].GetChordText());
            return manager.tempChords[i];
        }
        else
        {
            Debug.LogError("Why?");
            return null;
        }
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

    public bool GetIsPlaying()
    {
        return isPlaying;
    }

    /// <summary>
    /// 음을 연주합니다.
    /// </summary>
    /// <param name="tone"></param>
    public void PlayTone(int tone, int staff)
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

    List<KeyValuePair<float, int>> ToMidiAll()
    {
        List<KeyValuePair<float, int>> list = new List<KeyValuePair<float, int>>();
        for (int i = 0; i < 3; i++)
        {
            foreach (KeyValuePair<float, int> p in manager.staffs[i].ToMidiAll())
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
        float last = 0, timingf;
        int mn, timing;
        manager.isPlaying = true;
        //Debug.LogWarning("Playing...");
        foreach (KeyValuePair<float, int> p in list)
        {
            if (last != p.Key)
            {
                yield return new WaitForSecondsRealtime((p.Key - last) / 2);
                last = p.Key;
            }
            if ((mn = manager.GetCursorMeasureNum()) != -1)
                mn += (int)(p.Key) / 4;
            else
                mn = (int)(p.Key) / 4;
            if (p.Value > 0)
            {
                timing = (int)(p.Key * 4f) % 16;
                PlayTone(p.Value & 65535, p.Value >> 16);
                /* TODO */
                SetNotesPlaying(mn, timing, true);
                Piano.SetKeyPlaying(Note.MidiToNote(p.Value & 65535), true);
            }
            else
            {
                timingf = (p.Key - ((int)(p.Key) / 4)) * 4f;
                //Debug.LogWarning("off note " + mn + ", " + timingf);
                Stop(-p.Value & 65535, -p.Value >> 16);
                SetNotesEndPlaying(mn, timingf, -p.Value >> 16);
                Piano.SetKeyPlaying(Note.MidiToNote(-p.Value & 65535), false);
            }
        }
        manager.isPlaying = false;
        //Debug.LogWarning("End playing");
    }

    public void PlayAll()
    {
        List<KeyValuePair<float, int>> list = ToMidiAll();
        StopAll();
        play = __PlayAll(list);
        manager.StartCoroutine(play);
    }

    public void PlayFromCursor()
    {
        List<KeyValuePair<float, int>> list = ToMidi();
        StopAll();
        play = __PlayAll(list);
        manager.StartCoroutine(play);
    }

    /// <summary>
    /// 모든 재생 중인 음을 멈춥니다.
    /// </summary>
    public void StopAll()
    {
        int i, j;
        if (play != null) manager.StopCoroutine(play);
        for (i = 0; i < 128; i++) for (j = 0; j < 3; j++) Stop(i, j);
    }

    public void SaveAll()
    {
        List<KeyValuePair<float, int>> list = ToMidiAll();
        Sequence seq = new Sequence();
        Track tr = new Track();
        float last = 0;
        foreach (KeyValuePair<float, int> p in list)
        {
            Debug.Log((int)(p.Key * 25 + .5) + " " + p.Value);
            tr.Insert((int)(p.Key * 25 + .5), new ChannelMessage(p.Value > 0 ? ChannelCommand.NoteOn : ChannelCommand.NoteOff, p.Value < 0 ? -p.Value >> 16 : p.Value >> 16, p.Value < 0 ? -p.Value & 65535 : p.Value & 65535, 127));
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
    /// startMeasureNum 마디부터 시작해 음소거되지 않은 부분에 음표가 하나라도 있으면 true를 반환합니다.
    /// 만약 startMeasureNum이 0보다 작게 주어지면 false를 반환합니다.
    /// </summary>
    /// <param name="startMeasureNum"></param>
    /// <returns></returns>
    private bool IsThereAnyNote(int startMeasureNum = 0)
    {
        if (startMeasureNum < 0) startMeasureNum = 0;
        for (int j = 0; j < 3; j++)
        {
            if (!manager.GetStaff(j).GetHasPlay()) continue;
            for (int i = startMeasureNum; i < manager.GetMaxMeasureNum(); i++)
            {
                if (manager.GetStaff(j).GetMeasure(i).GetNotes().Count > 0) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 조건에 맞는 음표들을 악보에서 모두 찾아 isPlaying 상태로 만들어줍니다.
    /// playing 상태로 체크된 음표는 초록색으로 표시됩니다.
    /// </summary>
    /// <param name="measureNum"></param>
    /// <param name="timing"></param>
    private void SetNotesPlaying(int measureNum, int timing, bool isPlaying)
    {
        if (measureNum < 0 || measureNum > manager.GetMaxMeasureNum()) measureNum = 0;
        for (int j = 0; j < 3; j++)
        {
            if (!manager.GetStaff(j).GetHasPlay()) continue;
            foreach (Note n in manager.GetStaff(j).GetMeasure(measureNum).GetNotes()) {
                if (n.GetTiming() == timing) n.SetIsPlaying(isPlaying);
            }
        }
    }

    /// <summary>
    /// 주어진 보표(staff)의 마디(measureNum)에서 timing보다 낮은 timing을 가진 
    /// 모든 음표를 찾아 playing 상태를 해제합니다.
    /// </summary>
    /// <param name="measureNum"></param>
    /// <param name="timing"></param>
    /// <param name="staff"></param>
    private void SetNotesEndPlaying(int measureNum, float timing, int staff)
    {
        if (measureNum < 0 || measureNum > manager.GetMaxMeasureNum()) measureNum = 0;
        if (!manager.GetStaff(staff).GetHasPlay()) return;
        foreach (Note n in manager.GetStaff(staff).GetMeasure(measureNum).GetNotes())
        {
            if (n.GetTiming() < timing) n.SetIsPlaying(false);
        }
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

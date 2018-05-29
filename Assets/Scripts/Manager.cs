using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Manager : MonoBehaviour {

    public static Manager manager;
    
    public GameObject measureNumCanvas;
    public GameObject noteObject;
    public GameObject additionalLineObject;
    public GameObject accidentalObject;

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
            // TODO this is midi encoding. PLEASE change integers to our encoding!
            tempChords.Add(new Chord(49, 53, 56));
            tempChords.Add(new Chord(54, 58, 61));
            tempChords.Add(new Chord(47, 50, 54));
            tempChords.Add(new Chord(51, 54, 57));
            tempChords.Add(new Chord(47, 49, 54));
            tempChords.Add(new Chord(51, 55, 58, 61));
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

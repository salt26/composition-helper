using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Manager : MonoBehaviour {

    public static Manager manager;
    public GameObject melodyPanel;
    public bool isChordDriven;
    public int measure;

	// Use this for initialization
	void Awake () {
        manager = this;
        DontDestroyOnLoad(this);
	}

    void FixedUpdate()
    {
        if ((melodyPanel == null || !melodyPanel.name.Equals("MelodyPanel")) && SceneManager.GetActiveScene().name.Equals("Start"))
        {
            melodyPanel = GameObject.Find("MelodyPanel");
        }
    }

    public void SetDriven(bool isChord)
    {
        isChordDriven = isChord;
        melodyPanel.SetActive(false);
    } 
	
    public void SetMeasureAndChangeScene(float sliderVal)
    {
        float i = Mathf.Round(sliderVal);
        if (i >= 2f)
        {
            measure = 8 * (int)i;
            SceneManager.LoadScene("Score");
        }
        else
        {
            Debug.LogError("SetMeasure Error!");
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else 
		Application.Quit();
#endif
    }
}

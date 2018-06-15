using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder : MonoBehaviour {

    static public Finder finder;
    public GameObject rhythmCaveatPanel;
    public GameObject chordPanel;
    public GameObject developingPanel;
    public GameObject savePanel;
    public GameObject instructionPanel;
    public GameObject instructionPanel2;
    public GameObject playButton;
    
    private void Awake()
    {
        finder = this;
    }

    public void RecommendChords()
    {
        if (Manager.manager != null)
        {
            int mn;
            if ((mn = Manager.manager.GetCursorMeasureNum()) >= 1)
                Manager.manager.RecommendChords(Manager.manager.GetStaff(2).GetMeasure(mn - 1).GetChord());
            else Manager.manager.RecommendChords(null);
        }
    }
}

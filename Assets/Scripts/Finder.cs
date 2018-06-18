using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder : MonoBehaviour {

    static public Finder finder;
    public GameObject darkPanel;
    public GameObject rhythmCaveatPanel;
    public GameObject chordPanel;
    public GameObject developingPanel;
    public GameObject savePanel;
    public GameObject instructionPanel;
    public GameObject instructionPanel2;
    public GameObject playButton;
    public GameObject playPrevChordButton;
    public GameObject copyButton;
    public GameObject pasteButton;
    
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

    public void PlayPrevChord()
    {
        if (Manager.manager != null)
        {
            int mn;
            if ((mn = Manager.manager.GetCursorMeasureNum()) >= 1)
            {
                Debug.LogWarning("PlayPrevChord");
                Debug.LogWarning(Manager.manager.GetStaff(2).GetMeasure(mn - 1).GetChord().GetNotes().Count);
                Manager.manager.GetStaff(2).GetMeasure(mn - 1).GetChord().PlayChord();
            }
        }
    }

    public bool HasPopupOn()
    {
        return (finder.chordPanel.activeInHierarchy ||
            finder.developingPanel.activeInHierarchy ||
            finder.rhythmCaveatPanel.activeInHierarchy ||
            finder.savePanel.activeInHierarchy ||
            finder.instructionPanel.activeInHierarchy ||
            finder.instructionPanel2.activeInHierarchy);
    }
}

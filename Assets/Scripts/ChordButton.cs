using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChordButton : MonoBehaviour {

    public int buttonNum;

    Chord chord;

    static bool firstTime = true;
    
	void FixedUpdate () {
        if (buttonNum < 0 || buttonNum >= 6) return;
        chord = Manager.manager.GetTempChord(buttonNum);
        if (chord == null)
        {
            Debug.Log("Error in ChordButton " + this.name);
        }
        else if (!chord.GetChordText().Equals(""))
        {
            GetComponentInChildren<Text>().text = chord.GetChordText();
        }
	}

    public void WriteChord()
    {
        if (!Manager.manager.GetIsThereFirstChord())
        {
            Manager.manager.SetIsThereFirstChord();
        }

        int measureNum = Manager.manager.GetCursorMeasureNum();
        Manager.manager.SetCursor(Manager.manager.GetStaff(2).GetMeasure(measureNum), measureNum);
        Manager.manager.GetStaff(1).GetMeasure(measureNum).ClearMeasure();
        Manager.manager.GetStaff(2).GetMeasure(measureNum).ClearMeasure();
        Color color = new Color(0, 0, 0, 0.3f);
        foreach (int c in chord.GetNotes())
        {
            if (c < 0 || c > 40) continue;
            Manager.manager.WriteNote(2, measureNum, c, "온음표", 0);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 0, color, true);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 4, color, true);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 8, color, true);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 12, color, true);
        }
        Debug.Log("ChordButton WriteChord " + chord.GetBass());
        Manager.manager.GetStaff(2).GetMeasure(measureNum).SetChord(chord);

        Manager.manager.GetStaff(1).GetMeasure(measureNum).InteractionOn();
        // Manager.manager.GetStaff(0).GetMeasure(measureNum).InteractionOn();
        if (measureNum < Manager.manager.GetMaxMeasureNum() - 1)
        {
            Manager.manager.GetStaff(2).GetMeasure(measureNum + 1).InteractionOn();
        }
        if (firstTime)
        {
            firstTime = false;
            Finder.finder.instructionPanel0.SetActive(true);
            Finder.finder.darkPanel.SetActive(true);
        }
    }

    public void PlayChord()
    {
        chord.PlayChord();
    }
}

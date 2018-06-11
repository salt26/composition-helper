using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChordButton : MonoBehaviour {

    public int buttonNum;

    Chord chord;
    
	void FixedUpdate () {
        if (buttonNum < 0 || buttonNum >= 6) return;
        chord = Manager.manager.GetTempChord(buttonNum);
        if (chord == null)
        {
            Debug.Log("Error in ChordButton "+this.name);
        }
	}

    public void WriteChord()
    {
        int measureNum = Manager.manager.GetCursorMeasureNum();
        Manager.manager.GetStaff(1).GetMeasure(measureNum).ClearMeasure();
        Manager.manager.GetStaff(2).GetMeasure(measureNum).ClearMeasure();
        Color color = new Color(0, 0, 0, 0.3f);
        foreach (int c in chord.GetNotes())
        {
            if (c < 0 || c > 40) continue;
            Manager.manager.WriteNote(2, measureNum, c, "온음표", 0);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 0, color);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 4, color);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 8, color);
            Manager.manager.WriteNote(1, measureNum, c, "4분음표", 12, color);
        }

        Manager.manager.GetStaff(1).GetMeasure(measureNum).InteractionOn();
        Manager.manager.GetStaff(0).GetMeasure(measureNum).InteractionOn();
        if (measureNum < Manager.manager.GetMaxMeasureNum() - 1)
        {
            Manager.manager.GetStaff(2).GetMeasure(measureNum + 1).InteractionOn();
        }
    }

    public void PlayChord()
    {
        chord.PlayChord();
    }
}

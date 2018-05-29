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
        // TODO 현재는 첫 마디에만 화음을 쓸 수 있습니다.
        GameObject g;
        Measure measure = Manager.manager.GetStaff(2).GetMeasure(0);
        measure.ClearMeasure();
        foreach (int c in chord.GetNotes())
        {
            if (c < 0 || c > 40) continue;
            Manager.manager.WriteNote(2, 0, c, "온음표", 0);
        } 
    }

    public void PlayChord()
    {
        chord.PlayChord();
    }
}

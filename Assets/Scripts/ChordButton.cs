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
        Manager.manager.GetStaff(1).GetMeasure(0).ClearMeasure();
        Manager.manager.GetStaff(2).GetMeasure(0).ClearMeasure();
        Color color = new Color(0, 0, 0, 0.3f);
        foreach (int c in chord.GetNotes())
        {
            if (c < 0 || c > 40) continue;
            Manager.manager.WriteNote(2, 0, c, "온음표", 0);
            Manager.manager.WriteNote(1, 0, c, "4분음표", 0, color);
            Manager.manager.WriteNote(1, 0, c, "4분음표", 4, color);
            Manager.manager.WriteNote(1, 0, c, "4분음표", 8, color);
            Manager.manager.WriteNote(1, 0, c, "4분음표", 12, color);
        } 
    }

    public void PlayChord()
    {
        chord.PlayChord();
    }
}

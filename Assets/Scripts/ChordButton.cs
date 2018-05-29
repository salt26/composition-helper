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
        GameObject measure = Manager.manager.GetStaff(2).GetMeasure(0);
        foreach (int c in chord.GetNotes())
        {
            //if (c < 36 || c > 64) continue;
            if (c < 0 || c > 40) continue;
            g = Instantiate(Manager.manager.noteObject, measure.GetComponent<Transform>());
            g.GetComponent<Note>().Initialize(false, c, "온음표", 0);
            /*
            g = Instantiate(Manager.manager.noteObject, measure.GetComponent<Transform>());
            g.GetComponent<Transform>().localPosition = new Vector3(-5.16f, Note.NoteToScore(c, false), 0f);
            g.GetComponent<SpriteRenderer>().color = Color.black;
            //if (c >= 50)    // tail up
            // based on bass clef
            if (c >= 20)
            {
                g.GetComponent<SpriteRenderer>().sprite = Resources.Load("Note1", typeof(Sprite)) as Sprite;
            }
            else            // tail down
            {
                g.GetComponent<SpriteRenderer>().sprite = Resources.Load("Note1", typeof(Sprite)) as Sprite;
            }
            // TODO 음높이에 따라 추가 선 붙이기
            /*
            switch ((int)(Chord.NoteToScore(c, false) * 8f))
            {
                case 8:
                    GameObject temp1 = Instantiate()
            }
            */
            // TODO 임시표 붙이기
            // TODO 미디 기준 번호 대신 고유한 인코딩을 사용하자.
        } 
    }
}

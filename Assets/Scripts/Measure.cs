using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour
{
    List<Note> notes = new List<Note>();
    bool isInteractive = true;
    bool isHighlighting = false;
    bool hasHoveringSaveButton = false;
    Chord chord;
    List<int> rhythm = null;

    static bool firstTime = true;

    void FixedUpdate()
    {
        if (isHighlighting)
        {

        }
        else if (Manager.manager != null && this.Equals(Manager.manager.GetCursor()))
        {
            if (hasHoveringSaveButton) GetComponent<SpriteRenderer>().color = new Color(1f, 0.6899f, 0.2405f, 0.1f);
            else GetComponent<SpriteRenderer>().color = new Color(1f, 0.6899f, 0.2405f, 1f);

            if (GetComponentInParent<Staff>().staffName.Equals("Chord"))
            {
                if (Manager.manager.GetIsPlaying())
                    Manager.manager.GetChordRecommendButton().interactable = false;
                else
                    Manager.manager.GetChordRecommendButton().interactable = true;
                Manager.manager.GetRhythmRecommendButton().interactable = false;
                if (notes.Count == 0)
                    Manager.manager.GetChordRecommendButton().GetComponent<Highlighter>().HighlightOn();
            }
            else if (GetComponentInParent<Staff>().staffName.Equals("Accompaniment"))
            {
                Manager.manager.GetChordRecommendButton().interactable = false;
                Manager.manager.GetRhythmRecommendButton().interactable = false;
            }
            else if (GetComponentInParent<Staff>().staffName.Equals("Melody"))
            {
                Manager.manager.GetChordRecommendButton().interactable = false;
                if (Manager.manager.GetIsPlaying())
                    Manager.manager.GetRhythmRecommendButton().interactable = false;
                else
                    Manager.manager.GetRhythmRecommendButton().interactable = true;
                if (notes.Count == 0)
                    Manager.manager.GetRhythmRecommendButton().GetComponent<Highlighter>().HighlightOn();
            }
        }
        else if (isInteractive)
        {
            if (hasHoveringSaveButton) GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0.1f);
            else GetComponent<SpriteRenderer>().color = Color.black;
            if (GetComponentInParent<Staff>().staffName.Equals("Melody")
                && notes.Count == 0
                && Manager.manager.GetStaff(1).GetMeasure(GetComponentInParent<Staff>().GetMeasureNum(this)).GetNotes().Count != 0)
            {
                HighlightOn();
            }
            if (GetComponentInParent<Staff>().staffName.Equals("Accompaniment")
                && notes.Count > 4
                && Manager.manager.GetStaff(2).GetMeasure(GetComponentInParent<Staff>().GetMeasureNum(this)).GetNotes().Count != 0)
            {
                HighlightOn();
            }
            if (GetComponentInParent<Staff>().staffName.Equals("Chord")
                && notes.Count == 0)
            {
                HighlightOn();
            }
        }
        else
        {
            if (hasHoveringSaveButton) GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.8325f, 0.8325f, 0.08f);
            else GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.8325f, 0.8325f, 0.8f);
        }
    }

    public List<Note> GetNotes()
    {
        return new List<Note>(notes);
    }

    public List<KeyValuePair<float, int> > ToMidi()
    {
        int cursorMN = 0;
        if (Manager.manager != null)
        {
            cursorMN = Manager.manager.GetCursorMeasureNum();
            if (cursorMN < 0) cursorMN = 0;
        }
        List<KeyValuePair<float, int> > res = new List<KeyValuePair<float, int>>();
        int this_m = (GetComponentInParent<Staff>().GetMeasureNum(this) - cursorMN) * 4;
        if (this_m < 0) this_m = GetComponentInParent<Staff>().GetMeasureNum(this) * 4;
        foreach (Note n in notes)
        {
            res.Add(new KeyValuePair<float, int>(this_m + n.GetTiming() / 4f, Note.NoteToMidi(n.GetPitch())));
            res.Add(new KeyValuePair<float, int>(this_m + (n.GetTiming() + n.GetRhythm() * 6f / 7f) / 4f, -Note.NoteToMidi(n.GetPitch())));
        }
        return res;
    }

    public List<KeyValuePair<float, int>> ToMidiAll()
    {
        List<KeyValuePair<float, int>> res = new List<KeyValuePair<float, int>>();
        int this_m = GetComponentInParent<Staff>().GetMeasureNum(this) * 4;
        foreach (Note n in notes)
        {
            res.Add(new KeyValuePair<float, int>(this_m + n.GetTiming() / 4f, Note.NoteToMidi(n.GetPitch())));
            res.Add(new KeyValuePair<float, int>(this_m + (n.GetTiming() + n.GetRhythm() * 6f / 7f) / 4f, -Note.NoteToMidi(n.GetPitch())));
        }
        return res;
    }

    void OnMouseDown()
    {
        if (isInteractive)
        {
            HighlightOff();
            Selected();
        }
    }

    public void Selected()
    {
        if (Finder.finder.HasPopupOn() || Manager.manager == null || Manager.manager.GetIsPlaying())
            return;
        Piano.SetAllKeyHighlightOff();
        Piano.SetAllKeyChordOff();
        int mn = GetComponentInParent<Staff>().GetMeasureNum(this);
        if (Manager.manager.GetStaff(2).GetMeasure(mn).GetNotes().Count > 0)
        {
            for (int tone = 0; tone <= 68; tone++)
            {
                bool b = false;
                foreach (Note n in Manager.manager.GetStaff(2).GetMeasure(mn).GetNotes())
                {
                    if ((tone - n.GetPitch()) % 17 == 0)
                    {
                        b = true;
                        break;
                    }
                }
                if (b) Piano.SetKeyChord(tone, true);

            }
        }

        Manager.manager.GetChordRecommendButton().GetComponent<Highlighter>().HighlightOff();
        Manager.manager.GetRhythmRecommendButton().GetComponent<Highlighter>().HighlightOff();
        Manager.manager.SetCursor(this, GetComponentInParent<Staff>().GetMeasureNum(this));

        if (firstTime && mn > 0)
        {
            firstTime = false;
            Finder.finder.darkPanel.SetActive(true);
            Finder.finder.instructionPanel3.SetActive(true);
        }
    }

    public void AddNote(Note note)
    {
        notes.Add(note);
        notes.Sort(delegate(Note u, Note v)
        {
            return u.GetTiming() - v.GetTiming();
        });
        int i, j;
        for (i = 0; i < notes.Count; i++)
        {
            for (j = i - 1; j >= 0; j--) if (Note.NoteToScore(notes[i].GetPitch(), notes[i].GetIsTreble()) == Note.NoteToScore(notes[j].GetPitch(), notes[j].GetIsTreble())) break;
            if (j == -1)
            {
                int acc = Note.NoteToAccidental(notes[i].GetPitch());
                notes[i].ChangeAccidental(acc == 0 ? -1 : acc);
            }
            else
            {
                int acci = Note.NoteToAccidental(notes[i].GetPitch()), accj = Note.NoteToAccidental(notes[j].GetPitch());
                notes[i].ChangeAccidental(acci == accj ? -1 : acci);
            }
        }
    }

    public void RemoveNote(Note note)
    {
        if (notes.Contains(note))
        {
            Piano.SetKeyHighlight(note.GetPitch(), false);
            if (note.Equals(Manager.manager.GetCursor())) Manager.manager.SetCursorToNull();
            notes.Remove(note);
            Destroy(note.gameObject);
        }
    }

    public void ClearMeasure()
    {
        rhythm = null;
        foreach (Note n in notes)
        {
            Piano.SetKeyHighlight(n.GetPitch(), false);
            if (n.Equals(Manager.manager.GetCursor())) Manager.manager.SetCursorToNull();
            Destroy(n.gameObject);
        }
        notes.Clear();
    }

    /// <summary>
    /// 저장 버튼에 마우스를 올릴 때 소리 재생이 꺼진 마디가 투명하게 보이도록 하는 함수입니다.
    /// b가 true이면 해당 마디는 투명해집니다.
    /// </summary>
    /// <param name="b"></param>
    public void SetHoveringSaveButton(bool b)
    {
        hasHoveringSaveButton = b;
    }

    public bool GetHoveringSaveButton()
    {
        return hasHoveringSaveButton;
    }

    public void SetChord(Chord ch)
    {
        chord = ch;
    }

    public Chord GetChord()
    {
        Debug.Log("Measure.GetChord " + chord.GetBass());
        Chord ch = new Chord(chord.GetNotes());
        ch.SetBass(chord.GetBass());
        ch.SetChordName(chord.GetChordName());
        ch.SetChordText(chord.GetChordText());
        Debug.LogWarning("GetChord " + ch.GetNotes().Count + " / chord " + chord.GetNotes().Count);
        return ch;
    }

    public void SetRhythm(List<int> rhythm)
    {
        this.rhythm = new List<int>();
        for (int i = 0; i < rhythm.Count; i++)
        {
            this.rhythm.Add(rhythm[i]);
        }
    }

    public List<int> GetRhythm()
    {
        if (rhythm == null) return null;
        List<int> r = new List<int>();
        for (int i = 0; i < rhythm.Count; i++)
        {
            r.Add(rhythm[i]);
        }
        return r;
    }

    public void InteractionOff()
    {
        isInteractive = false;
        HighlightOff();
    }

    public void InteractionOn()
    {
        isInteractive = true;
    }

    public void HighlightOn()
    {
        if (isHighlighting || !isInteractive) return;
        isHighlighting = true;
        StartCoroutine("HighlightColor");
    }

    public void HighlightOff()
    {
        if (!isHighlighting) return;
        isHighlighting = false;
        StopCoroutine("HighlightColor");
        //if (isInteractive) GetComponent<SpriteRenderer>().color = Color.black;
        //else GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.51f, 0.85f, 0.7f);
    }

    IEnumerator HighlightColor()
    {
        while (true)
        {
            int frame = 16;
            for (int i = 0; i < frame; i++)
            {
                if (hasHoveringSaveButton)
                    GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.5443f, 0.8962f, 0.1564f, 0.1f), new Color(0.8980f, 0.1568f, 0.4420f, 0.1f), i / (float)frame);
                else
                    GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.5443f, 0.8962f, 0.1564f, 1f), new Color(0.8980f, 0.1568f, 0.4420f, 1f), i / (float)frame);
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i < frame; i++)
            {
                if (hasHoveringSaveButton)
                    GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.8980f, 0.1568f, 0.4420f, 0.1f), new Color(0.1568f, 0.5407f, 0.8980f, 0.1f), i / (float)frame);
                else
                    GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.8980f, 0.1568f, 0.4420f, 1f), new Color(0.1568f, 0.5407f, 0.8980f, 1f), i / (float)frame);
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i < frame; i++)
            {
                if (hasHoveringSaveButton)
                    GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.1568f, 0.5407f, 0.8980f, 0.1f), new Color(0.5443f, 0.8962f, 0.1564f, 0.1f), i / (float)frame);
                else
                    GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.1568f, 0.5407f, 0.8980f, 1f), new Color(0.5443f, 0.8962f, 0.1564f, 1f), i / (float)frame);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}

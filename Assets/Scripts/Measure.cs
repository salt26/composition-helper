using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour
{
    List<Note> notes = new List<Note>();
    bool isInteractive = true;
    bool isHighlighting = false;

    void FixedUpdate()
    {
        if (isHighlighting)
        {

        }
        else if (Manager.manager != null && this.Equals(Manager.manager.GetCursor()))
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.6899f, 0.2405f, 1f);

            if (GetComponentInParent<Staff>().staffName.Equals("Chord"))
            {
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
                Manager.manager.GetRhythmRecommendButton().interactable = true;
                if (notes.Count == 0)
                    Manager.manager.GetRhythmRecommendButton().GetComponent<Highlighter>().HighlightOn();
            }
        }
        else if (isInteractive)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
            if (GetComponentInParent<Staff>().staffName.Equals("Melody")
                && notes.Count == 0
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
            GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.8325f, 0.8325f, 0.8f);
        }
    }

    public List<Note> GetNotes()
    {
        return new List<Note>(notes);
    }

    public List<KeyValuePair<float, int> > ToMidi()
    {
        List<KeyValuePair<float, int> > res = new List<KeyValuePair<float, int>>();
        foreach (Note n in notes)
        {
            res.Add(new KeyValuePair<float, int>(n.GetTiming() / 4f, Note.NoteToMidi(n.GetPitch())));
            res.Add(new KeyValuePair<float, int>((n.GetTiming() + n.GetRhythm() * 0.9f) / 4f, -Note.NoteToMidi(n.GetPitch())));
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
        if (GetComponentInParent<Staff>().chordPanel.activeInHierarchy)
            return;
        Piano.SetAllKeyHighlightOff();

        Manager.manager.GetChordRecommendButton().GetComponent<Highlighter>().HighlightOff();
        Manager.manager.GetRhythmRecommendButton().GetComponent<Highlighter>().HighlightOff();
        Manager.manager.SetCursor(this, GetComponentInParent<Staff>().GetMeasureNum(this));
    }

    public void AddNote(Note note)
    {
        notes.Add(note);
    }

    public void RemoveNote(Note note)
    {
        if (notes.Contains(note))
        {
            Piano.SetKeyHighlight(note.GetPitch(), false);
            notes.Remove(note);
            Destroy(note.gameObject);
        }
    }

    public void ClearMeasure()
    {
        foreach (Note n in notes)
        {
            Piano.SetKeyHighlight(n.GetPitch(), false);
            Destroy(n.gameObject);
        }
        notes.Clear();
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
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.5443f, 0.8962f, 0.1564f, 1f), new Color(0.8980f, 0.1568f, 0.4420f, 1f), i / (float)frame);
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i < frame; i++)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.8980f, 0.1568f, 0.4420f, 1f), new Color(0.1568f, 0.5407f, 0.8980f, 1f), i / (float)frame);
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i < frame; i++)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0.1568f, 0.5407f, 0.8980f, 1f), new Color(0.5443f, 0.8962f, 0.1564f, 1f), i / (float)frame);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}

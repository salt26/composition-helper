using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour
{

    List<Note> notes = new List<Note>();
    bool isInteractive = true;
    bool isHighlighting = false;

    /*
    void FixedUpdate()
    {
        Ray ray = Manager.manager.GetMainCamera().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (isInteractive && Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 8))) {
            Debug.Log("Hello");
            HighlightOff();
            Manager.manager.GetChordRecommendButton().interactable = true;
            Selected();
        }
    }
    */

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
            res.Add(new KeyValuePair<float, int>((n.GetTiming() + n.GetRhythm()) / 4f, -Note.NoteToMidi(n.GetPitch())));
        }
        return res;
    }

    void OnMouseDown()
    {
        if (isInteractive)
        {
            HighlightOff();
            Manager.manager.GetChordRecommendButton().interactable = true;
            Selected();
        }
    }

    public void AddNote(Note note)
    {
        notes.Add(note);
    }

    public void RemoveNote(Note note)
    {
        if (notes.Contains(note))
        {
            notes.Remove(note);
            Destroy(note.gameObject);
        }
    }

    public void ClearMeasure()
    {
        foreach (Note n in notes)
        {
            Destroy(n.gameObject);
        }
        notes.Clear();
    }

    public void InteractionOff()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.8325f, 0.8325f, 0.8f);
        isInteractive = false;
    }

    public void InteractionOn()
    {
        GetComponent<SpriteRenderer>().color = Color.black;
        isInteractive = true;
    }

    public void Selected()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 0.6899f, 0.2405f, 1f);
        Manager.manager.SetCursor(this);
    }

    public void HighlightOn()
    {
        if (isHighlighting) return;
        isHighlighting = true;
        StartCoroutine("HighlightColor");
    }

    public void HighlightOff()
    {
        if (!isHighlighting) return;
        isHighlighting = false;
        StopCoroutine("HighlightColor");
        if (isInteractive) GetComponent<SpriteRenderer>().color = Color.black;
        else GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.51f, 0.85f, 0.7f);
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

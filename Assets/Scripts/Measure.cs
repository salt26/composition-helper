using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour
{

    List<Note> notes = new List<Note>();
    bool isInteractive = true;
    Color disabledColor = new Color(0.8325f, 0.8325f, 0.8325f, 0.8f);
    Color enabledColor = Color.black;
    Color selectedColor = new Color(1f, 0.6899f, 0.2405f, 1f);

    void FixedUpdate()
    {
        if (Manager.manager != null && this.Equals(Manager.manager.GetCursor()) && isInteractive)
        {
            GetComponent<SpriteRenderer>().color = selectedColor;
        }
        else if (isInteractive)
        {
            GetComponent<SpriteRenderer>().color = enabledColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = disabledColor;
        }
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
        GetComponent<SpriteRenderer>().color = disabledColor;
        isInteractive = false;
    }

    public void InteractionOn()
    {
        GetComponent<SpriteRenderer>().color = enabledColor;
        isInteractive = true;
    }

    public void Selected()
    {
        Manager.manager.SetCursor(this);
    }

    public void HighlightOn()
    {
        GetComponent<Highlighter>().HighlightOn();
    }

    public void HighlightOff()
    {
        GetComponent<Highlighter>().HighlightOff();

        if (isInteractive) GetComponent<SpriteRenderer>().color = Color.black;
        else GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.8325f, 0.8325f, 0.8f);
    }


}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measure : MonoBehaviour
{

    List<Note> notes = new List<Note>();
    bool isInteractive = true;
    bool isHighlighting = false;

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
            notes.Remove(n);
            Destroy(n.gameObject);
        }
    }

    public void InteractionOff()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.8325f, 0.51f, 0.85f, 0.7f);
        isInteractive = false;
    }

    public void InteractionOn()
    {
        GetComponent<SpriteRenderer>().color = Color.black;
        isInteractive = true;
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
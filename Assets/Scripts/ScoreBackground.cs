using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreBackground : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    static public ScoreBackground sb;

    public Scrollbar scrollbar;
    public Camera mainCamera;
    private float x, s, mc;
    private bool clicked, dragged;
    private object cursor;
    private int cursorMN;

    void Awake()
    {
        sb = this;
        clicked = false;
        //dragged = false;
    }

    void OnMouseDown()
    {
        if (!clicked)
        {
            clicked = true;
            x = Input.mousePosition.x / 60f;
            s = scrollbar.value;
            /*
            if (Manager.manager != null)
            {
                cursor = Manager.manager.GetCursor();
                cursorMN = Manager.manager.GetCursorMeasureNum();
            }
            */
            HideCursor();
        }
    }

    void OnMouseDrag()
    {
        if (Manager.manager != null)
        {
            scrollbar.value = s - ((Input.mousePosition.x / 60f) - x) *
                (1 / ((Manager.manager.GetMaxMeasureNum() - 1) * 11f - 5f));
            /*
            if (!dragged && Mathf.Abs((Input.mousePosition.x / 60f) - x) > 1f)
            {
                dragged = true;
            }
            */
        }
    }

    void OnMouseUp()
    {
        clicked = false;
        /*
        if (dragged && cursor != null && Manager.manager != null)
        {
            if (cursor.GetType() == typeof(Note))
            {
                ((Note)cursor).Selected();
            }
            else if (cursor.GetType() == typeof(Measure))
            {
                ((Measure)cursor).HighlightOff();
                ((Measure)cursor).Selected();
            }
        }
        if (!dragged)
        {
            HideCursor();
        }
        */
        //dragged = false;
    }

    public void HideCursor()
    {
        if (Manager.manager != null)
        {
            Manager.manager.SetCursorToNull();
        }
    }
    
}

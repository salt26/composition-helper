using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCursor : MonoBehaviour
{
    void OnMouseDown()
    {
        Selected();
    }

    public void Selected()
    {
        if (Manager.manager != null)
        {
            Manager.manager.SetCursorToNull();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour {

    bool isHighlighting = false;

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

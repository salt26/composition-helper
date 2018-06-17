using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isPlaying = false;

    void FixedUpdate()
    {
        if (Manager.manager != null && Manager.manager.GetIsPlaying()) {
            isPlaying = true;
            StopCoroutine(ToWhite());
            GetComponent<Image>().color = new Color(0f, 0.839f, 0.5312847f, 1f);
        }
        else if (Manager.manager != null && !Manager.manager.GetIsPlaying() && isPlaying)
        {
            isPlaying = false;
            StartCoroutine(ToWhite());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        int mn;
        if (Manager.manager == null) return;
        //Debug.LogWarning("Play OnPointEnter");
        mn = Manager.manager.GetCursorMeasureNum();
        bool[] vanishing = new bool[3] { !Manager.manager.GetStaff(0).GetHasPlay(), !Manager.manager.GetStaff(1).GetHasPlay(), !Manager.manager.GetStaff(2).GetHasPlay() };
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < mn; i++)
            {
                Manager.manager.GetStaff(j).GetMeasure(i).SetHoveringSaveButton(true);
            }
            
            for (int i = (mn < 0 ? 0 : mn); i < Manager.manager.GetMaxMeasureNum(); i++)
            {
                Manager.manager.GetStaff(j).GetMeasure(i).SetHoveringSaveButton(vanishing[j]);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Manager.manager == null) return;
        //Debug.LogWarning("Play OnPointExit");
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < Manager.manager.GetMaxMeasureNum(); i++)
            {
                Manager.manager.GetStaff(j).GetMeasure(i).SetHoveringSaveButton(false);
            }
        }
    }

    IEnumerator ToWhite()
    {
        for (int i = 0; i < 16; i++)
        {
            GetComponent<Image>().color = Color.Lerp(new Color(0f, 0.839f, 0.5312847f, 1f), Color.white, i / 16f);
            yield return new WaitForFixedUpdate();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        int mn;
        if (Manager.manager == null) return;
        Debug.LogWarning("Play OnPointEnter");
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
        Debug.LogWarning("Play OnPointExit");
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < Manager.manager.GetMaxMeasureNum(); i++)
            {
                Manager.manager.GetStaff(j).GetMeasure(i).SetHoveringSaveButton(false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    static public SaveButton sb;

	// Use this for initialization
	void Awake () {
        sb = this;
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Manager.manager == null) return;
        bool[] vanishing = new bool[3] { !Manager.manager.GetStaff(0).GetHasPlay(), !Manager.manager.GetStaff(1).GetHasPlay(), !Manager.manager.GetStaff(2).GetHasPlay() };
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < Manager.manager.GetMaxMeasureNum(); i++)
            {
                Manager.manager.GetStaff(j).GetMeasure(i).SetHoveringSaveButton(vanishing[j]);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Manager.manager == null) return;
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < Manager.manager.GetMaxMeasureNum(); i++)
            {
                Manager.manager.GetStaff(j).GetMeasure(i).SetHoveringSaveButton(false);
            }
        }
    }
}

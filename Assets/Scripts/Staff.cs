using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour {

    public GameObject measure;
    public string staffName;  // "Melody", "Accompaniment", "Chord" 중 하나
    bool hasPlay = true;
    List<Measure> measures = new List<Measure>();

    /// <summary>
    /// 인자 m만큼 마디를 생성해주는 함수입니다.
    /// </summary>
    /// <param name="m"></param>
    public void CreateMeasure(int m)
    {
        GameObject g;
        measures.Clear();
        for (int i = 0; i < m - 1; i++)
        {
            g = Instantiate(measure, GetComponent<Transform>());
            g.GetComponent<Transform>().localPosition = new Vector3(i * 11f, 0f, 0f);
            measures.Add(g.GetComponent<Measure>());
        }
        g = Instantiate(measure, GetComponent<Transform>());
        g.GetComponent<Transform>().localPosition = new Vector3((m - 1) * 11f, 0f, 0f);
        g.GetComponent<SpriteRenderer>().sprite = Resources.Load("staff5", typeof(Sprite)) as Sprite;
        measures.Add(g.GetComponent<Measure>());
    }

    /// <summary>
    /// 특정된 마디(Measure)를 반환합니다.
    /// </summary>
    /// <param name="measureIndex"></param>
    /// <returns></returns>
    public Measure GetMeasure(int measureIndex)
    {
        if (measureIndex >= 0 && measureIndex < measures.Count)
        {
            return measures[measureIndex];
        }
        return null;
    }

    /// <summary>
    /// 현재 악보의 원하는 위치에 원하는 음표를 하나 그리는 메서드입니다.
    /// 음표 게임오브젝트를 생성하고 해당 마디의 음표 리스트에 새 음표를 추가합니다.
    /// </summary>
    /// <param name="staff"></param>
    /// <param name="measure"></param>
    /// <param name="pitch"></param>
    /// <param name="rhythm"></param>
    /// <param name="timing"></param>
    public void WriteNote(int measure, int pitch, string rhythm, int timing)
    {
        if (pitch < 0 || pitch > 68 || rhythm == null || timing < 0 || timing >= 16) return;
        GameObject g = Instantiate(Manager.manager.noteObject, GetMeasure(measure).GetComponent<Transform>());
        g.GetComponent<Note>().Initialize(staffName.Equals("Melody"), pitch, rhythm, timing);
        GetMeasure(measure).AddNote(g.GetComponent<Note>());
    }

    public void TogglePlay()
    {
        hasPlay = !hasPlay;
    }

    public void InteractionAllOff()
    {
        foreach (Measure m in measures)
        {
            m.InteractionOff();
        }
    }

    public void InteractionAllOn()
    {
        foreach (Measure m in measures)
        {
            m.InteractionOn();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour {

    public GameObject measure;
    List<GameObject> measures = new List<GameObject>();

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
            measures.Add(g);
        }
        g = Instantiate(measure, GetComponent<Transform>());
        g.GetComponent<Transform>().localPosition = new Vector3((m - 1) * 11f, 0f, 0f);
        g.GetComponent<SpriteRenderer>().sprite = Resources.Load("staff5", typeof(Sprite)) as Sprite;
        measures.Add(g);
    }

    /// <summary>
    /// 특정된 마디 게임오브젝트를 반환합니다.
    /// </summary>
    /// <param name="measureIndex"></param>
    /// <returns></returns>
    public GameObject GetMeasure(int measureIndex)
    {
        if (measureIndex >= 0 && measureIndex < measures.Count)
        {
            return measures[measureIndex];
        }
        return null;
    }
}

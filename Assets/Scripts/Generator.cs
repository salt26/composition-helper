using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator {
    public static List<int> GenerateNotes()
    {
        List<int> res = new List<int>();
        bool[] notes = new bool[17];
        notes[0] = notes[16] = true;
        for (int i = 0; i < 7; i++)
        {
            if (Random.Range(0, 2) == 1)
            {
                notes[i * 2 + 2] = true;
            }
        }
        for (int i = 0; i < 16; i += 2)
        {
            if (notes[i] && notes[i + 2] && Random.Range(0, 4) == 1)
            {
                notes[i + 1] = true;
            }
        }
        for (int i = 0; i < 14; i += 2)
        {
            if (notes[i] && !notes[i + 2] && notes[i + 4])
            {
                int t = Random.Range(0, 8);
                switch (t)
                {
                    case 1:
                        notes[i + 1] = true;
                        break;
                    case 2:
                        notes[i + 3] = true;
                        break;
                    case 3:
                        notes[i + 1] = true;
                        notes[i + 3] = true;
                        break;
                }
            }
        }
        for (int i = 0; i < 10; i += 2)
        {
            if (notes[i] && !notes[i + 2] && !notes[i + 4] && notes[i + 6] && Random.Range(0, 4) == 1)
            {
                notes[i + 3] = true;
            }
        }
        for (int i = 0; i < 6; i += 2)
        {
            if (notes[i] && !notes[i + 2] && !notes[i + 4] && !notes[i + 6] && !notes[i + 8] && notes[i + 10])
            {
                int t = Random.Range(0, 4);
                if (t == 1)
                {
                    notes[i + 6] = true;
                }
                else
                {
                    notes[i + 4] = true;
                }
            }
        }
        int cnt = 0;
        for (int i = 1; i <= 16; i++)
        {
            cnt++;
            if (notes[i])
            {
                res.Add(cnt);
                cnt = 0;
            }
        }
        string tmp = "";
        foreach (int it in res)
        {
            tmp += it;
            tmp += " ";
        }
        Debug.Log(tmp);
        return res;
    }
}

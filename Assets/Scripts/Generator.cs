using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator {
    public static List<int> GenerateNotes()
    {
        List<int> res = new List<int>();
        bool[] notes = new bool[17];
        int i;
        notes[0] = notes[16] = true;
        while (true)
        {
            for (i = 1; i < 16; i++) notes[i] = false;
            for (i = 0; i < 7; i++)
            {
                if (Random.Range(0, 2) == 1)
                {
                    notes[i * 2 + 2] = true;
                }
            }
            for (i = 0; i < 16; i += 2)
            {
                if (notes[i] && notes[i + 2] && Random.Range(0, 4) == 1)
                {
                    notes[i + 1] = true;
                }
            }
            for (i = 0; i < 14; i += 2)
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
            for (i = 0; i < 10; i += 2)
            {
                if (notes[i] && !notes[i + 2] && !notes[i + 4] && notes[i + 6] && Random.Range(0, 4) == 1)
                {
                    notes[i + 3] = true;
                }
            }
            int cnt = 0;
            for (i = 1; i <= 16; i++)
            {
                cnt++;
                if (notes[i])
                {
                    if (cnt != 1 && cnt != 2 && cnt != 3 && cnt != 4 && cnt != 6 && cnt != 8 && cnt != 12 && cnt != 16) break;
                    res.Add(cnt);
                    cnt = 0;
                }
            }
            if (i > 16) break;
            Debug.Log("Failed");
        }
        return res;
    }
}

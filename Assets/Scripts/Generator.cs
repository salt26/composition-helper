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
            res.Clear();
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
        }
        return res;
    }

    static int[] baseNote = { 17, 10, 20, 13, 23, 14, 8, 7, 16, 21, 11, 18 };

    public static Chord GenerateChord()
    {
        int a = baseNote[Random.Range(0, 12)], b, c, d = -1;
        switch (Random.Range(0, 8))
        {
            case 0:
            case 1:     // major
                b = Note.NoteToMidi(a) + 4;
                c = b + 3;
                break;
            case 2:     // diminish
                b = Note.NoteToMidi(a) + 3;
                c = b + 3;
                break;
            case 3:     // augmentation
                b = Note.NoteToMidi(a) + 4;
                c = b + 4;
                break;
            case 5:     // minor 7
                b = Note.NoteToMidi(a) + 3;
                c = b + 4;
                d = c + 3;
                break;
            case 4:     // major 7
                b = Note.NoteToMidi(a) + 4;
                c = b + 3;
                d = c + 3;
                break;
            default:    // minor
                b = Note.NoteToMidi(a) + 3;
                c = b + 4;
                break;
        }
        b = Note.MidiToNote(b);
        c = Note.MidiToNote(c);
        if (d != -1) d = Note.MidiToNote(d);
        Debug.Log("before " + a + " " + b + " " + c);
        Debug.Log(Note.NoteToScore(a, false) + " " + Note.NoteToScore(b, false) + " " + Note.NoteToScore(c, false));
        if ((int)((Note.NoteToScore(b, false) - Note.NoteToScore(a, false)) * 8 + .5) != 2
            && Note.NoteToMidi(b) == Note.NoteToMidi(b + 1)) b++;
        if ((int)((Note.NoteToScore(c, false) - Note.NoteToScore(a, false)) * 8 + .5) != 4
            && Note.NoteToMidi(c) == Note.NoteToMidi(c + 1)) c++;
        if (d != -1 && (int)((Note.NoteToScore(d, false) - Note.NoteToScore(a, false)) * 8 + .5) != 6
            && Note.NoteToMidi(d) == Note.NoteToMidi(d + 1)) d++;
        Debug.Log("after " + a + " " + b + " " + c);
        Debug.Log(Note.NoteToScore(a, false) + " " + Note.NoteToScore(b, false) + " " + Note.NoteToScore(c, false));
        if (a == 18 && b == 24 && c == 28)
        {
            a = 19;
            b = 24;
            c = 29;
        }
        if (a == 21 && b == 27 && c == 31)
        {
            a = 22;
            b = 27;
            c = 32;
        }
        if (d != -1) return new Chord(a, b, c, d);
        return new Chord(a, b, c);
    }
}

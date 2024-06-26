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
        Chord chord;
        int a = baseNote[Random.Range(0, 12)], b, c, d = -1;
        string chordText = "";
        string chordName = "";
        switch (Random.Range(0, 7))
        {
            case 0:     // major
                b = Note.NoteToMidi(a) + 4;
                c = b + 3;
                chordText = "";
                chordName = "Major";
                break;
            case 2:     // diminish
                b = Note.NoteToMidi(a) + 3;
                c = b + 3;
                chordText = "dim";
                chordName = "diminished";
                break;
            case 3:     // augmentation
                b = Note.NoteToMidi(a) + 4;
                c = b + 4;
                chordText = "aug";
                chordName = "augmented";
                break;
            case 5:     // minor 7
                b = Note.NoteToMidi(a) + 3;
                c = b + 4;
                d = c + 3;
                chordText = "m7";
                chordName = "minor7";
                break;
            case 4:     // major 7
                b = Note.NoteToMidi(a) + 4;
                c = b + 3;
                d = c + 3;
                chordText = "7";
                chordName = "Major7";
                break;
            case 6:     // sus4 (sus2)
                b = Note.NoteToMidi(a) + 5;
                c = b + 2;
                chordText = "sus4";
                chordName = "suspension4";
                break;
            default:    // minor
                b = Note.NoteToMidi(a) + 3;
                c = b + 4;
                chordText = "m";
                chordName = "minor";
                break;
        }
        b = Note.MidiToNote(b);
        c = Note.MidiToNote(c);
        if (d != -1)
        {
            d = Note.MidiToNote(d);
            chord = new Chord(a, b, c, d);
        }
        else
        {
            chord = new Chord(a, b, c);
        }
        chord.SetChordName(chordName);
        
        //Debug.Log("before " + chord.NotesName());
        /*
        Debug.Log(Note.NoteToScore(a, false) + " " + Note.NoteToScore(b, false) + " " + Note.NoteToScore(c, false));
        if ((int)((Note.NoteToScore(b, false) - Note.NoteToScore(a, false)) * 8 + .5) != 2
            && Note.NoteToMidi(b) == Note.NoteToMidi(b + 1)) b++;
        if ((int)((Note.NoteToScore(c, false) - Note.NoteToScore(a, false)) * 8 + .5) != 4
            && Note.NoteToMidi(c) == Note.NoteToMidi(c + 1)) c++;
        if (d != -1 && (int)((Note.NoteToScore(d, false) - Note.NoteToScore(a, false)) * 8 + .5) != 6
            && Note.NoteToMidi(d) == Note.NoteToMidi(d + 1)) d++;
        */
        if ((chord = Chord.ReviseScoreNotation(chord, false)) == null)
        {
            Debug.Log("fail to revise chord");
            return GenerateChord();
        }
        //Debug.Log("after " + chord.NotesName());
        //Debug.Log("Generator " + chord.GetBass());
        chord.SetChordText(Note.NoteToName2(a) + chordText + "\n<size=10>(" + chord.NotesName() + ")</size>");
        return chord;
    }

    /// <summary>
    /// 화음을 생성하되, 확률 분포를 인자로 직접 줄 수 있습니다.
    /// chordNameBox에 chordName들을 넣어서 주면, 이 중에서 랜덤으로 하나 뽑아서 그 종류의 화음을 생성합니다.
    /// chordNameBox에는 "Major", "minor", "diminished", "augmented", "suspension4", "Major7", "minor7"만 들어가야 합니다.
    /// </summary>
    /// <param name="chordNameBox"></param>
    /// <returns></returns>
    public static Chord GenerateChord(List<string> chordNameBox)
    {
        if (chordNameBox == null || chordNameBox.Count <= 0) return null;
        Chord chord;
        int a = baseNote[Random.Range(0, 12)], b, c, d = -1;
        string chordText = "";
        string chordName = "";
        switch (chordNameBox[Random.Range(0, chordNameBox.Count)])
        {
            case "Major":
                b = Note.NoteToMidi(a) + 4;
                c = b + 3;
                chordText = "";
                chordName = "Major";
                break;
            case "diminished":
                b = Note.NoteToMidi(a) + 3;
                c = b + 3;
                chordText = "dim";
                chordName = "diminished";
                break;
            case "augmented":
                b = Note.NoteToMidi(a) + 4;
                c = b + 4;
                chordText = "aug";
                chordName = "augmented";
                break;
            case "minor7":
                b = Note.NoteToMidi(a) + 3;
                c = b + 4;
                d = c + 3;
                chordText = "m7";
                chordName = "minor7";
                break;
            case "Major7":
                b = Note.NoteToMidi(a) + 4;
                c = b + 3;
                d = c + 3;
                chordText = "7";
                chordName = "Major7";
                break;
            case "suspension4":
                b = Note.NoteToMidi(a) + 5;
                c = b + 2;
                chordText = "sus4";
                chordName = "suspension4";
                break;
            default:    // minor
                b = Note.NoteToMidi(a) + 3;
                c = b + 4;
                chordText = "m";
                chordName = "minor";
                break;
        }
        b = Note.MidiToNote(b);
        c = Note.MidiToNote(c);
        if (d != -1)
        {
            d = Note.MidiToNote(d);
            chord = new Chord(a, b, c, d);
        }
        else
        {
            chord = new Chord(a, b, c);
        }
        chord.SetChordName(chordName);
        if ((chord = Chord.ReviseScoreNotation(chord, false)) == null)
        {
            Debug.Log("fail to revise chord");
            return GenerateChord();
        }
        Debug.Log("Generator " + chord.GetBass());
        chord.SetChordText(Note.NoteToName2(a) + chordText + "\n<size=10>(" + chord.NotesName() + ")</size>");
        return chord;
    }
}

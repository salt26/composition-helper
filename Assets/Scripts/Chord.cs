using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chord {

    // 각 음표를 표현하는 값은 특별히 인코딩된 숫자를 사용한다.
    // 예) 가장 낮은 도: 0 / 바로 다음 음인 도#: 1 / 같은 음인 레b: 2 / 바로 다음 음인 레: 3 / 가장 높은 도: 68
    // 옥타브 차이 나는 중복 음이나 반음 차이 나는 두 음은 한 화음에 들어있을 수 없다.
    List<int> notes = new List<int>();
    string chordText = "";
    int bass = -1;
    string chordName = "";

    public Chord()
    {
        notes.Clear();
    }

    public Chord(List<int> ns)
    {
        notes.Clear();
        foreach (int n in ns)
        {
            notes.Add(n);
        }
        if (ns.Count > 0) bass = ns[0];
        RemoveDuplicatesAndHalfs();
    }

    public Chord(int note1, int note2, int note3)
    {
        notes.Clear();
        notes.Add(note1);
        notes.Add(note2);
        notes.Add(note3);
        if (notes.Count > 0) bass = notes[0];
        RemoveDuplicatesAndHalfs();
    }

    public Chord(int note1, int note2, int note3, int note4)
    {
        notes.Clear();
        notes.Add(note1);
        notes.Add(note2);
        notes.Add(note3);
        notes.Add(note4);
        if (notes.Count > 0) bass = notes[0];
        RemoveDuplicatesAndHalfs();
    }

    public void AddNote(int note)
    {
        notes.Add(note);
        RemoveDuplicatesAndHalfs();
    }

    /// <summary>
    /// 화음의 설명을 설정합니다.
    /// </summary>
    /// <param name="text"></param>
    public void SetChordText(string text)
    {
        chordText = text;
    }

    /// <summary>
    /// 화음의 종류 이름을 설정합니다.
    /// (예: "Major", "minor", "suspension4", "Major7", "minor7", "augmented", "diminished")
    /// </summary>
    /// <param name="name"></param>
    public void SetChordName(string name)
    {
        chordName = name;
    }

    /// <summary>
    /// 화음의 근음(베이스)을 설정합니다.
    /// 인자가 1개 이상인 생성자(Chord)로 화음을 만들면 자동으로 첫 번째 음이 근음이 됩니다.
    /// </summary>
    /// <param name="b"></param>
    public void SetBass(int b)
    {
        bass = b;
    }

    public bool IsChordal(int note)
    {
        foreach (int n in notes)
        {
            if ((n - note) % 17 == 0)
            {
                return true;
            }
        }
        return false;
    }

    public List<int> GetNotes()
    {
        return new List<int>(notes);
    }

    /// <summary>
    /// 화음의 설명을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public string GetChordText()
    {
        return chordText;
    }

    /// <summary>
    /// 화음의 종류 이름을 반환합니다.
    /// (예: "Major", "minor", "suspension4", "Major7", "minor7", "augmented", "diminished")
    /// </summary>
    /// <returns></returns>
    public string GetChordName()
    {
        return chordName;
    }

    /// <summary>
    /// 화음의 근음(베이스)을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int GetBass()
    {
        return bass;
    }

    public void PlayChord()
    {
        Manager.manager.StopAll();
        foreach (int note in notes)
        {
            Manager.manager.PlayTone(Note.NoteToMidi(note), 0);
        }
    }

    /// <summary>
    /// 중복 음, 반음 차이 음 잡아라!
    /// 화음에서 중복 음과 반음 차이 나는 음을 제거합니다.
    /// 두 음이 충돌하면 음 목록에서 앞에 있는 음은 남고, 뒤에 있는 음이 제거됩니다.
    /// </summary>
    /// <returns></returns>
    public void RemoveDuplicatesAndHalfs()
    {
        bool b = false;
        int i = 0, j = 0;
        while (true) {
            b = false;
            for (i = 0; i < GetNotes().Count; i++)
            {
                for (j = 0; j < i; j++)
                {
                    int m = (GetNotes()[i] - GetNotes()[j]) % 17;
                    if (m == 0 || m == 1 || m == 16)
                    {
                        b = true;
                        break;
                    }
                }
                if (b) break;
            }
            if (b)
            {
                notes.RemoveAt(i);
            }
            else return;
        }
    }

    /// <summary>
    /// 화음의 음들이 악보에 겹치지 않고 표시되면 true, 겹쳐서 표시되면 false를 반환합니다.
    /// </summary>
    /// <param name="chord"></param>
    /// <returns></returns>
    public static bool CheckScoreNotation(Chord chord, bool isTreble)
    {
        List<int> notes = chord.GetNotes();
        for (int i = 0; i < notes.Count; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (Mathf.Abs(Note.NoteToScore(notes[i], isTreble) - Note.NoteToScore(notes[j], isTreble)) <= 0.125f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 화음의 음들이 악보에 겹치지 않고 표시될 수 있도록 음의 위치를 보정합니다.
    /// 만약 한 옥타브 내에서 겹칠 수밖에 없는 화음이라면 다른 옥타브로도 음을 옮겨봅니다.
    /// 보정을 완료하고 그 화음을 반환합니다.
    /// isTreble은 낮은음자리표이면 false를, 높은음자리표이면 true를 넣습니다.
    /// 악보에서 겹친다는 뜻은 두 음의 y좌표가 0.125 이하만큼 차이 난다는 뜻입니다.
    /// </summary>
    /// <param name="chord"></param>
    /// <returns></returns>
    public static Chord ReviseScoreNotation(Chord chord, bool isTreble)
    {
        //if (CheckScoreNotation(chord)) return chord;
        List<Chord> enumerated = new List<Chord>();
        List<Chord> temp = new List<Chord>();
        Chord initChord = new Chord();
        initChord.SetChordName(chord.GetChordName());
        initChord.SetChordText(chord.GetChordText());
        initChord.SetBass(chord.GetBass());

        // Initialize (half moving)
        foreach (int n in chord.GetNotes())
        {
            if (Note.NoteToAccidental(n) == 2)  // b이면
            {
                initChord.AddNote(n - 1);      // #으로 통일
            }
            else
            {
                initChord.AddNote(n);
            }
        }
        Debug.Log("Chord ReviseScoreNotation " + initChord.GetBass());

        for (int i = 0; i < initChord.GetNotes().Count; i++)
        {
            if (isTreble && (initChord.GetNotes()[i] / 17 < 2
                || initChord.GetNotes()[i] / 17 > 3)) {
                initChord = ReviseNote(initChord, initChord.GetNotes()[i],
                    initChord.GetNotes()[i] % 17 + 34);
            }
            else if (!isTreble && (initChord.GetNotes()[i] / 17 < 0
                || initChord.GetNotes()[i] / 17 > 1))
            {
                initChord = ReviseNote(initChord, initChord.GetNotes()[i],
                    initChord.GetNotes()[i] % 17 + 17);
            }
        }

        // Enumerate all possible cases (half moving)
        enumerated.Add(initChord);
        for (int i = 0; i < initChord.GetNotes().Count; i++)
        {
            if (Note.NoteToAccidental(initChord.GetNotes()[i]) == 1)
            {
                temp.Clear();
                foreach (Chord c in enumerated)
                {
                    temp.Add(ReviseNote(c, initChord.GetNotes()[i], initChord.GetNotes()[i] + 1));
                }
                foreach (Chord t in temp)
                {
                    enumerated.Add(t);
                }
            }
        }

        // Check each case (half moving)
        foreach (Chord c in enumerated)
        {
            //Debug.Log("enumerated " + c.NotesName());
            if (CheckScoreNotation(c, isTreble))
            {
                return c;
            }
        }

        // 반음 조절로 실패한 경우 옥타브 조절을 시도
        initChord = new Chord();
        enumerated.Clear();
        initChord.SetChordName(chord.GetChordName());
        initChord.SetChordText(chord.GetChordText());
        initChord.SetBass(chord.GetBass());

        // Initialize (octave moving)
        foreach (int n in chord.GetNotes())
        {
            initChord.AddNote((n % 17) + 17);
        }

        // Enumerate all possible cases (octave moving)
        enumerated.Add(initChord);
        for (int i = initChord.GetNotes().Count - 1; i >= 0; i--)
        {
            if (initChord.GetNotes()[i] / 17 == 1)
            {
                temp.Clear();
                foreach (Chord c in enumerated)
                {
                    temp.Add(ReviseNote(c, initChord.GetNotes()[i], initChord.GetNotes()[i] - 17));
                }
                foreach (Chord t in temp)
                {
                    enumerated.Add(t);
                }
            }
        }

        // Check each case (octave moving)
        foreach (Chord c in enumerated)
        {
            //Debug.Log("octave enumerated " + c.NotesName());
            if (CheckScoreNotation(c, isTreble))
            {
                return c;
            }
        }
        return null;
    }

    /// <summary>
    /// 화음을 구성하는 각 음들의 계이름을 합쳐서 반환합니다.
    /// (예: "도# 미 솔#")
    /// </summary>
    /// <param name="chord"></param>
    /// <returns></returns>
    public string NotesName()
    {
        string s = "";
        foreach (int n in GetNotes())
        {
            s += Note.NoteToName(n);
            s += ",";
        }
        s = s.TrimEnd(',');
        return s;
    }

    /// <summary>
    /// 화음에서 특정 음(before)을 다른 음(after)으로 바꾸어 반환합니다.
    /// </summary>
    /// <param name="chord"></param>
    /// <param name="before"></param>
    /// <param name="after"></param>
    /// <returns></returns>
    static Chord ReviseNote(Chord chord, int before, int after)
    {
        Chord newChord = new Chord();
        newChord.SetChordName(chord.GetChordName());
        newChord.SetChordText(chord.GetChordText());
        newChord.SetBass(chord.GetBass());
        foreach (int n in chord.GetNotes())
        {
            if (n == before)
            {
                newChord.AddNote(after);
            }
            else
            {
                newChord.AddNote(n);
            }
        }
        Debug.Log("Chord ReviseNote " + newChord.GetBass());
        if (chord.GetBass() == before) newChord.SetBass(after);
        else newChord.SetBass(chord.GetBass());
        return newChord;
    }
}

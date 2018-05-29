using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chord {

    // 각 음표를 표현하는 값은 특별히 인코딩된 숫자를 사용한다.
    // 예) 가장 낮은 도: 0 / 바로 다음 음인 도#: 1 / 같은 음인 레b: 2 / 바로 다음 음인 레: 3 / 가장 높은 도: 68
    List<int> notes = new List<int>();

    public Chord()
    {
        notes.Clear();
    }

    public Chord(List<int> notes)
    {
        notes.Clear();
        foreach (int n in notes)
        {
            this.notes.Add(n);
        }
    }

    public Chord(int note1, int note2, int note3)
    {
        notes.Clear();
        notes.Add(note1);
        notes.Add(note2);
        notes.Add(note3);
    }

    public Chord(int note1, int note2, int note3, int note4)
    {
        notes.Clear();
        notes.Add(note1);
        notes.Add(note2);
        notes.Add(note3);
        notes.Add(note4);
    }

    public void AddNote(int note)
    {
        notes.Add(note);
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

    public void PlayChord()
    {
        foreach (int note in notes)
        {
            Manager.manager.Play(Note.NoteToMidi(note));
        }
    }
}

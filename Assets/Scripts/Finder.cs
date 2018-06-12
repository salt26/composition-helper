using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder : MonoBehaviour {

    static public Finder finder;
    public GameObject rhythmCaveatPanel;
    public GameObject chordPanel;
    public GameObject developingPanel;
    public GameObject savePanel;
    public GameObject instructionPanel;
    public GameObject instructionPanel2;
    
    private void Awake()
    {
        finder = this;
    }
}

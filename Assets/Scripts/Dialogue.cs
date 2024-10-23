using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DialogueState
{
    serio,
    normal,
    sorprendido,
    feliz,
    duda
}

[System.Serializable]
public class Dialogue
{
    
    [TextArea(3, 10)]
    public string[] sentences;
    public DialogueState[] states;
    
    public void SyncStates()
    {
        if (states == null || states.Length != sentences.Length)
        {
            states = new DialogueState[sentences.Length];
        }
    }
}

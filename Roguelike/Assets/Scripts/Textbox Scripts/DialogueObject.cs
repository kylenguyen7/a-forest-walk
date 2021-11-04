using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_", menuName = "DialogueObject", order = 0)]
public class DialogueObject : ScriptableObject
{
    public string speakerName;
    public Sprite portrait;
    public AudioClip talkingSFX;

    [TextArea]
    public string[] lines;

    public ResponseObject[] responses;
    public DialogueObject nextDialogue = null;
}

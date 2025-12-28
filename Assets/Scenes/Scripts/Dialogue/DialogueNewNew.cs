using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Entry", menuName = "Dialogue New New")]
public class DialogueNewNew : ScriptableObject
{
    public int level; 
    public List<DialogueEntryNew> dialogueEntries;
}

[System.Serializable]
public class DialogueEntryNew
{
    public string speakerName;
    [TextArea(6, 6)]
    public string dialogueText;
    public Sprite portrait;
}
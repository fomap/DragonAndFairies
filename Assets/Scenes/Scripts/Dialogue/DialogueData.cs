using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueGroup> dialogueGroups;
}

[System.Serializable]
public class DialogueGroup
{
    // public int triggerAtMoveCount; 
    public int triggerAtLevelIndex;
    public List<DialogueEntry> dialogueEntries;
}

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;
    [TextArea(6, 6)]
    public string dialogueText;
    public Sprite portrait;
}
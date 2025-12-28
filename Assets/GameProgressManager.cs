using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }
    
    private HashSet<int> _playedDialogues = new HashSet<int>();
    private const string PLAYED_DIALOGUES_KEY = "PlayedDialogues";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkDialogueAsPlayed(int levelIndex)
    {
        if (_playedDialogues.Contains(levelIndex)) return;
        
        _playedDialogues.Add(levelIndex);
        SaveProgress();
        Debug.Log($"Marked dialogue for level {levelIndex} as played");
    }

    public bool HasPlayedDialogue(int levelIndex)
    {
        return _playedDialogues.Contains(levelIndex);
    }

    public void ResetProgressForLevel(int levelIndex)
    {
        _playedDialogues.Remove(levelIndex);
        SaveProgress();
    }

    public void ResetAllProgress()
    {
        _playedDialogues.Clear();
        PlayerPrefs.DeleteKey(PLAYED_DIALOGUES_KEY);
        PlayerPrefs.Save();
        Debug.Log("All game progress reset.");
    }

    private void SaveProgress()
    {
        List<string> levelStrings = new List<string>();
        foreach (int level in _playedDialogues)
        {
            levelStrings.Add(level.ToString());
        }
        
        string savedString = string.Join(",", levelStrings);
        PlayerPrefs.SetString(PLAYED_DIALOGUES_KEY, savedString);
        PlayerPrefs.Save();
        Debug.Log($"Saved progress: {savedString}");
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey(PLAYED_DIALOGUES_KEY))
        {
            string savedString = PlayerPrefs.GetString(PLAYED_DIALOGUES_KEY);
            
            if (!string.IsNullOrEmpty(savedString))
            {
                string[] levelStrings = savedString.Split(',');
                foreach (string levelStr in levelStrings)
                {
                    if (int.TryParse(levelStr, out int level))
                    {
                        _playedDialogues.Add(level);
                    }
                }
            }
            Debug.Log($"Loaded {_playedDialogues.Count} played dialogues");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }
    

    private HashSet<string> _playedDialogues = new HashSet<string>();
    private const string PLAYED_PREFIX = "PlayedDialogue_";

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

    public void MarkDialogueAsPlayed(int levelIndex, int moveCount)
    {
        string key = $"{PLAYED_PREFIX}L{levelIndex}M{moveCount}";
        _playedDialogues.Add(key);
        SaveProgress();
    }

    public bool HasPlayedDialogue(int levelIndex, int moveCount)
    {
        string key = $"{PLAYED_PREFIX}L{levelIndex}M{moveCount}";
        return _playedDialogues.Contains(key);
    }

    public void ResetAllProgress()
    {
        _playedDialogues.Clear();
        PlayerPrefs.DeleteKey("GameProgress"); 
        Debug.Log("All game progress reset.");
    }

    private void SaveProgress()
    {
        List<string> playedList = new List<string>(_playedDialogues);
        string json = JsonUtility.ToJson(new SerializableList(playedList));
        PlayerPrefs.SetString("GameProgress", json);
        PlayerPrefs.Save();
    }



    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey("GameProgress"))
        {
            string json = PlayerPrefs.GetString("GameProgress");
            SerializableList list = JsonUtility.FromJson<SerializableList>(json);
            _playedDialogues = new HashSet<string>(list.items);
        }
    }


    [System.Serializable]
    private class SerializableList
    {
        public List<string> items;
        public SerializableList(List<string> list) { items = list; }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManagerV2 : MonoBehaviour
{
    public static DialogueManagerV2 Instance { get; private set; }
    
    [Header("Dialogue Settings")]
    [SerializeField] private bool playOnLevelStart = true;
    [SerializeField] private string dialogueResourcesPath = "Dialogues";
    
    public bool IsDialogueActive => dialogueCanvas.activeSelf;

    [Header("UI refs")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;

    [Header("Typewriter Effect")]
    [SerializeField] private float typewriterSpeed = 0.05f;

    private DialogueNewNew _currentDialogue;
    private List<DialogueEntryNew> _currentEntries;
    private int _currentEntryIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Coroutine dialogueCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!playOnLevelStart) return;

        _currentEntryIndex = 0;

        LoadAndPlayDialogueForLevel(scene.buildIndex);
    }

    private void LoadAndPlayDialogueForLevel(int levelIndex)
    {
        if (GameProgressManager.Instance.HasPlayedDialogue(levelIndex))
        {
            Debug.Log($"Dialogue for level {levelIndex} already played, skipping.");
            return;
        }

        string dialogueName = $"Dialogue_Level{levelIndex}";
        DialogueNewNew dialogue = Resources.Load<DialogueNewNew>($"{dialogueResourcesPath}/{dialogueName}");
        
        if (dialogue == null)
        {
            Debug.LogWarning($"No dialogue found for level {levelIndex} at path: {dialogueResourcesPath}/{dialogueName}");
            return;
        }

        if (dialogue.level != levelIndex)
        {
            Debug.LogWarning($"Dialogue level mismatch: Expected {levelIndex}, found {dialogue.level}");
            return;
        }

        _currentDialogue = dialogue;
        _currentEntries = dialogue.dialogueEntries;

        PlayDialogue();

        GameProgressManager.Instance.MarkDialogueAsPlayed(levelIndex);
    }

    public void PlayDialogue()
    {
        if (_currentEntries == null || _currentEntries.Count == 0)
        {
            Debug.LogWarning("No dialogue entries to play!");
            return;
        }

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }
        
        dialogueCoroutine = StartCoroutine(PlayDialogueSequence());
    }

    private IEnumerator PlayDialogueSequence()
    {
        dialogueCanvas.SetActive(true);
        GlobalSkyfallEventManager.Instance?.PauseGame();

        for (_currentEntryIndex = 0; _currentEntryIndex < _currentEntries.Count; _currentEntryIndex++)
        {
            DialogueEntryNew entry = _currentEntries[_currentEntryIndex];
            
            speakerName.text = entry.speakerName;
            portraitImage.sprite = entry.portrait;

            dialogueText.text = "";

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            
            typingCoroutine = StartCoroutine(TypeText(entry.dialogueText));

            bool lineFinished = false;
            while (!lineFinished)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    if (isTyping)
                    {
                        SkipTyping(entry.dialogueText);
                    }
                    else
                    {
                        lineFinished = true;
                    }
                }
                yield return null;
            }
        }

        OnDialogueFinished();
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }

        isTyping = false;
    }

    private void SkipTyping(string fullText)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialogueText.text = fullText;
        isTyping = false;
    }

    public void OnDialogueFinished()
    {
        Debug.Log("Dialogue finished");
        
        dialogueCanvas.SetActive(false);
        GlobalSkyfallEventManager.Instance?.ResumeGame();
        
        // Clean up
        _currentDialogue = null;
        _currentEntries = null;
        _currentEntryIndex = 0;
    }

    public void TriggerDialogueForLevel(int levelIndex)
    {
        LoadAndPlayDialogueForLevel(levelIndex);
    }

    public void ForcePlayDialogue(DialogueNewNew dialogue)
    {
        _currentDialogue = dialogue;
        _currentEntries = dialogue.dialogueEntries;
        PlayDialogue();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{

    public static Dialogue Instance { get; private set; }

    [SerializeField] private DialogueData dialogueData;
    private AsyncOperation asyncLoadOperation;
    private bool pendingLevelLoad = false;
    
     public bool IsDialogueActive => dialogueCanvas.activeSelf;


    [Header("UI refs")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;



    [Header("Typewriter Effect")]
    [SerializeField] private float typewriterSpeed = 0.05f;

    private bool isTyping = false;
    private Coroutine typingCoroutine;


    private void OnEnable()
    {
        GlobalSkyfallEventManager.OnBoxMoved += CheckForDialogueTrigger;
    }

    private void OnDisable()
    {
        GlobalSkyfallEventManager.OnBoxMoved -= CheckForDialogueTrigger;
    }




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
         //   DontDestroyOnLoad(dialogueCanvas);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator DisplayDialogueGroup(List<DialogueEntry> dialogueEntries)
    {
        foreach (DialogueEntry entry in dialogueEntries)
        {
            ShowDialogueEntry(entry);

           
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(entry.dialogueText));

         
            bool lineFinished = false;
            while (!lineFinished)
            {
                if (Input.GetKeyDown(KeyCode.Return))
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
    private void ShowDialogueEntry(DialogueEntry entry)
    {
        speakerName.text = entry.speakerName;
        dialogueText.text = entry.dialogueText;
        portraitImage.sprite = entry.portrait;

        dialogueCanvas.SetActive(true);
        GlobalSkyfallEventManager.Instance?.PauseGame();
    }


    public void OnDialogueFinished()
    {
        dialogueCanvas.SetActive(false);
        
        GlobalSkyfallEventManager.Instance?.ResumeGame();
        
        
        if (pendingLevelLoad)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndex + 1);
            }
            else
            {
                Debug.Log("No more scenes to load.");
            }
            pendingLevelLoad = false;
        }


    }

    public void QueueLevelLoad()
    {
        pendingLevelLoad = true;
    }

    private void CheckForDialogueTrigger(int currentCount)
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        
      
        if (GameProgressManager.Instance.HasPlayedDialogue(currentLevelIndex, currentCount))
        {
            Debug.Log("Dialogue already played, skipping.");
            return;
        }

        DialogueGroup targetGroup = dialogueData.dialogueGroups
            .Find(group => group.triggerAtMoveCount == currentCount);

        if (targetGroup != null)
        {
            Debug.Log($"Found and playing dialogue for count {currentCount}");
            GameProgressManager.Instance.MarkDialogueAsPlayed(currentLevelIndex, currentCount);
            StartCoroutine(DisplayDialogueGroup(targetGroup.dialogueEntries));
        }
    }



}
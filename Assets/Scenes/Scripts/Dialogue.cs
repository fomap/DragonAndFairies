using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;

    [Header("Dialogue content")]
    [SerializeField] private string[] speaker;
    [TextArea]
    [SerializeField] private string[] dialogueWords;
    [SerializeField] private Sprite[] portraits;

    
    [Header("Typewriter Effect")]
    [SerializeField] private float typewriterSpeed = 0.05f; 
    
    private int currentIndex;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (HasDialogueContent())
        {
            ShowDialogue(0);
        }
        else
        {
            Debug.LogWarning("Dialogue arrays are empty!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            if (isTyping)
            {
               
                SkipTyping();
            }
            else if (currentIndex < dialogueWords.Length - 1)
            {
                currentIndex++;
                ShowDialogue(currentIndex);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            if (isTyping)
            {
                
                SkipTyping();
            }
            else if (currentIndex > 0)
            {
                currentIndex--;
                ShowDialogue(currentIndex);
            }
        }
    }

    private bool HasDialogueContent()
    {
        return speaker.Length > 0 && dialogueWords.Length > 0 && portraits.Length > 0;
    }

    private void ShowDialogue(int index)
    {
        speakerText.text = speaker[index];
        portraitImage.sprite = portraits[index];
        currentIndex = index;
        
        // typewriter effect
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(dialogueWords[index]));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        
        isTyping = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialogueText.text = dialogueWords[currentIndex];
        isTyping = false;
    }

    private void EndDialogue()
    {
        dialogueCanvas.SetActive(false);
    }
}
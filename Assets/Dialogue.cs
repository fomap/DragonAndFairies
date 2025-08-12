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

    [Header("StartGame")]
    [SerializeField] private GameObject dragon;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject box;
    
    private int currentIndex;

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

        // dragon.SetActive(false);
        // player.SetActive(false);
        // box.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            if (currentIndex < dialogueWords.Length - 1)
            {
                currentIndex++;
                ShowDialogue(currentIndex);
            }
            else
            {
               // EndDialogue();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            if (currentIndex > 0)
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
        dialogueText.text = dialogueWords[index];
        portraitImage.sprite = portraits[index];
        currentIndex = index;
    }

    private void EndDialogue()
    {
        dialogueCanvas.SetActive(false);
        // dragon.SetActive(true);
        // player.SetActive(true);
        // box.SetActive(true);
    }
}
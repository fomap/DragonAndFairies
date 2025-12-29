using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
     [SerializeField] GameObject pauseBtn;
      [SerializeField] GameObject  menuPanel;

    private bool isPaused = false;

    private void Update()
    {
    // if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame) 
    // {
    //     if (isPaused) Resume();
    //     else Pause();

    //     isPaused = !isPaused;
    // }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        GlobalSkyfallEventManager.Instance?.PauseGame();
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GlobalSkyfallEventManager.Instance?.ResumeGame();
    }


    public void Home()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        pauseBtn.SetActive(false);
        menuPanel.SetActive(true);
         GlobalSkyfallEventManager.Instance?.ResumeGame();
        SceneManager.LoadScene(0);
    }
}
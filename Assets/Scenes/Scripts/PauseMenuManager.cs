using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    private bool isPaused = false;

    private void Update()
    {
    if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame) 
    {
        if (isPaused) Resume();
        else Pause();

        isPaused = !isPaused;
    }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        GlobalSkyfallEventManager.Instance?.PauseGame();
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        GlobalSkyfallEventManager.Instance?.ResumeGame();
    }


    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
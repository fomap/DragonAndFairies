using System;
using UnityEngine;

public class GlobalSkyfallEventManager : MonoBehaviour
{
    public static GlobalSkyfallEventManager Instance { get; private set; }

    // Global events
    public static event Action OnAnyObjectStartFalling;
    public static event Action OnAnyObjectStopFalling;
    public static event Action OnFeyStartMoving;
    public static event Action OnFeyStopMoving;

    // New pause events
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    private int currentlyFallingObjects = 0;
    private bool isGamePaused = false;


    public static Action<int> OnBoxMoved;
    public int BoxMoveCount { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BoxMoveCount = 0;
            Debug.Log("GlobalSkyfallEventManager initialized");
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Duplicate GlobalSkyfallEventManager destroyed");
            Destroy(gameObject);
        }
    }

    public void NotifyStartFalling()
    {
        if (isGamePaused) return;
        currentlyFallingObjects++;
        if (currentlyFallingObjects == 1)
        {
            OnAnyObjectStartFalling?.Invoke();
        }
    }

    public void NotifyStopFalling()
    {
        if (isGamePaused) return;
        currentlyFallingObjects = Mathf.Max(0, currentlyFallingObjects - 1);
        if (currentlyFallingObjects == 0)
        {
            OnAnyObjectStopFalling?.Invoke();
        }
    }

    public void NotifyFeyStartMoving()
    {
        if (!isGamePaused) OnFeyStartMoving?.Invoke();
    }

    public void NotifyFeyStopMoving()
    {
        if (!isGamePaused) OnFeyStopMoving?.Invoke();
    }
    public static bool IsGamePaused => Instance != null && Instance.isGamePaused;

    public void TogglePause()
    {
        if (isGamePaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isGamePaused) return;

        isGamePaused = true;
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!isGamePaused) return;

        isGamePaused = false;
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();

        Debug.Log("Game Resumed");
    }


    public void IncrementBoxMoveCount()
    {
        BoxMoveCount++;
        Debug.Log($"Box move count: {BoxMoveCount}");
        OnBoxMoved?.Invoke(BoxMoveCount);
    }

}



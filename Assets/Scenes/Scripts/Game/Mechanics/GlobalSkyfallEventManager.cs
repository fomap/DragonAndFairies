using System;
using UnityEngine;

public class GlobalSkyfallEventManager : MonoBehaviour
{
    public static GlobalSkyfallEventManager Instance;

    // Global events
    public static event Action OnAnyObjectStartFalling;
    public static event Action OnAnyObjectStopFalling;
    public static event Action OnFeyStartMoving;
    public static event Action OnFeyStopMoving;

    private int currentlyFallingObjects = 0;


    [SerializeField] private PlayerOneMovement dragonController; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GlobalSkyfallEventManager initialized");
        }
        else
        {
            Debug.LogWarning("Duplicate GlobalSkyfallEventManager destroyed");
            Destroy(gameObject);
        }
    }

    public void NotifyStartFalling()
    {
        currentlyFallingObjects++;
        if (currentlyFallingObjects == 1)
        {
            OnAnyObjectStartFalling?.Invoke();
        }
    }

    public void NotifyStopFalling()
    {
        currentlyFallingObjects = Mathf.Max(0, currentlyFallingObjects - 1);
        if (currentlyFallingObjects == 0)
        {
            OnAnyObjectStopFalling?.Invoke();
        }
    }

    public void NotifyFeyStartMoving() => OnFeyStartMoving?.Invoke();
    public void NotifyFeyStopMoving() => OnFeyStopMoving?.Invoke();
    


}



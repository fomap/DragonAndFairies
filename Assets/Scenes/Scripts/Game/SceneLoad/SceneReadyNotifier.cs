using System;
using UnityEngine;

public class SceneReadyNotifier : MonoBehaviour
{
    public static SceneReadyNotifier Instance { get; private set; }

    public static event Action OnSceneReady;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void NotifySceneReady()
    {
        OnSceneReady?.Invoke();
    }
}
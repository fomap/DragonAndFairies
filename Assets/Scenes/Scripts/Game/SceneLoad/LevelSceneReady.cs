using System.Collections;
using UnityEngine;

public class LevelSceneReady : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 0f;

    private void Start()
    {
        StartCoroutine(NotifyWhenReady());
    }

    private IEnumerator NotifyWhenReady()
    {
        yield return null;
        if (delaySeconds > 0f)
            yield return new WaitForSeconds(delaySeconds);
        SceneReadyNotifier.NotifySceneReady();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioSource bgMusic;
    // Start is called before the first frame update
    void Start()
    {
        bgMusic.Play();
    }

}

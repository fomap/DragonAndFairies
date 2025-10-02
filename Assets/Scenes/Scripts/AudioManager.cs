using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    // Start is called before the first frame update





    private void Start()
    {
        // if
        SetMusicVolume();
    }

    public void SetMusicVolume()
    {
        //float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(10)*20);
    }

}

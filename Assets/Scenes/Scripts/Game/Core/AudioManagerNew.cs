using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManagerNew : MonoBehaviour
{
    public static AudioManagerNew Instance { get; private set; }

    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private LevelMusic[] levelMusicTracks;

    [System.Serializable]
    public class LevelMusic
    {
        public int levelBuildIndex;
        public AudioClip musicClip;
    }

    private const string MusicVolumeKey = "MusicVolume";

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);


            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        //musicSlider = FindAnyObjectByType<Slider>();
    }

    private void Start()
    {
       
        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
            musicSlider.value = savedVolume;
            SetMusicVolume(savedVolume);
        }
        else
        {
          
            SetMusicVolume(musicSlider.value);
        }

    
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= 0.0001f) 
        {
            myMixer.SetFloat("music", -80f); 
        }
        else
        {

            float dB = Mathf.Log10(volume) * 20f;
            myMixer.SetFloat("music", dB);
        }
        
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        LevelMusic levelMusic = System.Array.Find(levelMusicTracks, x => x.levelBuildIndex == scene.buildIndex);
        
        if (levelMusic != null && levelMusic.musicClip != bgMusic.clip)
        {
           
            bgMusic.clip = levelMusic.musicClip;
            bgMusic.Play();
        }
    }

    private void OnDestroy()
    {
      
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

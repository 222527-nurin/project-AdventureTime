using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicBackground : MonoBehaviour
{
    public static MusicBackground instance;

    public AudioSource sfxSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    private const string MUSIC_KEY = "MusicMute";
    private const string SFX_KEY = "SFXMute";

    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ApplySavedSettings();
    }

    void ApplySavedSettings()
    {
        bool musicMuted = PlayerPrefs.GetInt(MUSIC_KEY, 0) == 1;
        bool sfxMuted = PlayerPrefs.GetInt(SFX_KEY, 0) == 1;

        SetMusicMute(musicMuted);
        SetSFXMute(sfxMuted);
    }

    // ---------------- MUSIC ----------------
    public void SetMusicMute(bool isMuted)
    {
        audioMixer.SetFloat("MusicVol", isMuted ? -80f : 0f);

        PlayerPrefs.SetInt(MUSIC_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ---------------- SFX ----------------
    public void SetSFXMute(bool isMuted)
    {
        audioMixer.SetFloat("SFXVol", isMuted ? -80f : 0f);

        PlayerPrefs.SetInt(SFX_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
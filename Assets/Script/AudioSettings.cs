using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;

    // Called by UI Toggle (true = mute ON)
    public void ToggleMusic(bool isMuted)
    {
        if (isMuted)
            audioMixer.SetFloat("MusicVol", -80f); // effectively mute
        else
            audioMixer.SetFloat("MusicVol", 0f);   // normal volume
    }

    public void ToggleSFX(bool isMuted)
    {
        if (isMuted)
            audioMixer.SetFloat("SFXVol", -80f);
        else
            audioMixer.SetFloat("SFXVol", 0f);
    }

    // Optional: slider control
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
    }
}
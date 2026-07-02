using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    public void ToggleMusic(bool isMuted)
    {
        if (MusicBackground.instance == null)
        {
            Debug.LogError("MusicBackground is missing in scene!");
            return;
        }

        MusicBackground.instance.SetMusicMute(isMuted);
    }

    public void ToggleSFX(bool isMuted)
    {
        if (MusicBackground.instance == null)
        {
            Debug.LogError("MusicBackground is missing in scene!");
            return;
        }

        MusicBackground.instance.SetSFXMute(isMuted);
    }

}
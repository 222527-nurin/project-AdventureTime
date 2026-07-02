using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBackground : MonoBehaviour
{
    // This allows other scripts to access this class easily
    public static MusicBackground instance;

    // Removing 'static' here lets it show up in the Inspector!
    public AudioSource sfxSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

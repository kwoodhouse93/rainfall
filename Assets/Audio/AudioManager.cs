using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource rainAudioSource;
    [SerializeField] private SoundCollection rainSounds;

    #region singleton
    public static AudioManager Instance => instance;
    private static AudioManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Object.Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion singleton

    public void PlayRainSound()
    {
        rainSounds.PlayRandomSound(rainAudioSource);
    }
}

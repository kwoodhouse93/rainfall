using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sounds", menuName = "Audio/SoundCollection")]
public class SoundCollection : ScriptableObject
{
    [SerializeField] private List<AudioClip> sounds;
    [SerializeField] private float volume;

    public void PlayRandomSound(AudioSource audioSource)
    {
        var audioClip = sounds[Random.Range(0, sounds.Count)];
        audioSource.PlayOneShot(audioClip, volume);
    }
}

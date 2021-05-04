using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(AudioLowPassFilter))]
public class SeaNoise : MonoBehaviour
{
    // Rate of change for pitch modulation.
    [SerializeField] private float pitchFreq;
    // Amount of pitch modulation (added and subtracted to starting pitch set on audio source).
    [SerializeField] private float pitchAmount;
    // Min and Max specify the range of cutoff frequencies to use for the low pass filter.
    [SerializeField] private float filterMinCutoff;
    [SerializeField] private float filterMaxCutoff;
    // Rate of change for how much the fade out effect is applied.
    [SerializeField] private float fadeOutDurFreq;
    // Proportion through clip from which to start fading out.
    [SerializeField] private float fadeOutMaxDur;
    // Maximum amount to reduce the volume by (0 to 1).
    [SerializeField] private float fadeOutAmount;

    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;
    private float startPitch;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();

        startPitch = audioSource.pitch;
    }

    void Update()
    {
        // Shift the pitch gradually over time. Intended to be used with quite small values
        // for pitchFreq and pitchAdjust, just to add a bit more movement.
        // Arbitrary choice of cosine over sine.
        float pitchAdjust = Mathf.Cos(Time.time * pitchFreq) * pitchAmount;
        audioSource.pitch = startPitch + pitchAdjust;

        // Playback01 is how far through the looping clip we are at any given moment
        // Counting samples makes it easier to account for our constantly changing pitch
        // than if we were to try and use time in seconds.
        float playback01 = audioSource.timeSamples / (float)audioSource.clip.samples;

        // We use sin of playback01 * pi, which forms an arc starting at 0 when playback01 is 0,
        // up to 1 when playback01 is 0.5, and back down to 1 when playback01 is 1.
        // Scale and translate to fit the min/max cutoff range we defined.
        //
        // The curve looks a bit like this (where t = playback01):
        //
        // filterMaxCutoff ^        ____
        //                 |     __/    \__
        //                 |   _/          \_
        //                 |  /              \
        //                 | /                \
        //                 |/                  \
        // filterMinCutoff -----------------------> t
        //                 0                   1
        //
        float filterCutoff = (Mathf.Sin(playback01 * Mathf.PI) * (filterMaxCutoff - filterMinCutoff)) + filterMinCutoff;
        lowPassFilter.cutoffFrequency = filterCutoff;

        // Finally, scale the volume similar to filterCutoff, but with extra math to adjust
        // what portion of the cutoff happens where during the loop.
        // The loop already has a ~1 second fade out. We just apply extra scaling, starting as early
        // as fadeOutMaxDur from the end of the clip
        //
        // Say this is the amplitude profile of the clip:
        //
        // 1 ^     _____
        //   |    /     \
        //   |   /       \
        //   |  /         \
        //   | /           \
        //   |/             \
        // 0 -----------------------> t
        //
        // We multiply it by a quarter period of a sine wave that looks a bit like this:
        //
        // 1 ^ _____
        //   |     .\___
        //   |     .    \__
        //   |     .       \_
        //   |     .         \
        //   |     .          \
        // 0 -----------------------> t
        //         .
        //         .
        //        (t = 1 - fadeOutMaxDir)
        //
        // The effect is to reshape the fade out to last longer and have a lower amplitude.
        //
        // Then by varying how much we apply the fade out modulation, we get different
        // durations of the fade out effect. Should give more movement and a more organic
        // feel.
        if (playback01 < 1 - fadeOutMaxDur)
        {
            audioSource.volume = 1;
        }
        else
        {
            // Just guard against potential divide by 0 later on.
            if (fadeOutAmount == 0)
            {
                audioSource.volume = 1;
                return;
            }

            // Linearly scale playback01, so scaledPlayback is now from 0.5 to 1 when playback01
            // is from (1-fadeOutMaxDur) to 1.
            //
            // Now our freq will move from 0.5pi to pi over the period from fadeOutMaxDur to the
            // end of the AudioClip.
            float pbMin = 1 - fadeOutMaxDur;
            float scaledPlayback = ((playback01 - pbMin) / (1 - pbMin)) * (1 - 0.5f) + 0.5f;
            float freq = scaledPlayback * Mathf.PI;

            // This part of the Sin curve outputs from 1 to 0 smoothly.
            float envelope = Mathf.Sin(freq);

            // Now modulate how much we apply our envelope over time with a low frequency sinusoid
            // Again, choice of cosine is pretty arbitrary.
            //
            // I guess it might reduce weird effects if all our waves start with the same phase?
            // In this case, it's also 1 at t=0, so if we forget to set a fadeOutDurFreq, we'll just
            // have volume = 1 all the time, which is nicer than it snapping to volume = 0.5 as soon
            // as we go past (1-fadeOutMaxDur).
            float envelopeMix = (Mathf.Cos(Time.time * fadeOutDurFreq) + 1) / 2f;

            // Aaaand the final step, multiply lerp between our envelope and 1 by the envelope mix
            float modulated = Mathf.Lerp(1, envelope, envelopeMix);

            // And one last scaling to make sure we never reduce the volume below fadeOutAmount.
            float minVol = 1 - fadeOutAmount;
            float volume = modulated * (1 - minVol) + minVol;

            // Debug.Log("playback01: " + playback01 + ", envelope: " + envelope + ", envelopeMix: " + envelopeMix + ", volume:" + volume);
            audioSource.volume = volume;
        }
    }
}

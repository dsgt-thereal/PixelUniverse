using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class TimeBasedAudioSwitcher : MonoBehaviour
{
    public AudioClip dayAudio;   // 6am - 6pm
    public AudioClip nightAudio; // 6pm - 6am

    private AudioSource audioSource;
    private bool isDaytime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateAudio();
        InvokeRepeating(nameof(UpdateAudio), 60f, 60f); // Check every minute
    }

    void UpdateAudio()
    {
        int hour = DateTime.Now.Hour;
        bool currentIsDaytime = hour >= 6 && hour < 18;

        if (currentIsDaytime != isDaytime || !audioSource.isPlaying)
        {
            isDaytime = currentIsDaytime;
            audioSource.clip = isDaytime ? dayAudio : nightAudio;
            audioSource.Play();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
    private static AudioSource audioSource;
    private bool muteState;
    private float preVolume;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(audioSource);
        audioSource.volume = 0.5f;
        muteState = false;
        preVolume = audioSource.volume;
    }

    public void VolumeChanged(float newVolume)
    {
        audioSource.volume = newVolume;
        muteState = false;
    }

    public void MuteClick()
    {
        muteState = !muteState;
        if (muteState)
        {
            preVolume = audioSource.volume;
            audioSource.volume = 0;
        }
        else
            audioSource.volume = preVolume;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

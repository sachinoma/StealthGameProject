using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicRangeAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Tag.SoundChecker)
        {
            AudioPlay();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == Tag.SoundChecker)
        {
            AudioStop();
        }
    }

    private void AudioPlay()
    {
        _audioSource.Play();
    }

    private void AudioStop()
    {
        _audioSource.Stop();
    }
}

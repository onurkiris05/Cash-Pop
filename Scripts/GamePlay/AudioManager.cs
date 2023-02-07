using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip hitWallSFX, hitBoundsSFX, buttonSFX;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayHitWallSFX()
    {
        _audioSource.PlayOneShot(hitWallSFX);
    }

    public void PlayHitBoundSFX()
    {
        _audioSource.PlayOneShot(hitBoundsSFX);
    }

    public void PlayButtonSFX()
    {
        _audioSource.PlayOneShot(buttonSFX);
    }

}

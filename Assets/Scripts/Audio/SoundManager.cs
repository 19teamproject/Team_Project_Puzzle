using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{

    [SerializeField] private AudioClip musicClip;
    [SerializeField][Range(0f, 1f)] private float musicVolume;

    private AudioSource audioSource;
    
    protected override void Awake()
    {
        base.Awake();
        
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = musicVolume;
        audioSource.loop = true;
    }

    private void Start()
    {
        if (musicClip != null) ChangeBackGroundMusic(musicClip);
    }

    public void ChangeBackGroundMusic(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        audioSource.PlayOneShot(clip, volume);
    }
}


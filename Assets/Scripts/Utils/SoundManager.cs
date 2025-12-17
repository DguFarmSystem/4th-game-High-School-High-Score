using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _gaugeSource;

    public void PlayBGM(AudioClip clip)
    {
        _bgmSource.clip = clip;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void PlayStoppableSFX(AudioClip clip)
    {
        _sfxSource.clip = clip;
        _sfxSource.Play();
    }

    public void StopStoppableSFX()
    {
        _sfxSource.Stop();
    }

    
    public void PlayGaugeSound(AudioClip clip)
    {
        _gaugeSource.clip = clip;
        _gaugeSource.loop = true;
        _gaugeSource.Play();
    }

    public void StopGaugeSound()
    {
        _gaugeSource.Stop();
    }
    
}

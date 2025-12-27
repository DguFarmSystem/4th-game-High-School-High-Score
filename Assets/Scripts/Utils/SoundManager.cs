using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _gaugeSource;

    public AudioSource BGMSource => _bgmSource;
    public AudioSource SFXSource => _sfxSource;
    public AudioSource GaugeSource => _gaugeSource;

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

    public void PlayStoppableSFX(AudioClip clip, float volume = 0.1f)
    {
        _sfxSource.clip = clip;
        _sfxSource.volume = volume;
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

    public void PlaySFXWithFadeOut(AudioClip clip, float fadeOutDuration)
    {
        StartCoroutine(PlaySFXWithFadeOutCoroutine(clip, fadeOutDuration));
    }

    private IEnumerator PlaySFXWithFadeOutCoroutine(AudioClip clip, float fadeOutDuration)
    {
        // 임시 AudioSource 생성 (다른 효과음에 영향 없음)
        GameObject tempObj = new GameObject("TempFadeOutAudio");
        tempObj.transform.SetParent(transform);
        AudioSource tempSource = tempObj.AddComponent<AudioSource>();
        
        tempSource.clip = clip;
        tempSource.volume = 0.1f;
        tempSource.Play();

        // 페이드아웃
        float elapsed = 0f;
        float startVolume = tempSource.volume;

        while (elapsed < fadeOutDuration && tempSource != null)
        {
            elapsed += Time.deltaTime;
            tempSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        // 임시 AudioSource 제거
        if (tempObj != null)
        {
            Destroy(tempObj);
        }
    }
    
}

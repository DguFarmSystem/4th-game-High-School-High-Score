using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelTouchHandler : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public enum AudioPlayState
    {
        NotPlayed,
        Playing,
        Paused
    }

    [SerializeField]
    private AudioSource SoundSource;
    [SerializeField] 
    private AudioClip GuitarClip;
    [SerializeField]
    private float playDuration = 1.0f;
    private int GuitarPlayCount = 0;

    private Coroutine pauseCoroutine;
    private AudioPlayState audioState = AudioPlayState.NotPlayed;

    void Start()
    {
        //음원 초기 세팅
        SoundSource.clip = GuitarClip;
        SoundSource.time = 52.0f;
        SoundSource.volume = 0.7f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("터치가 패널 영역에 진입");
        ++GuitarPlayCount;
        PlayOrResume();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("터치가 패널 영역에서 이탈");
        ++GuitarPlayCount;
        PlayOrResume();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("패널 위에서 터치 시작");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("패널 위에서 터치 종료");
    }

    private void PlayOrResume()
    {
        switch (audioState)
        {
            case AudioPlayState.NotPlayed:
                SoundSource.Play();
                audioState = AudioPlayState.Playing;
                break;

            case AudioPlayState.Paused:
                SoundSource.UnPause();
                audioState = AudioPlayState.Playing;
                break;

            case AudioPlayState.Playing:
                // 이미 재생 중이면 아무 것도 하지 않음
                break;
        }

        // Pause 타이머 갱신
        if (pauseCoroutine != null)
            StopCoroutine(pauseCoroutine);

        pauseCoroutine = StartCoroutine(PauseAfterDelay());
    }

    private IEnumerator PauseAfterDelay()
    {
        yield return new WaitForSeconds(playDuration);
        SoundSource.Pause();
        audioState = AudioPlayState.Paused;
        pauseCoroutine = null;
    }

    public int GetPlayCount()
    {
        return GuitarPlayCount;
    }
}

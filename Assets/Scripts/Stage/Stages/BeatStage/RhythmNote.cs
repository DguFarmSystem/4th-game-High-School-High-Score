using UnityEngine;

// [수정] Bass 타입을 여기에 정의합니다.
public enum NoteType { Hi, Snare, Bass }

public class RhythmNote : MonoBehaviour
{
    public NoteType type;
    [HideInInspector] public BeatStageManager manager;

    private bool isProcessed = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isProcessed) return;

        if (collision.CompareTag("Bar"))
        {
            if (type == NoteType.Bass)
            {
                manager.PlayBass();
                OnHit(); // 즉시 처리 완료
            }
            // Hi, Snare는 입력 대기열에 등록 (플레이어가 터치해야 함)
            else
            {
                manager.RegisterHittableNote(this);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (isProcessed) return;

        // Bass는 Enter에서 처리되므로 Exit에서 무시
        if (type == NoteType.Bass) return;

        if (collision.CompareTag("Bar"))
        {
            manager.UnregisterNote(this, true); 
        }
    }

    public void OnHit()
    {
        isProcessed = true;
        gameObject.SetActive(false);
    }
}
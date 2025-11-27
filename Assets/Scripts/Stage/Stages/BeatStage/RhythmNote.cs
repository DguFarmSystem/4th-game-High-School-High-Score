using UnityEngine;

public enum NoteType { Hi, Snare }

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
            manager.RegisterHittableNote(this);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (isProcessed) return;

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Stage;

public class DanceButton : MonoBehaviour
{
    private SpriteRenderer selectedFX;
    private bool isCorrectBtn = false;
    private static bool isTapped = false;

    private MusicDanceStage _musicDanceStage;

    public void SetCorrect() => isCorrectBtn = true;

    [SerializeField] private AudioClip _tapSFX;

    public void OnTap(InputAction.CallbackContext context)
    {
        if (_musicDanceStage.TimerTime <= 0f) return;

        if (InputManager.Instance.TouchedCollider != null && 
            InputManager.Instance.TouchedCollider.gameObject == this.gameObject &&
            !isTapped)
        {
            SoundManager.Instance.PlaySFX(_tapSFX);
            isTapped = true;
            selectedFX.enabled = true;
            if (isCorrectBtn) _musicDanceStage.SetStageClear();
            else {};

            DanceButton[] allButtons = FindObjectsOfType<DanceButton>();
            foreach (DanceButton button in allButtons)
            {
                InputManager.Instance._tapAction.performed -= button.OnTap;
            }
        }
    }

    //============= Lifecycle methods =============//

    void Awake()
    {
        _musicDanceStage = FindObjectOfType<MusicDanceStage>();

        selectedFX = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    void Start()
    {

        if (InputManager.Instance != null)
        {
            InputManager.Instance._tapAction.performed += OnTap;
        }

        isTapped = false;
    }

    void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance._tapAction != null)
        {
            InputManager.Instance._tapAction.performed -= OnTap;
        }
    }
}

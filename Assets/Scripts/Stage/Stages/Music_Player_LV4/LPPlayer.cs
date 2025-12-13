using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LPPlayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _lpPlayerIndicator1;
    [SerializeField] private SpriteRenderer _lpPlayerIndicator2;

    [Space(10)]

    [SerializeField] private Collider2D _lpPlayerCollider1;
    [SerializeField] private Collider2D _lpPlayerCollider2;

    [Space(10)]

    [SerializeField] private Sprite _lpInsertedSprite;
    [SerializeField] private Sprite _lpActivatedSprite;

    private SpriteRenderer _spriteRenderer;

    private Music_Player_LV4 _stage;

    public void OnTap(InputAction.CallbackContext context)
    {
        Collider2D hit = Physics2D.OverlapPoint(InputManager.Instance.TouchWorldPos);

        switch(_stage.ClearConditionsQ.Peek())
        {
            case Music_Player_LV4.StageClearConditions.InsertLPCase:
                if (hit == _lpPlayerCollider1)
                {
                    _stage.ClearConditionsQ.Dequeue();
                    _lpPlayerIndicator1.enabled = false;
                    _spriteRenderer.sprite = _lpInsertedSprite;
                }
                break;

            case Music_Player_LV4.StageClearConditions.ActivateLPPlayer:
                if (hit == _lpPlayerCollider2)
                {
                    _stage.ClearConditionsQ.Dequeue();
                    _spriteRenderer.sprite = _lpActivatedSprite;
                    _lpPlayerIndicator2.enabled = false;

                    InputManager.Instance._tapAction.performed -= OnTap;
                }
                break;
        }
    }

    void Start()
    {
        _stage = FindObjectOfType<Music_Player_LV4>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (InputManager.Instance != null)
        {
            InputManager.Instance._tapAction.performed += OnTap;
        }
    }

    void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance._tapAction != null)
        {
            InputManager.Instance._tapAction.performed -= OnTap;
        }
    }
}

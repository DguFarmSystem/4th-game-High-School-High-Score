using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind_Handle : MonoBehaviour
{
    [SerializeField] private Sprite _DefaultSprite;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HandleAnimation());
    }

    private IEnumerator HandleAnimation()
    {
        yield return new WaitForSeconds(2.3f);
        GetComponent<SpriteRenderer>().sprite = _DefaultSprite;
    }
}

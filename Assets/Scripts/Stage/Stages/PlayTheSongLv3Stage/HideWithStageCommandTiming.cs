using System.Collections;
using UnityEngine;

public class HideWithStageCommandTiming : MonoBehaviour
{
    [SerializeField] float totalVisibleTime = 1.8f; // 0.5 + 1.3

    void Start()
    {
        StartCoroutine(HideAfter());
    }

    IEnumerator HideAfter()
    {
        yield return new WaitForSeconds(totalVisibleTime);
        gameObject.SetActive(false);
    }
}
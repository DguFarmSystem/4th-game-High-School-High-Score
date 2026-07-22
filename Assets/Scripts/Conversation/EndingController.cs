using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    public GameObject EndingCanvas;
    public Image FadeOutPanel;
    public GameObject Continue;

    public void SetEnding()
    {
        EndingCanvas.SetActive(true);
        StartCoroutine(FadeOutRoutine(FadeOutPanel, 0.7f));
    }

    IEnumerator FadeOutRoutine(Image img, float duration)
    {
        float time = 0f;
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        while (time < duration)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, time / duration);
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;

        Continue.SetActive(true);
    }
}

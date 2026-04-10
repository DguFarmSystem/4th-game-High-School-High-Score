using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clicker : MonoBehaviour
{
    public Text countText;
    public Button myButton;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public int countNum = 0;

    void Start()
    {
        myButton.onClick.AddListener(CountUp);
        myButton.onClick.AddListener(ClickSound);
    }

    public void CountUp()
    {
        countNum++;
        countText.text = countNum.ToString("D5");
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
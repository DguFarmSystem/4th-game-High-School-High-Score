using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clicker : MonoBehaviour
{
    public Text countText;
    public Button myButton;
    public int countNum = 0;

    void Start()
    {
        myButton.onClick.AddListener(CountUp);
    }

    public void CountUp()
    {
        countNum++;
        countText.text = countNum.ToString("D5");
    }
}
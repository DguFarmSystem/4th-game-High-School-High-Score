using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseResources : MonoBehaviour
{
    //���̵��� ������������ �ʿ��� ���ҽ��� �����ϴ� �ڵ�
    public Sprite[] backgrounds;                   //��� �̹��� ����
    public Sprite[] Erasers;                       //���찳 �̹��� ����
    [SerializeField] private Image image;          //������ ��� �̹����� ���� �̹��� ������Ʈ
    [SerializeField] private Text InfoText;
    //��׶��� �̹��� ������ ���� �����ư��鼭 ��µǵ��� �ϴ� static ����(���� ����)
    //static private int ImageNum = 0;
    //Start is called before the first frame update

    void Start()
    {
        int randIndex = Random.Range(0, backgrounds.Length);
        if(randIndex == 0)
        {
            InfoText.color = Color.white;
        }
        else if (randIndex == 1)
        {
            InfoText.color = Color.black;
        }
        else if (randIndex == 2)
        {
            InfoText.color = Color.cyan;
        }
        image.sprite = backgrounds[randIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

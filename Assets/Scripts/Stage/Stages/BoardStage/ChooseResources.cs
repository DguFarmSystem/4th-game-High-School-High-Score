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
    //��׶��� �̹��� ������ ���� �����ư��鼭 ��µǵ��� �ϴ� static ����(���� ����)
    //static private int ImageNum = 0;
    //Start is called before the first frame update

    void Start()
    {
        int randIndex = Random.Range(0, backgrounds.Length);
        image.sprite = backgrounds[randIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

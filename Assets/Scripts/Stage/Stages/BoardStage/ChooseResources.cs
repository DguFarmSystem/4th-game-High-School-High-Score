using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseResources : MonoBehaviour
{
    //난이도별 스테이지에서 필요한 리소스를 지정하는 코드
    public Sprite[] backgrounds;                   //배경 이미지 설정
    public Sprite[] Erasers;                       //지우개 이미지 설정
    [SerializeField] private Image image;          //선택한 배경 이미지를 넣을 이미지 오브젝트
    //백그라운드 이미지 개수에 따라 번갈아가면서 출력되도록 하는 static 변수(구현 예정)
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

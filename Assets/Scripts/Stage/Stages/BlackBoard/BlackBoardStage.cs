using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

/*
 * 스테이지 설계
 * 변수 선언 : 스테이지 변수, 스테이지 지속 시간 확인 변수
 * 스테이지 시작 : 지워야할 게임오브젝트에 스테이지에 맞는 이미지 부여
 * 스테이지 도중 : 지워야할 게임오브젝트가 얼마나 지워졌는지 확인
 */
public class BlackBoardStage : StageNormal
{
    //현재 스테이지 레벨
    [SerializeField] private int currentLevel = 0;
    //스테이지 제한 시간
    [SerializeField] private int timeLimit = 0;
    //gameobjects
    //터치 시 나타날 지우개 오브젝트
    [SerializeField] private GameObject BoardEraser;
    //지워야할 스프라이트 게임오브젝트
    //[SerializeField] private GameObject Eraserable;
    //Test Object
    public GameObject TestSphere;
    //inputManager 객체 지정을 통해 좌표 획득
    [SerializeField] private InputManager IManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

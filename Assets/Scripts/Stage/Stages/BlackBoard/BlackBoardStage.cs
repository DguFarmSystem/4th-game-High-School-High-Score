using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

/*
 * �������� ����
 * ���� ���� : �������� ����, �������� ���� �ð� Ȯ�� ����
 * �������� ���� : �������� ���ӿ�����Ʈ�� ���������� �´� �̹��� �ο�
 * �������� ���� : �������� ���ӿ�����Ʈ�� �󸶳� ���������� Ȯ��
 */
public class BlackBoardStage : StageNormal
{
    //���� �������� ����
    [SerializeField] private int currentLevel = 0;
    //�������� ���� �ð�
    [SerializeField] private int timeLimit = 0;
    //gameobjects
    //��ġ �� ��Ÿ�� ���찳 ������Ʈ
    [SerializeField] private GameObject BoardEraser;
    //�������� ��������Ʈ ���ӿ�����Ʈ
    //[SerializeField] private GameObject Eraserable;
    //Test Object
    public GameObject TestSphere;
    //inputManager ��ü ������ ���� ��ǥ ȹ��
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

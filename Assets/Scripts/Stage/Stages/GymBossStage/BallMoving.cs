using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMoving : MonoBehaviour
{
    //경로 상의 모든 볼 이미지
    public Image[] TraceLine;
    public float deltaBallMoving = 1.1f;
    public int BallNum = 0;

    //현재 몇번 볼이 켜져 있는지
    private int BallIndex = 0;
    private bool Upward = true;
    private float Tempdelta = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        BallIndex = Random.Range(1, BallNum - 1);
        TraceLine[BallIndex].gameObject.SetActive(true);
        Tempdelta = deltaBallMoving;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log(Tempdelta);

        if(Tempdelta < 0.0f)
        {
            //TraceLine[BallIndex].enabled = false;
            //++BallIndex;
            //TraceLine[BallIndex].enabled = true;
            if(Upward)
            {
                TraceLine[BallIndex].gameObject.SetActive(false);
                ++BallIndex;
                TraceLine[BallIndex].gameObject.SetActive(true);
                if(BallIndex == BallNum - 1)
                {
                    Upward = false;
                }
            }
            else
            {
                TraceLine[BallIndex].gameObject.SetActive(false);
                --BallIndex;
                TraceLine[BallIndex].gameObject.SetActive(true);
                if(BallIndex == 0)
                {
                    Upward = true;
                }
            }

            Tempdelta =  deltaBallMoving;
            Debug.Log(deltaBallMoving);
        }
        else
        {
            Tempdelta -= Time.deltaTime;
        }
    }
}

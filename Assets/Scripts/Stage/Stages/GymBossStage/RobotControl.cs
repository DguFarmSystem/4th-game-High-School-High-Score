using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotControl : MonoBehaviour
{
    public AudioClip RobotBodySound;

    [SerializeField]
    private Image[] RobotParts;
    private AudioSource RobotAudio;

    public bool LeftTilted = true;

    void LeftLegHand()
    {
        RobotParts[1].gameObject.SetActive(false);
        RobotParts[3].gameObject.SetActive(false);
        RobotParts[5].gameObject.SetActive(false);
        RobotParts[7].gameObject.SetActive(false);
        RobotParts[0].gameObject.SetActive(true);
        RobotParts[2].gameObject.SetActive(true);
        RobotParts[4].gameObject.SetActive(true);
        RobotParts[6].gameObject.SetActive(true);
    }

    void RightLegHand()
    {
        RobotParts[0].gameObject.SetActive(false);
        RobotParts[2].gameObject.SetActive(false);
        RobotParts[4].gameObject.SetActive(false);
        RobotParts[6].gameObject.SetActive(false);
        RobotParts[1].gameObject.SetActive(true);
        RobotParts[3].gameObject.SetActive(true);
        RobotParts[5].gameObject.SetActive(true);
        RobotParts[7].gameObject.SetActive(true);
    }

    void ChangeRobotHandLeg()
    {
        if(LeftTilted)
        {
            LeftLegHand();
        }
        else
        {
            RightLegHand();
        }
    }

    public void RobotTouched()
    {
        LeftTilted = !LeftTilted;
        ChangeRobotHandLeg();
        RobotAudio.clip = RobotBodySound;
        RobotAudio.Play();

    }

    // Start is called before the first frame update
    void Start()
    {
        RobotAudio = GetComponent<AudioSource>();
        if (LeftTilted)
        {
            LeftLegHand();
        }
        else
        {
            RightLegHand();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

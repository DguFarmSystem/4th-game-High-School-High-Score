using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalProcessor : MonoBehaviour
{
    [SerializeField]
    private Sprite[] VitalSprites;
    [SerializeField]
    private Image VitalImage1;
    [SerializeField]
    private Image VitalImage2;
    [SerializeField]
    private Image VitalImage3;
    [SerializeField]
    private float Speed = 100f;

    private Vector3 MovePos;
    // Start is called before the first frame update
    void Start()
    {
        MovePos = VitalImage3.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        VitalMove();
    }

    void VitalMove()
    {
        VitalImage1.transform.Translate(Vector3.left * Speed * Time.deltaTime);
        VitalImage2.transform.Translate(Vector3.left * Speed * Time.deltaTime);
        VitalImage3.transform.Translate(Vector3.left * Speed * Time.deltaTime);

        Vector3 Pos1 = VitalImage1.rectTransform.anchoredPosition;
        Vector3 Pos2 = VitalImage2.rectTransform.anchoredPosition;
        Vector3 Pos3 = VitalImage3.rectTransform.anchoredPosition;

        if(Pos1.x < -1700)
        {
            MoveImage(VitalImage1);
        }
        else if (Pos2.x < -1700)
        {
            MoveImage(VitalImage2);
        }
        else if (Pos3.x < -1700)
        {
            MoveImage(VitalImage3);
        }
    }

    void MoveImage(Image MoveImage)
    {
        MoveImage.transform.position = MovePos;
    }

    public void ChangeImage(int index)
    {
        VitalImage1.sprite = VitalSprites[index];
        VitalImage2.sprite = VitalSprites[index];
        VitalImage3.sprite = VitalSprites[index];
    }
}

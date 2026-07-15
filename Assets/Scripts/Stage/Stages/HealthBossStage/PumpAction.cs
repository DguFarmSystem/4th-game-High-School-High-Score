using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PumpAction : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private VitalProcessor VitalCheck;

    private Animator MyAnimator;
    private int CountClick = 0;
    private bool CanClick = true;
    private bool isPumpClear = false;

    public bool GetPumpClear()
    {
        return isPumpClear;
    }

    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeVitalImage();
        if(CountClick >= 6)
        {
            isPumpClear = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(CanClick)
        {
            ++CountClick;
            MyAnimator.SetTrigger("Click");
            CanClick = false;
            StartCoroutine(BlockClick());
        }
    }

    void ChangeVitalImage()
    {
        if(CountClick == 2)
        {
            VitalCheck.ChangeImage(1);
        }
        else if (CountClick == 4)
        {
            VitalCheck.ChangeImage(2);
        }
    }

    private IEnumerator BlockClick()
    {
        yield return new WaitForSeconds(0.5f);
        CanClick = true;
    }
}

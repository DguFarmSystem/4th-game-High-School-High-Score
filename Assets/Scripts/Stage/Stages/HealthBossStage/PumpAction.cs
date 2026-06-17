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

    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ++CountClick;
        MyAnimator.SetTrigger("Click");
    }
}

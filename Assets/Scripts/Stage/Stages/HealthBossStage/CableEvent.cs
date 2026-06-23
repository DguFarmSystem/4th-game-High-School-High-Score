using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CableEvent : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private RectTransform HeartRange;
    [SerializeField]
    private GameObject ElectricVFX;

    private RectTransform Mytransform;
    private Canvas Mycanvas;

    // Start is called before the first frame update
    void Start()
    {
        Mytransform = GetComponent<RectTransform>();
        Mycanvas = GetComponentInParent<Canvas>();
        StartCoroutine(CheckCollisionRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(Mycanvas != null)
        {
            Mytransform.anchoredPosition += eventData.delta / Mycanvas.scaleFactor;
        }
    }

    private IEnumerator CheckCollisionRoutine()
    {
        float currentMatchTime = 0f;
        float targetDuration = 2f; // 목표 시간 (2초)

        while (true)
        {
            // 1번 방법인 사각형 영역 겹침 검사 실행
            bool isColliding = CheckUICollision(Mytransform, HeartRange);

            if (isColliding)
            {
                // 충돌 중이라면 시간을 누적합니다.
                currentMatchTime += Time.deltaTime;

                // 2초 연속 충돌에 성공했을 때
                if (currentMatchTime >= targetDuration)
                {
                    OnCollisionSuccess();

                    // 성공 후 타이머를 리셋하고 계속 감지할지, 
                    // 아니면 루프를 탈출(break)할지는 기획에 따라 선택하세요.
                    break;
                }
            }
            else
            {
                // 단 한 프레임이라도 false가 되면 타이머를 0으로 리셋합니다.
                if (currentMatchTime > 0f)
                {
                    Debug.Log("충돌이 끊겼습니다. 타이머를 리셋합니다.");
                    currentMatchTime = 0f;
                }
            }

            // 다음 프레임까지 대기
            yield return null;
        }
    }

    // 2초 버티기 성공 시 호출될 함수
    private void OnCollisionSuccess()
    {
        Debug.Log("UI 이미지 2초 연속 충돌 성공!");
        StartCoroutine(ActivateVFX());
        // 여기에 애니메이션 실행, 게이지 충전 완료, 다음 단계 이동 등의 로직을 넣으세요.
    }

    private bool CheckUICollision(RectTransform a, RectTransform b)
    {
        if (a == null || b == null) return false;

        Rect rectAWorld = GetWorldRect(a);
        Rect rectBWorld = GetWorldRect(b);

        return rectAWorld.Overlaps(rectBWorld);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        return new Rect(corners[0].x, corners[0].y, width, height);
    }
    
    private IEnumerator ActivateVFX()
    {
        ElectricVFX.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        ElectricVFX.SetActive(false);
    }
}


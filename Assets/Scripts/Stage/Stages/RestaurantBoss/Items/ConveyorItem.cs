using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorItem : MonoBehaviour
{
    [SerializeField] private Sprite _goldenSpongeSprite;

    public virtual void OnRemovedFromConveyor()
    {
        // 기본 구현은 비어 있음. 필요에 따라 서브클래스에서 오버라이드 가능.
        // 콤보 ui 업데이트, 효과음 재생 등
    }

    public void SwitchToGoldenSponge()
    {
        // 황금수세미로 변환하는 로직 구현
        if (this is not TimerItem) // 타이머는 제외
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = _goldenSpongeSprite;
            gameObject.AddComponent<GoldenSpongeItem>();
            Destroy(this);
        }
    }
}

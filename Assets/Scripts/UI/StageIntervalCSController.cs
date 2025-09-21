using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageIntervalCSController : MonoBehaviour
{
    private Image background;
    private Image character;
    private Image hpImage;
    private Animator hpAnimator;
    private List<Sprite> stageCountImage;
    private GameObject optionalLayoutPrefab;

    public void ApplySkin(StageIntervalSkin skin)
    {
        if (skin == null) return;

        if (skin.backgroundDefault) background.sprite = skin.backgroundDefault;
        if (skin.backgroundSuccess) background.sprite = skin.backgroundSuccess;
        if (skin.backgroundFailure) background.sprite = skin.backgroundFailure;

        if (skin.characterDefault) character.sprite = skin.characterDefault;
        if (skin.characterSuccess) character.sprite = skin.characterSuccess;
        if (skin.characterFailure) character.sprite = skin.characterFailure;

        if (skin.hpDefault) hpImage.sprite = skin.hpDefault;
        if (skin.animatorControllerHP != null && hpAnimator != null)
        {
            hpAnimator.runtimeAnimatorController = skin.animatorControllerHP;
        }

        if (skin.stageCountImage != null && skin.stageCountImage.Count > 0)
        {
            stageCountImage = skin.stageCountImage;
        }

        if (skin.optionalLayoutPrefab != null)
        {
            optionalLayoutPrefab = skin.optionalLayoutPrefab;
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    /*
    public IEnumerator StageIntervalCutscene()
    {
        yield return StartCoroutine(ShowHP());

        _titleLogo.SetActive(true);
        yield return new WaitForSeconds(1.1f);

        yield return StartCoroutine(BarAction());
        yield return new WaitForEndOfFrame();

        _gameStartBtn.SetActive(true);
        _settingsBtn.SetActive(true);
    }

    private IEnumerator ShowHP()
    {
        if (hpAnimator != null)
        {
            hpAnimator.SetTrigger("Show");
            yield return new WaitForSeconds(1.0f); // 애니메이션 재생 시간만큼 대기
        }
    }
    */

    /* =========== Lifecycle Methods =========== */
    private void Start()
    {
        //StartCoroutine(StageIntervalCutscene());
    }
}

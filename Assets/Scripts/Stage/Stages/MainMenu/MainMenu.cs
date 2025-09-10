using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _backGroundRT;
    [SerializeField] private GameObject _titleLogo;
    [SerializeField] private Image _upperBar;
    [SerializeField] private Image _lowerBar;
    [SerializeField] private GameObject _gameStartBtn;
    [SerializeField] private GameObject _settingsBtn;

    private Vector3 _startPos;
    private Vector3 _endPos;

    public IEnumerator TitleCutScene()
    {
        yield return StartCoroutine(MoveCamera());

        _titleLogo.SetActive(true);
        yield return new WaitForSeconds(1.1f);

        yield return StartCoroutine(BarAction());
        yield return new WaitForEndOfFrame();

        _gameStartBtn.SetActive(true);
        _settingsBtn.SetActive(true);
    }

    public IEnumerator MoveCamera()
    {
        Camera.main.transform.position = _startPos;

        float elapsedTime = 0f;

        while (elapsedTime < 1.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 1.5f;
            t = Mathf.SmoothStep(0f, 1f, t); // 0→1 사이를 부드럽게 증가

            Camera.main.transform.position = Vector3.Lerp(_startPos, _endPos, t);

            yield return null; // 다음 프레임까지 대기
        }
    }

    public IEnumerator BarAction()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 0.25f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.25f;

            _upperBar.fillAmount = Mathf.Lerp(0f, 1f, t);
            _lowerBar.fillAmount = Mathf.Lerp(0f, 1f, t);

            yield return null; // 다음 프레임까지 대기
        }
    }

    // ========== Lifecycle methods ========== //
    void Awake()
    {
        Vector3[] corners = new Vector3[4];
        _backGroundRT.GetWorldCorners(corners);

        _startPos = new Vector3(0,
                    corners[0].y + Camera.main.orthographicSize,
                   -10f);
        _endPos = new Vector3(0,
                    corners[1].y - Camera.main.orthographicSize,
                   -10f);
    }

    void Start()
    {
        StartCoroutine(TitleCutScene());
    }
}

using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;

public class Music_Player_LV4 : StageNormal
{
    [Header("LP Case")]
    [SerializeField] private GameObject _lpCase;
    [SerializeField] private SpriteRenderer _lpCaseIndicator;
    [SerializeField] private Vector3 _lpCaseTargetPos;

    [Space(10)]

    [Header("LP Player")]
    [SerializeField] private SpriteRenderer _lpPlayerIndicator1;
    [SerializeField] private SpriteRenderer _lpPlayerIndicator2;

    public enum StageClearConditions
    {
        TouchLPCase,
        InsertLPCase,
        ActivateLPPlayer,
    }

    public Queue<StageClearConditions> ClearConditionsQ =
        new Queue<StageClearConditions>(new[] { StageClearConditions.TouchLPCase,
                                                StageClearConditions.InsertLPCase,
                                                StageClearConditions.ActivateLPPlayer });
                                                
    public StageState CurrentState => CurrentStageState;

    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared)
        {
            Debug.Log("Stage cleared!");
            StageManager.Instance.StageClear(true);
        }
        else
        {
            Debug.Log("Stage failed!");
            StageManager.Instance.StageClear(false);
        }
    }

    private IEnumerator ShowUpLPCase()
    {
        float elapsedTime = 0f;
        Vector3 startingPos = _lpCase.transform.position;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;
            t = Mathf.SmoothStep(0f, 1f, t); // 0→1 사이를 부드럽게 증가

            _lpCase.transform.position = Vector3.Lerp(startingPos, _lpCaseTargetPos, t);

            yield return null; // 다음 프레임까지 대기
        }

        _lpCaseIndicator.enabled = true;
    }

    // ============ Lifecycle methods ============ //
    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    void Start()
    {
        // 스테이지 시작
        OnStageStart();
        StartCoroutine(ShowUpLPCase());
    }

    
    void Update()
    {
        if (ClearConditionsQ.Count > 0)
        {
            switch (ClearConditionsQ.Peek())
            {
                case StageClearConditions.TouchLPCase:

                    break;

                case StageClearConditions.InsertLPCase:
                    if (_lpPlayerIndicator1.enabled != true) _lpPlayerIndicator1.enabled = true;
                    break;

                case StageClearConditions.ActivateLPPlayer:
                    if (_lpPlayerIndicator2.enabled != true) _lpPlayerIndicator2.enabled = true;
                    break;
            }
        }
        
        if ( ClearConditionsQ.Count == 0 && CurrentStageState == StageState.Playing)
        {
            OnStageClear(); // 모든 조건이 완료되면 스테이지 클리어 처리
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

public class MusicGuitarStage : StageNormal
{
    [SerializeField]
    private PanelTouchHandler PickHandler;
    [SerializeField]
    private Image CrowdImage;
    [SerializeField]
    private List<Sprite> CrowdSprite;
    [Header("Audio Source")]
    [SerializeField]
    private AudioSource CrowdAudio;
    [SerializeField]
    private AudioSource GuitarAudio;

    [SerializeField]
    private AudioClip CrowdSource;
    [SerializeField]
    private List<int> Goal;

    private bool StopCrowdSound = false;
    private int CurrentCount = 0;
    private bool StageClearFlag = false;
    private static int stageLevel = 0;

    public override void OnStageStart()
    {
        base.OnStageStart();
    }
    protected override void OnStageEnd()
    {
        //Debug.Log("Stage Failed.. Current Level : " + stageLevel);
        base.OnStageEnd();
    }

    protected override void OnStageClear()
    {
        //stageLevel++;
        //Debug.Log("Stage Cleared!! Current Level : " + stageLevel);
        base.OnStageClear();
    }

    private void OnStageEndedGimmik(bool isStageCleared)
    {
        CrowdAudio.Stop();

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

    // ============ Lifecycle methods ============ //
    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    // Start is called before the first frame update
    void Start()
    {
        stageLevel = StageManager.Instance.GetDifficulty() - 1;
        CrowdImage.sprite = CrowdSprite[0];
        CrowdAudio.clip = CrowdSource;
        OnStageStart();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentCount = PickHandler.GetPlayCount();
        ChangeCrowdImage();
        if(CurrentCount >= Goal[stageLevel])
        {
            StageClearFlag = true;
        }

        if(StageClearFlag && CurrentStageState == StageState.Playing)
        {
            OnStageClear();
            StartCoroutine(StopSound());
        }
    }

    void ChangeCrowdImage()
    {
        float ClearRatio = (float)CurrentCount / Goal[stageLevel];
        float volumeSize = 0.0f;

        Debug.Log($"[ChangeCrowdImage] CurrentCount: {CurrentCount}, Goal: {Goal[stageLevel]}, ClearRatio: {ClearRatio:F2}");

        if (ClearRatio <= 0.25f)
        {
            CrowdImage.sprite = CrowdSprite[0];
        }
        else if (ClearRatio > 0.25f && ClearRatio <= 0.5f)
        {
            CrowdImage.sprite = CrowdSprite[1];
            volumeSize = 0.05f;
            HandleCrowdSound(volumeSize);

        }
        else if (ClearRatio > 0.5f && ClearRatio <= 0.75f)
        {
            CrowdImage.sprite = CrowdSprite[2];
            volumeSize = 0.1f;
            HandleCrowdSound(volumeSize);
        }
        else
        {
            CrowdImage.sprite = CrowdSprite[3];
            volumeSize = 0.2f;
            HandleCrowdSound(volumeSize);
        }
    }

    void HandleCrowdSound(float volumeRatio)
    {
        if (!CrowdAudio.isPlaying)
            CrowdAudio.Play();
        else
        {
            CrowdAudio.volume = volumeRatio;
        }

        if(StopCrowdSound)
        {
            CrowdAudio.Stop();
        }
    }

    private IEnumerator StopSound()
    {
        yield return new WaitForSeconds(2f);
        GuitarAudio.Stop();
        StopCrowdSound = true;
    }
}

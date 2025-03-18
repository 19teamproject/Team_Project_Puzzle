using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Title : MonoBehaviour
{
    [Header("Anim")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Material blurMaterial;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    [Header("Button")]
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button selectStageButton;

    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup titleGroup;
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private CanvasGroup stageGroup;

    [Header("Stage")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameObject loadPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button loadButtonPrefab;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        titleGroup.alpha = 1f;
        mainGroup.alpha = 0f;
        stageGroup.alpha = 0f;

        blurMaterial.SetFloat("_BlurRadius", 0f);

        fullscreenButton.gameObject.SetActive(true);
        fullscreenButton.onClick.AddListener(PlayAnim);
    }

    private void PlayAnim()
    {
        canvasGroup.DOFade(1f, duration)
            .SetEase(ease);
            
        blurMaterial.DOFloat(15f, "_BlurRadius", duration * 4f)
            .SetEase(ease)
            .OnComplete(() => {
                fullscreenButton.onClick.RemoveListener(PlayAnim);
                fullscreenButton.onClick.AddListener(MainMenu);
            });
    }

    private void MainMenu()
    {
        fullscreenButton.gameObject.SetActive(false);

        if (stageManager == null)
        {
            Debug.LogWarning("스테이지 매니저가 등록되지 않았습니다.");
            return;
        }

        titleGroup.DOFade(0f, duration)
            .SetEase(ease);

        mainGroup.DOFade(1f, duration)
            .SetEase(ease);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(stageManager.NewGame);

        if (continueGameButton != null)
            continueGameButton.onClick.AddListener(stageManager.ContinueGame);

        if (selectStageButton != null)
            selectStageButton.onClick.AddListener(SelectStage);
    }

    private void SelectStage()
    {
        mainGroup.DOFade(0f, duration)
            .SetEase(ease);

        stageGroup.DOFade(1f, duration)
            .SetEase(ease);

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        SaveData saveData = SaveSystem.LoadGame();
        List<int> availableStages = new(saveData.clearedStages)
        {
            saveData.currentStage // 현재 진행 중인 스테이지도 포함
        };

        foreach (int stage in availableStages)
        {
            Button newButton = Instantiate(loadButtonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = stage.ToString();
            newButton.onClick.AddListener(() => stageManager.LoadStage(stage));
        }
    }
}
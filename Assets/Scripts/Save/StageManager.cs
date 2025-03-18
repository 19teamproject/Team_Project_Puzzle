using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class StageManager : MonoSingleton<StageManager>
{
    [SerializeField] private StageList stageList;
    [SerializeField] private CanvasGroup timeUI; // 시간 프리팹
    private SaveData saveData;

    private bool isStage;
    private bool isClear;
    private bool isTransition;


    [ShowNonSerializedField] private float time;
    private readonly string timeKey;

    [Button]
    private void DebugStageReset()
    {
        SaveSystem.ResetGame(); // 기존 세이브 데이터 삭제
        saveData = new SaveData(); // 새로운 세이브 데이터 생성
        SaveSystem.SaveGame(saveData);
    }
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // 게임 데이터를 불러와서 현재 스테이지 설정
        saveData = SaveSystem.LoadGame();
    }

    private void Update()
    {
        if (isStage) time += Time.deltaTime;
        if (isClear && Input.GetMouseButtonDown(0))
        {
            isClear = false;
            SceneLoader.Instance.LoadScene(stageList.titleScene.name);
            DOVirtual.DelayedCall(0.5f, () => CharacterManager.Instance.Player.Controller.cursorInputForLook = true);
        }
    }

    /// <summary>
    /// 현재 진행 중인 스테이지 업데이트
    /// </summary>
    /// <param name="stageIndex">스테이지 인덱스</param>
    public void EnterStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= stageList.stageScenes.Count)
        {
            Debug.LogError("잘못된 스테이지입니다!");
            return;
        }

        saveData.currentStage = stageIndex;
        SaveSystem.SaveGame(saveData);
    }

    /// <summary>
    /// 현재 진행 중인 스테이지 로드 (이어하기 기능)
    /// </summary>
    public void ContinueGame()
    {
        LoadStage(saveData.currentStage);
    }

    /// <summary>
    /// 특정 스테이지 로드 (불러오기 기능)
    /// </summary>
    /// <param name="stageIndex">스테이지 인덱스</param>
    public void LoadStage(int stageIndex)
    {
        string sceneName = stageList.GetSceneName(stageIndex);

        if (stageIndex < 0 || stageIndex >= stageList.stageScenes.Count)
        {
            Debug.LogError("잘못된 스테이지입니다!");
            return;
        }

        saveData.currentStage = stageIndex;
        SaveSystem.SaveGame(saveData);
        SceneLoader.Instance.LoadScene(sceneName);

        if (time == 0f) time += PlayerPrefs.GetFloat(timeKey, 0f);
        isStage = true;
    }

    /// <summary>
    /// 스테이지 클리어 시 저장 및 다음 스테이지 이동
    /// </summary>
    public void CompleteStage()
    {
        if (isTransition) return;

        isTransition = true;

        if (!saveData.clearedStages.Contains(saveData.currentStage))
        {
            saveData.clearedStages.Add(saveData.currentStage);
        }

        if (saveData.currentStage < stageList.stageScenes.Count - 1)
        {
            saveData.currentStage++; // 다음 스테이지로 진행
            SaveSystem.SaveGame(saveData);

            LoadStage(saveData.currentStage);
        }
        else
        {
            if (isClear) return;

            Debug.Log("모든 스테이지를 클리어했습니다!");

            CanvasGroup timeCG = Instantiate(timeUI);

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            string timeText = $"<b><color=#fff9>CLEAR</color=#fff9></b>\n{minutes:00}:{seconds:00}";

            timeCG.GetComponentInChildren<TextMeshProUGUI>().text = timeText;

            timeCG.DOFade(1f, 0.5f)
                .From(0f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);

            isStage = false;
            isClear = true;
            time = 0f;

            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 0.5f)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
                
            CharacterManager.Instance.Player.Controller.cursorInputForLook = false;
            CharacterManager.Instance.Player.Controller.look = Vector2.zero;
        }
    }

    /// <summary>
    /// 새 게임 시작 (진행 데이터 초기화)
    /// </summary>
    public void NewGame()
    {
        SaveSystem.ResetGame(); // 기존 세이브 데이터 삭제
        saveData = new SaveData(); // 새로운 세이브 데이터 생성
        SaveSystem.SaveGame(saveData);

        // 첫 번째 스테이지로 이동
        string firstSceneName = stageList.GetSceneName(0);
        if (!string.IsNullOrEmpty(firstSceneName))
        {
            isStage = true;
            SceneLoader.Instance.LoadScene(firstSceneName);
        }
        else
        {
            Debug.LogError("스테이지 리스트에 스테이지가 없습니다!");
        }
    }

    private void OnApplicationQuit()
    {
        isStage = false;
        PlayerPrefs.SetFloat(timeKey, time);
        PlayerPrefs.Save();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isTransition = false;
    }
}

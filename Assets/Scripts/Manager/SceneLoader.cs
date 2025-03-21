using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoSingleton<SceneLoader>
{
    [SerializeField] private CanvasGroup fadeCanvas; // 페이드 효과를 위한 CanvasGroup
    [SerializeField] private float fadeDuration = 0.5f; // 페이드 속도

    private CanvasGroup fadeCanvasInstance;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (fadeCanvasInstance == null)
        {
            fadeCanvasInstance = Instantiate(fadeCanvas);
            DontDestroyOnLoad(fadeCanvasInstance.gameObject);
        }
        if (fadeCanvasInstance != null)
        {
            fadeCanvasInstance.alpha = 1;
            StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// 씬 이동 (기본)
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    /// <summary>
    /// 현재 씬 다시 불러오기
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneCoroutine(currentScene));
    }

    /// <summary>
    /// 비동기 씬 로딩 (페이드 효과 포함)
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    /// <returns></returns>
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        if (fadeCanvasInstance != null)
            yield return StartCoroutine(FadeOut());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (fadeCanvasInstance != null)
            yield return StartCoroutine(FadeIn());
    }

    /// <summary>
    /// 페이드 인 효과 (씬 진입 시)
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            fadeCanvasInstance.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeCanvasInstance.alpha = 0;
        fadeCanvasInstance.blocksRaycasts = false;
    }

    /// <summary>
    /// 페이드 아웃 효과 (씬 나가기 전)
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut()
    {
        fadeCanvasInstance.blocksRaycasts = true;
        float time = 0;
        while (time < fadeDuration)
        {
            fadeCanvasInstance.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeCanvasInstance.alpha = 1;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("フェード設定")]
    [Tooltip("シーン切り替え直前のフェードにかかる時間")] // Tooltipの文言を分かりやすく変更
    [SerializeField] private float fadeDuration = 1.0f;

    [Tooltip("フェードに使用するUIのImage")]
    [SerializeField] private Image fadeImage;

    private AudioSource audioSource;
    private bool isLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        fadeImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 外部からシーン遷移を呼び出すための公開メソッド
    /// </summary>
    /// <param name="sceneName">読み込むシーンの名前</param>
    /// <param name="sound">再生するサウンド</param>
    public void StartTransition(string sceneName, AudioClip sound)
    {
        if (isLoading)
        {
            return;
        }
        isLoading = true;
        
        // 遷移処理の本体であるコルーチンを呼び出す
        StartCoroutine(LoadSceneRoutine(sceneName, sound));
    }

    // ▼▼▼ このコルーチンの処理順を大きく変更 ▼▼▼
    private IEnumerator LoadSceneRoutine(string sceneName, AudioClip sound)
    {
        // ① 裏でシーンをロード開始
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // ② SEを再生
        if (sound != null)
        {
            audioSource.PlayOneShot(sound);
        }

        // ③ SEの再生とシーンのロードが終わるのを並行して待つ
        float soundWaitTime = sound != null ? sound.length : 0f;
        float startTime = Time.unscaledTime; // 時間計測を開始

        // SEの再生時間が経過し、かつ、ロードの進捗が0.9以上になるまで待機
        while (Time.unscaledTime - startTime < soundWaitTime || asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // ④ すべての準備が完了したので、フェードアウトを開始
        yield return StartCoroutine(Fade(1.0f));

        // ⑤ フェードアウト完了後、シーンを有効化する
        Time.timeScale = 1f; // ポーズ中からの遷移も考慮して時間を戻す
        asyncLoad.allowSceneActivation = true;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // シーンが完全にロードされたらフェードインを開始
        StartCoroutine(Fade(0.0f));
        isLoading = false;
    }

    // フェード処理のコルーチン (変更なし)
    private IEnumerator Fade(float targetAlpha)
    {
        fadeImage.gameObject.SetActive(true);
        float startAlpha = fadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);

        if (targetAlpha == 0)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}
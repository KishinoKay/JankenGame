using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("フェード設定")]
    [Tooltip("フェードに使用するUIのImage")]
    [SerializeField] private Image fadeImage;

    [Tooltip("フェードにかかる時間")]
    [SerializeField] private float fadeDuration = 1.0f;

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

    private IEnumerator LoadSceneRoutine(string sceneName, AudioClip sound)
    {
        // ① まずフェードアウト
        yield return StartCoroutine(Fade(1.0f));

        // ② 裏でシーンをロード開始
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // ③ SEを再生して待機（時間は止まったまま）
        if (sound != null)
        {
            audioSource.PlayOneShot(sound);
            yield return new WaitForSecondsRealtime(sound.length);
        }
        
        // ④ ロードが終わるのを待つ
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // ⑤ シーンを有効化する直前に、時間を元に戻す！
        Time.timeScale = 1f;
        asyncLoad.allowSceneActivation = true;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(Fade(0.0f));
        isLoading = false;
    }

    // フェード処理のコルーチン
    private IEnumerator Fade(float targetAlpha)
    {
        fadeImage.gameObject.SetActive(true);
        float startAlpha = fadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            // ここを unscaledDeltaTime に変更！
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
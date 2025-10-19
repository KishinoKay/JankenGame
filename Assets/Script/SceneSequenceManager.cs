using System.Collections;
using UnityEngine;

/// <summary>
/// CanvasGroupを使ったフェード処理と「スキップ可能な待機」を含むシーケンス管理クラス
/// </summary>
public class SceneSequenceManager : MonoBehaviour
{
    [Header("フェード設定")]
    [Tooltip("フェード処理の対象となるCanvasGroup")]
    [SerializeField]
    private CanvasGroup fadeCanvasGroup;

    [Tooltip("フェードイン・アウトにかかる時間（秒）")]
    [SerializeField]
    private float fadeDuration = 1.5f;

    [Tooltip("フェードイン後の待機時間（秒）")]
    [SerializeField]
    private float waitDuration = 3.0f;


    [Header("シーン遷移の設定")]
    [SerializeField]
    private string sceneToLoad = "YourNextSceneName";

    [SerializeField]
    private AudioClip buttonSound;


    // --- スキップ関連の追加 ---
    /// <summary>
    /// スキップがリクエストされたかどうかのフラグ
    /// </summary>
    private bool isSkipRequested = false;

    /// <summary>
    /// (public) スキップボタンのOnClickイベントから呼び出す
    /// </summary>
    public void RequestSkip()
    {
        // 待機中（isSkipRequestedがfalseの間）のみ受け付ける
        if (!isSkipRequested)
        {
            isSkipRequested = true;
            Debug.Log("スキップリクエストを受け付けました");
        }
    }
    // ------------------------


    private void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
        }

        // シーケンスを開始
        StartCoroutine(RunSceneSequence());
    }

    /// <summary>
    /// 目的のシーケンスを実行するコルーチン本体
    /// </summary>
    private IEnumerator RunSceneSequence()
    {
        // 1. フェードイン (Alpha を 1 へ)
        Debug.Log("フェードイン開始");
        yield return StartCoroutine(FadeCanvas(1f, fadeDuration));

        // 2. 数秒待機 (スキップ可能)
        Debug.Log("待機開始 (スキップ可能)");
        // ▼▼▼ 以前のコード ▼▼▼
        // yield return new WaitForSeconds(waitDuration);
        // ▼▼▼ 新しいコード (スキップ処理を呼び出す) ▼▼▼
        yield return StartCoroutine(WaitWithSkip(waitDuration));

        // 3. フェードアウト (Alpha を 0 へ)
        //    (スキップされた場合、isSkipRequested は true のままだが、
        //     フェードアウト処理はスキップしないのでこのままでOK)
        Debug.Log("フェードアウト開始");
        yield return StartCoroutine(FadeCanvas(0f, fadeDuration));

        // 4. 次のシーンへ
        Debug.Log("シーン遷移実行");
        SceneTransition.Instance.StartTransition(sceneToLoad, buttonSound);
    }

    /// <summary>
    /// CanvasGroupのAlphaを徐々に変更するコルーチン
    /// </summary>
    private IEnumerator FadeCanvas(float targetAlpha, float duration)
    {
        // (前回のコードから変更なし)
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("FadeCanvasGroupが設定されていません！");
            yield break;
        }
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;
        while (time < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
    }


    // --- スキップ可能な待機コルーチン (新規追加) ---
    /// <summary>
    /// 指定時間待機する。ただし isSkipRequested が true になったら即座に待機を中断する。
    /// </summary>
    private IEnumerator WaitWithSkip(float duration)
    {
        float timer = 0f;

        // 時間が経過するか、スキップフラグが立つまでループ
        while (timer < duration && !isSkipRequested)
        {
            timer += Time.deltaTime;
            yield return null; // 1フレーム待つ
        }

        if (isSkipRequested)
        {
            Debug.Log("待機をスキップしました");
        }
        else
        {
            Debug.Log("指定時間の待機が完了しました");
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI（ボタン）操作に必要

#if UNITY_EDITOR
using UnityEditor; // Unityエディタの操作に必要
#endif

/// <summary>
/// ボタンのSEを再生した後にゲームを終了するクラス
/// </summary>
// AudioSourceコンポーネントがアタッチされていることを保証
[RequireComponent(typeof(AudioSource))]
public class QuitWithSound : MonoBehaviour
{
    [Header("サウンド設定")]
    [Tooltip("再生するボタンのSE")]
    [SerializeField] private AudioClip buttonSound;

    [Header("ボタン設定")]
    [Tooltip("このスクリプトを呼び出すボタン")]
    [SerializeField] private Button quitButton;

    // SE再生用のAudioSource
    private AudioSource audioSource;

    // 連打防止用のフラグ
    private bool isQuitting = false;

    private void Start()
    {
        // このゲームオブジェクトにアタッチされているAudioSourceを取得
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// ボタンのOnClickイベントからこのメソッドを呼び出してゲーム終了処理を開始する
    /// </summary>
    public void StartQuitProcess()
    {
        // すでに処理が開始されていたら、何もしない（連打防止）
        if (isQuitting)
        {
            return;
        }

        // 処理中フラグを立て、ボタンを無効化する
        isQuitting = true;
        if (quitButton != null)
        {
            quitButton.interactable = false;
        }

        // コルーチンを開始
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        // 1. SEを再生する
        // PlayOneShotを使うと、他のSEを邪魔せずに再生できる
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        // 2. SEの再生が終わるまで待つ
        // buttonSoundが設定されていない場合は待たない
        if (buttonSound != null)
        {
            yield return new WaitForSeconds(buttonSound.length);
        }

        // 3. ゲームを終了する
        Debug.Log("ゲームを終了します！");
        
#if UNITY_EDITOR
        // Unityエディタで実行している場合、再生を停止
        EditorApplication.isPlaying = false;
#else
        // ビルドされたゲームの場合、アプリケーションを終了
        Application.Quit();
#endif
    }
}
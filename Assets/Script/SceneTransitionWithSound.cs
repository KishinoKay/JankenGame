using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理に必要
using UnityEngine.UI; // UI（ボタン）操作に必要

// AudioSourceコンポーネントがアタッチされていることを保証
[RequireComponent(typeof(AudioSource))]
public class SceneTransitionWithSound : MonoBehaviour
{
    [Header("シーン設定")]
    [Tooltip("読み込むシーンの名前")]
    [SerializeField] private string sceneToLoad;

    [Header("サウンド設定")]
    [Tooltip("再生するボタンのSE")]
    [SerializeField] private AudioClip buttonSound;

    [Header("ボタン設定")]
    [Tooltip("このスクリプトを呼び出すボタン")]
    [SerializeField] private Button transitionButton;

    // SE再生用のAudioSource
    private AudioSource audioSource;

    // 連打防止用のフラグ
    private bool isLoading = false;

    private void Start()
    {
        // このゲームオブジェクトにアタッチされているAudioSourceを取得
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// ボタンのOnClickイベントからこのメソッドを呼び出す
    /// </summary>
    public void StartTransition()
    {
        // すでに処理が開始されていたら、何もしない（連打防止）
        if (isLoading)
        {
            return;
        }

        // 処理中フラグを立て、ボタンを無効化する
        isLoading = true;
        if (transitionButton != null)
        {
            transitionButton.interactable = false;
        }

        // コルーチンを開始
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        // 1. バックグラウンドでシーンの読み込みを開始する
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        // すぐにはシーンを有効化しないように設定
        asyncLoad.allowSceneActivation = false;

        // 2. SEを再生する
        // PlayOneShotを使うと、他のSEを邪魔せずに再生できる
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        // 3. SEの再生が終わるまで待つ
        // buttonSoundが設定されていない場合は待たない
        if (buttonSound != null)
        {
            yield return new WaitForSeconds(buttonSound.length);
        }

        // 4. シーンの読み込みが完了するのを待つ (通常はSEの再生中に終わっている)
        // allowSceneActivationがfalseの場合、進捗は0.9で止まる
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 5. シーンを有効化して、画面を切り替える
        Debug.Log("シーンを切り替えます！");
        asyncLoad.allowSceneActivation = true;
    }
}
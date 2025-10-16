// GameManager.cs (Input System版)
using UnityEngine;
using UnityEngine.InputSystem; // Input Systemの名前空間を追加
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseUIPanel;
    private bool isPaused = false;

    [Header("ゲーム中断の設定")]
    [Tooltip("再生するボタンのSE")]
    [SerializeField] private AudioClip pauseSound;

    [Header("ゲームに戻る時の設定")]
    [Tooltip("再生するボタンのSE")]
    [SerializeField] private AudioClip resumeSound;

    private AudioSource[] audioSource;

    void Start()
    {
        audioSource = GetComponents<AudioSource>();
        pauseUIPanel.SetActive(false);
    }

    // このメソッドをPlayerInputコンポーネントから呼び出す
    public void OnPauseButton(InputAction.CallbackContext context)
    {
        // キーが押された瞬間だけ処理する
        if (context.performed)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (pauseSound != null)
        {
            audioSource[0].PlayOneShot(pauseSound);
        }
        isPaused = true;
        Time.timeScale = 0f; // 時間を止める
        pauseUIPanel.SetActive(true);
        // 必要なら、プレイヤーの操作Action Mapを無効化し、UI操作のAction Mapを有効化する
        // FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("UI");
    }

    public void ResumeGame()
    {
        if (resumeSound != null)
        {
            audioSource[1].PlayOneShot(resumeSound);
        }
        isPaused = false;
        Time.timeScale = 1f; // 時間を再開
        pauseUIPanel.SetActive(false);
        // UI操作のAction Mapからプレイヤー操作のAction Mapに戻す
        // FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Player");
    }
}
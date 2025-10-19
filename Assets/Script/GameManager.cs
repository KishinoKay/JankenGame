// GameManager.cs (完成版)
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("参照するコンポーネント")]
    [SerializeField] private GameObject pauseUIPanel;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject firstSelectedButton; // ポーズ時に最初に選択するボタン
    private bool isPaused = false;

    [Header("ゲーム中断の設定")]
    [Tooltip("再生するボタンのSE")]
    [SerializeField] private AudioClip pauseSound;

    [Header("ゲームに戻る時の設定")]
    [Tooltip("再生するボタンのSE")]
    [SerializeField] private AudioClip resumeSound;

    private AudioSource audioSource;

    private float lastPauseTime;
    private float pauseCooldown = 0.2f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pauseUIPanel.SetActive(false);
    }

    // このメソッドをPlayerInputコンポーネントから呼び出す
    public void OnPauseButton(InputAction.CallbackContext context)
    {
        // キーが押された瞬間だけ処理する
        if (context.performed)
        {
            // ★★★ 3. 時間チェックのロジックを追加 ★★★
            // (現在の実時間 - 最後にポーズした実時間) がクールダウン時間より短い場合
            if (Time.realtimeSinceStartup - lastPauseTime < pauseCooldown)
            {
                // 処理を中断して、何も起こさない
                return;
            }
            
            // クールダウン時間を超えていたら、時刻を更新
            lastPauseTime = Time.realtimeSinceStartup;
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
            audioSource.PlayOneShot(pauseSound);
        }
        isPaused = true;
        Time.timeScale = 0f; // 時間を止める
        pauseUIPanel.SetActive(true);

        // UI操作のAction Mapに切り替える
        playerInput.SwitchCurrentActionMap("UI");

        // ★★★ ここからが追加した処理 ★★★
        // 一度選択をクリアしてから、指定したボタンを選択状態にする
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void ResumeGame()
    {
        if (resumeSound != null)
        {
            audioSource.PlayOneShot(resumeSound);
        }
        isPaused = false;
        Time.timeScale = 1f; // 時間を再開
        pauseUIPanel.SetActive(false);

        // プレイヤー操作のAction Mapに戻す
        playerInput.SwitchCurrentActionMap("Player");
        
        // ★★★ 選択状態をクリアしておく ★★★
        EventSystem.current.SetSelectedGameObject(null);
    }
}
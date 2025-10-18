// GoalTrigger.cs
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // ★★★ UI操作のために追加 ★★★

public class GoalTrigger : MonoBehaviour
{
    [Header("UIの設定")]
    [Tooltip("表示するリザルト画面のUIパネル")]
    [SerializeField] private GameObject resultUIPanel;
    [Tooltip("リザルト画面表示時に最初に選択するボタン")] // ★★★ 追加 ★★★
    [SerializeField] private GameObject firstSelectedButton; 

    [Header("判定するオブジェクト")]
    [Tooltip("ゴール判定を行うプレイヤーオブジェクト")]
    [SerializeField] private GameObject playerObject;

    [Header("サウンドの設定")]
    [Tooltip("ゴール時に再生するサウンド")]
    [SerializeField] private AudioClip goalSound;

    private AudioSource audioSource;
    private bool isGoal = false; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (resultUIPanel != null)
        {
            resultUIPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("GoalTrigger: OnTriggerEnter detected with " + other.gameObject.name);
        if (!isGoal && other.gameObject == playerObject)
        {
            PerformGoal();
        }
    }

    private void PerformGoal()
    {
        isGoal = true;

        if (goalSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(goalSound);
        }

        Time.timeScale = 0f;

        if (resultUIPanel != null)
        {
            resultUIPanel.SetActive(true);
        }

        // プレイヤーの操作Action Mapを無効にするか、UI用のAction Mapに切り替える
        PlayerInput playerInput = playerObject.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            // "UI"という名前のAction Mapがある場合はそちらに切り替えるのが一般的
            playerInput.SwitchCurrentActionMap("UI");
        }
        
        // マウスカーソルを表示し、ロックを解除する
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ★★★ ここからが追加した処理 ★★★
        // 一度選択をクリアしてから、指定したボタンを選択状態にする
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}
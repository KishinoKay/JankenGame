using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class GoalTrigger : MonoBehaviour
{
    [Header("UIの設定")]
    [Tooltip("表示するリザルト画面のUIパネル")]
    [SerializeField] private GameObject resultUIPanel;
    [Tooltip("リザルト画面表示時に最初に選択するボタン")]
    [SerializeField] private GameObject firstSelectedButton;

    [Header("判定するオブジェクト")]
    [Tooltip("ゴール判定を行うプレイヤーオブジェクト")]
    [SerializeField] private GameObject playerObject;

    [Header("演出の設定")]
    [Tooltip("プレイヤーが自動で歩いていく目標地点（城の奥など）")]
    [SerializeField] private Transform entrancePoint;

    [Tooltip("自動で歩くときの速度")]
    [SerializeField] private float autoMoveSpeed = 2f;

    [Tooltip("自動移動の強制タイムアウト時間（秒）")]
    [SerializeField] private float autoMoveTimeout = 10f;

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
        if (!isGoal && other.gameObject == playerObject)
        {
            StartCoroutine(GoalSequence());
        }
    }

    /// <summary>
    /// ゴール演出からリザルトUI表示までの一連の流れを実行するコルーチン
    /// </summary>
    private IEnumerator GoalSequence()
    {
        isGoal = true;
        Debug.Log("1. ゴールシーケンス開始");

        // --- 1. ゴールサウンド再生 ---
        if (goalSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(goalSound);
        }

        // --- 2. プレイヤーの操作と物理を無効化 ---
        PlayerInput playerInput = playerObject.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false; // 一時的に入力を完全に無効化
        }

        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // velocity の方が一般的
            rb.gravityScale = 0;        // 重力を切る
        }
        Debug.Log("2. プレイヤーの操作を無効化");

        // --- 3. プレイヤーを目標地点 (entrancePoint) まで自動移動 ---
        if (entrancePoint != null)
        {
            Debug.Log("3. 自動移動を開始します。");
            float timer = 0f;

            while (Vector2.Distance(playerObject.transform.position, entrancePoint.position) > 0.05f)
            {
                timer += Time.deltaTime;
                if (timer > autoMoveTimeout)
                {
                    Debug.LogWarning($"自動移動がタイムアウト ({autoMoveTimeout}秒) しました。演出をスキップしてリザルトを表示します。");
                    break;
                }

                playerObject.transform.position = Vector2.MoveTowards(
                    playerObject.transform.position,
                    entrancePoint.position,
                    autoMoveSpeed * Time.deltaTime
                );
                yield return null;
            }
            Debug.Log("4. 自動移動が完了（またはスキップ）しました。");
        }
        else
        {
            Debug.LogError("Entrance Point がインスペクタで設定されていません！ 自動移動はスキップされます。");
        }


        // ★★★ 修正点：ここから処理の順番を変更 ★★★

        // --- 5. 時間を止めてリザルトUIを表示する ---
        Time.timeScale = 0f;

        if (resultUIPanel != null)
        {
            resultUIPanel.SetActive(true);
            Debug.Log("5. リザルトUIを表示しました。");
        }
        else
        {
            Debug.LogError("Result UI Panel がインスペクタで設定されていません！");
        }
        
        // --- 6. PlayerInput を再度有効にし、Action Map を "UI" に切り替える ---
        // (プレイヤーを非表示にする前に実行)
        if (playerInput != null)
        {
            playerInput.enabled = true; // 再び有効化
            playerInput.SwitchCurrentActionMap("UI");
            Debug.Log("6. Action Map を 'UI' に切り替えました。");
        }

        // --- 7. マウスカーソルとボタン選択の処理 ---
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        // --- 8. 最後にプレイヤーを非表示にする ---
        playerObject.SetActive(false);
        Debug.Log("7. プレイヤーを非表示にしました。");


        Debug.Log("8. シーケンス完了");
    }
}

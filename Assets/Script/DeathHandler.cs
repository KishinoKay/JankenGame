using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// プレイヤーが死亡した時の処理（死亡演出、ゲームオーバーUI表示）を管理するクラス
/// ★外部から呼び出されることを想定
/// </summary>
public class PlayerDeathHandler : MonoBehaviour
{
    [Header("UIの設定")]
    [Tooltip("表示するゲームオーバー画面のUIパネル")]
    [SerializeField] private GameObject gameOverUIPanel;
    [Tooltip("ゲームオーバー画面表示時に最初に選択するボタン（リトライボタンなど）")]
    [SerializeField] private GameObject firstSelectedButton;

    [Header("判定するオブジェクト")]
    [Tooltip("死亡判定を行うプレイヤーオブジェクト")]
    [SerializeField] private GameObject playerObject; // 演出の対象

    [Header("演出の設定")]
    [Tooltip("死亡アニメーションや演出を見せるための待機時間（秒）")]
    [SerializeField] private float deathSequenceTime = 1.5f;

    [Header("サウンドの設定")]
    [Tooltip("死亡時に再生するサウンド")]
    [SerializeField] private AudioClip deathSound;

    private AudioSource audioSource;
    private bool isDead = false;

    void Start()
    {
        // AudioSourceを取得（このオブジェクト、またはプレイヤーから）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && playerObject != null)
        {
            audioSource = playerObject.GetComponent<AudioSource>();
        }
        
        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 【公開関数】死亡処理を開始するトリガー。
    /// PlayerHealthスクリプトなど、外部からこの関数を呼び出す。
    /// </summary>
    public void TriggerDeath()
    {
        // 既に死亡処理が開始していれば何もしない
        if (isDead) return;
        
        StartCoroutine(DeathSequence());
    }

    /// <summary>
    /// 死亡演出からゲームオーバーUI表示までの一連の流れを実行するコルーチン
    /// </summary>
    private IEnumerator DeathSequence()
    {
        isDead = true; // 多重実行を防止
        Debug.Log("1. 死亡シーケンス開始");

        // --- 1. 死亡サウンド再生 ---
        if (deathSound != null && audioSource != null)
        {
            Debug.Log("   死亡サウンドを再生します。");
            audioSource.PlayOneShot(deathSound);
        }

        // --- 2. プレイヤーの操作と物理を無効化 ---
        if (playerObject == null)
        {
            Debug.LogError("Player Object が設定されていません！ 死亡処理を中断します。");
            yield break;
        }

        PlayerInput playerInput = playerObject.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false; // 入力を完全に無効化
        }

        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // 速度をゼロに
            rb.gravityScale = 0;          // 重力を切る
        }
        Debug.Log("2. プレイヤーの操作と物理を無効化");

        // (オプション) ここで死亡アニメーションをトリガーする
        // Animator anim = playerObject.GetComponent<Animator>();
        // if (anim != null)
        // {
        //     anim.SetTrigger("Die");
        // }

        // --- 3. 死亡演出のための待機 ---
        Debug.Log($"3. 死亡演出のため {deathSequenceTime} 秒待機します。");
        // Time.timeScaleの影響を受けない待機に変更
        yield return new WaitForSecondsRealtime(deathSequenceTime); 
        Debug.Log("4. 待機完了。");

        // --- 4. 時間を止めてゲームオーバーUIを表示する ---
        Time.timeScale = 0f;

        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(true);
            Debug.Log("5. ゲームオーバーUIを表示しました。");
        }
        else
        {
            Debug.LogError("Game Over UI Panel がインスペクタで設定されていません！");
        }
        
        // --- 5. PlayerInput を再度有効にし、Action Map を "UI" に切り替える ---
        if (playerInput != null)
        {
            playerInput.enabled = true; // UI操作のために再度有効化
            playerInput.SwitchCurrentActionMap("UI");
            Debug.Log("6. Action Map を 'UI' に切り替えました。");
        }

        // --- 6. マウスカーソルとボタン選択の処理 ---
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        // --- 7. 最後にプレイヤーを非表示にする ---
        playerObject.SetActive(false);
        Debug.Log("7. プレイヤーを非表示にしました。");
        Debug.Log("8. 死亡シーケンス完了");
    }
}
using UnityEngine;

/// <summary>
/// 「水中にいる」かつ「プレイヤーが範囲内にいる」時だけプレイヤーを追跡するスクリプト
/// </summary>
public class WaterEnemyChase : MonoBehaviour
{
    [Header("追跡設定")]
    [Tooltip("追跡時の移動速度")]
    [SerializeField] private float chaseSpeed = 5f;
    [Tooltip("プレイヤーのタグ名")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("水のタグ名")]
    [SerializeField] private string waterTag = "Water";

    private Rigidbody2D rb;
    private int waterTriggerCount = 0;    // 水中にいくつ入っているか
    private Transform playerTarget;       // 追跡対象（プレイヤー）
    private Vector3 originalScale;    // 元のスプライトの向き

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 物理演算の影響を受けず、スクリプト（transform.position）だけで動くように
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        playerTarget = null;
        waterTriggerCount = 0;
        
        // 元の向きを保存
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 追跡条件：水中にいて(>0) かつ プレイヤーが範囲内にいる(nullではない)
        bool canChase = (waterTriggerCount > 0 && playerTarget != null);

        if (canChase)
        {
            // ★追跡処理を実行
            
            // プレイヤーの X 座標を追いかける
            Vector2 targetPosition = new Vector2(
                playerTarget.position.x, // 目的地の X 座標はプレイヤー
                playerTarget.position.y  // Y 座標もプレイヤーの Y 座標を使う
            );

            // プレイヤーに向かって移動
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                chaseSpeed * Time.deltaTime // 追跡速度
            );

            // プレイヤーのいる方向にスプライトを向ける
            FlipSprite(playerTarget.position.x > transform.position.x);
        }
        
        // ★
        // else (canChase が false の場合) は何もしない（停止する）
        // ★
    }

    
    // --- トリガー検知 ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 水に入ったか
        if (other.CompareTag(waterTag))
        {
            waterTriggerCount++;
        }
        // プレイヤーが検知範囲に入ったか
        if (other.CompareTag(playerTag))
        {
            playerTarget = other.transform; // プレイヤーを追跡対象に設定
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 水から出たか
        if (other.CompareTag(waterTag))
        {
            waterTriggerCount--;
        }
        // プレイヤーが検知範囲から出たか
        if (other.CompareTag(playerTag))
        {
            playerTarget = null; // 追跡対象を解除
        }
    }
    
    /// <summary>
    /// スプライトの向きを反転させる
    /// </summary>
    /// <param name="faceRight">右を向かせる場合は true</param>
    private void FlipSprite(bool faceRight)
    {
        if (faceRight)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x), // Xを正の値に (右向き)
                originalScale.y,
                originalScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x), // Xを負の値に (左向き)
                originalScale.y,
                originalScale.z
            );
        }
    }
}
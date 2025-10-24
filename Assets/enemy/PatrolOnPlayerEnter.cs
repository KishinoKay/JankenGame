using UnityEngine;
using System.Collections;

/// <summary>
/// プレイヤーが検知範囲に入ったら、設定したターゲット地点まで移動し、
/// その後、指定された戻り地点（または元の場所）へ戻るスクリプト。
/// </summary>
public class PatrolOnPlayerEnter : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度")]
    [SerializeField] private float moveSpeed = 3f;
    
    [Tooltip("移動先のターゲット地点（必須）")]
    [SerializeField] private Transform targetPoint; 

    // ↓↓↓ ここに追加 ↓↓↓
    [Tooltip("戻る場所（オプション）。設定しない場合は元の場所に戻る。")]
    [SerializeField] private Transform customReturnPoint = null; 
    // ↑↑↑ ここまで追加 ↑↑↑

    [Header("検知設定")]
    [Tooltip("プレイヤーのタグ名")]
    [SerializeField] private string playerTag = "Player";

    private Rigidbody2D rb;
    private Vector3 originalPosition; // 元の場所
    private Vector3 originalScale;    // 元のスプライトの向き
    
    // 敵の状態を管理する
    private enum EnemyState
    {
        Idle,           // 待機中
        MovingToTarget, // ターゲット地点へ移動中
        Returning       // 元の場所へ戻り中
    }
    private EnemyState currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        // 元の場所と向きを保存
        originalPosition = transform.position;
        originalScale = transform.localScale;
        
        currentState = EnemyState.Idle;
        
        if (targetPoint == null)
        {
            Debug.LogError("PatrolOnPlayerEnter: targetPoint が設定されていません！", this);
        }
    }

    void Update()
    {
        if (targetPoint == null) return; // ターゲットがないと動けない

        // 現在の状態で処理を分岐する
        switch (currentState)
        {
            // ターゲット地点へ移動中の処理
            case EnemyState.MovingToTarget:
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPoint.position, // 目的地
                    moveSpeed * Time.deltaTime
                );
                
                // ターゲットの方向にスプライトを向ける
                FlipSprite(targetPoint.position.x > transform.position.x);

                // ターゲット地点に到着したら
                if (Vector2.Distance(transform.position, targetPoint.position) < 0.01f)
                {
                    // 状態を「戻り中」に変更
                    currentState = EnemyState.Returning;
                }
                break;
            }

            // 元の場所へ戻り中の処理
            case EnemyState.Returning:
            {
                // ↓↓↓ ここから変更 ↓↓↓

                // 戻るべき場所を決定する
                // customReturnPoint が設定されていればそこを、なければ元の場所(originalPosition) を目的地にする
                Vector3 returnDestination = (customReturnPoint != null) ? customReturnPoint.position : originalPosition;

                // 決定した目的地(returnDestination)へ移動
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    returnDestination, // 目的地
                    moveSpeed * Time.deltaTime
                );
                
                // 決定した目的地の方向にスプライトを向ける
                FlipSprite(returnDestination.x > transform.position.x);

                // 決定した目的地に到着したら
                if (Vector2.Distance(transform.position, returnDestination) < 0.01f)
                {
                    // 状態を「待機中」に戻す
                    currentState = EnemyState.Idle;
                }
                
                // ↑↑↑ ここまで変更 ↑↑↑
                break;
            }
            
            // 待機中の処理（何もしない）
            case EnemyState.Idle:
                break;
        }
    }
    
    // --- トリガー検知 ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーが検知範囲に入ったか
        // かつ、現在「待機中(Idle)」の場合のみ反応する
        if (other.CompareTag(playerTag) && currentState == EnemyState.Idle)
        {
            // 状態を「ターゲット地点へ移動中」に変更
            currentState = EnemyState.MovingToTarget;
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
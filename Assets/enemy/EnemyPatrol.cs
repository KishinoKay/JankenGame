using UnityEngine;

/// <summary>
/// 2つの地点（leftPoint と rightPoint）の間を往復移動するスクリプト
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("移動の目印")]
    [Tooltip("左側の移動限界地点")]
    [SerializeField] private Transform leftPoint;

    [Tooltip("右側の移動限界地点")]
    [SerializeField] private Transform rightPoint;

    private Transform target;       // 現在の目的地
    private Vector3 originalScale;  // 元のスプライトの向き

    void Start()
    {
        // 最初に右側（rightPoint）を目的地に設定
        target = rightPoint;
        
        // 元の向きを保存
        originalScale = transform.localScale;
    }

void Update()
    {
        // --- 修正点 1 ---
        // 目的地の Y 座標を、自分自身の現在の Y 座標に強制的に合わせる
        Vector2 targetPosition = new Vector2(
            target.position.x,      // 目的地の X 座標はそのまま
            transform.position.y    // Y 座標は自分の現在の Y 座標を使う
        );

        // 目的地（Y 座標を補正済み）に向かって移動
        transform.position = Vector2.MoveTowards(
            transform.position,             // 自分の現在位置
            targetPosition,                 // Y 座標を補正した目的地
            moveSpeed * Time.deltaTime      // 移動速度
        );

        // --- 修正点 2 ---
        // 目的地に到着したかどうかの判定を、Y 軸を無視して X 軸だけで行う
        // (Mathf.Abs は絶対値)
        if (Mathf.Abs(transform.position.x - target.position.x) < 0.1f)
        {
            // 目的地を切り替える
            if (target == rightPoint)
            {
                // 右端に着いたら、次は左端へ
                target = leftPoint;
                FlipSprite(false); // 左を向く
            }
            else
            {
                // 左端に着いたら、次は右端へ
                target = rightPoint;
                FlipSprite(true); // 右を向く
            }
        }
    }

    /// <summary>
    /// スプライトの向きを反転させる
    /// </summary>
    /// <param name="faceRight">右を向かせる場合は true</param>
    private void FlipSprite(bool faceRight)
    {
        // localScale の X の符号を変えることで、スプライトを反転させる
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
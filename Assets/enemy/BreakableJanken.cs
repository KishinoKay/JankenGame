using UnityEngine;

// クラス名を BreakableJanken に変更
public class BreakableJanken : MonoBehaviour
{
    // ★ 削除: myState, repulsionForce, deathHandler 
    // これらは通常のじゃんけん判定にしか使わないため不要になりました。

    [Header("グーでの破壊設定")]
    [Tooltip("プレイヤーがグー状態で、ここで設定したY軸速度（例: 5）より速く（下向きに）落下してきたら破壊される")]
    [SerializeField] private float breakThresholdVelocityY = 1f;

    // OnCollisionEnter2D メソッド（判定ロジックを修正）
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 相手が "Player" タグか確認
        if (!collision.gameObject.CompareTag("Player"))
        {
            return; 
        }

        // 2. 相手（プレイヤー）のスクリプトを取得
        PlayerStateController player = collision.gameObject.GetComponent<PlayerStateController>();
        if (player == null)
        {
            return; 
        }
        
        // 3. プレイヤーが「グー」状態か確認
        if (player.CurrentState == JankenState.Guu)
        {
            // 4. プレイヤーの Rigidbody2D を取得
            Rigidbody2D playerRb = collision.rigidbody; 
            if (playerRb != null)
            {
                float relativeVelocityY = collision.relativeVelocity.y;
                float threshold = -breakThresholdVelocityY;

                Debug.Log($"[判定] プレイヤーの相対Y速度: {relativeVelocityY} | しきい値: {threshold}");
                // 5. プレイヤーのY軸速度がしきい値より速い（下向き）かチェック
                if (relativeVelocityY < threshold)
                {
                    Debug.Log("プレイヤーのグーによる高速落下で破壊されました！");
                    OnLose(); // 自分（エネミー）を破壊
                }
            }
        }
        
        // 6. 通常の Judge() 呼び出しは削除
    }

    // ★ 削除: Judge() メソッドは不要になりました。

    // OnLose メソッド（破壊される処理は残す）
    public virtual void OnLose()
    {
        Debug.Log(gameObject.name + "は破壊されました。");
        Destroy(gameObject);
    }
    
    // ★ 削除: OnDraw() メソッドは不要になりました。
    
    // ★ 削除: OnWin() メソッドは不要になりました。
}
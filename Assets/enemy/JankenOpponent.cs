using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))] <-- 削除済み

public class JankenOpponent : MonoBehaviour
{
    [Header("このオブジェクトのじゃんけんタイプ")]
    public JankenState myState; // Inspectorで設定

    [Header("あいこの時の反発力")]
    [SerializeField] private float repulsionForce = 10f; // Inspectorで調整

    [Header("参照するマネージャー")]
    [Tooltip("PlayerDeathHandler スクリプトがアタッチされているオブジェクト（GameManagerなど）")]
    [SerializeField] private PlayerDeathHandler deathHandler;

    // Rigidbody2D と Awake() は削除済み

    // ★ 1. 衝突した瞬間に呼ばれる
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 判定処理を呼び出す
        HandleCollision(collision);
    }

    // ★ 2. 接触し続けている間、毎フレーム呼ばれる
    void OnCollisionStay2D(Collision2D collision)
    {
        // 判定処理を呼び出す
        HandleCollision(collision);
    }

    // ★ 3. 衝突判定の共通処理
    void HandleCollision(Collision2D collision)
    {
        // プレイヤー以外なら無視
        if (!collision.gameObject.CompareTag("Player"))
        {
            return; 
        }

        // プレイヤーのコンポーネントを取得
        PlayerStateController player = collision.gameObject.GetComponent<PlayerStateController>();
        if (player == null)
        {
            return; 
        }

        // プレイヤーの現在の状態を取得して判定
        JankenState playerState = player.CurrentState;
        Judge(player, playerState, collision);
    }


    // Judge メソッド (変更なし)
    void Judge(PlayerStateController player, JankenState playerState, Collision2D collision)
    {
        // あいこ
        if (myState == playerState)
        {
            OnDraw(collision); 
            return;
        }

        // プレイヤーの勝ち判定
        bool playerWins = 
            (playerState == JankenState.Guu && myState == JankenState.Choki) ||
            (playerState == JankenState.Choki && myState == JankenState.Paa) ||
            (playerState == JankenState.Paa && myState == JankenState.Guu);

        if (playerWins)
        {
            OnLose(); // 自分が負けた
        }
        else
        {
            OnWin(); // 自分が勝った (プレイヤーが負けた)
        }
    }

    // OnLose メソッド (変更なし)
    public virtual void OnLose()
    {
        Debug.Log(gameObject.name + "は負けた！ 破壊されます。");
        Destroy(gameObject);
    }
    
    // OnDraw メソッド (変更なし)
    public virtual void OnDraw(Collision2D collision)
    {
        Debug.Log(gameObject.name + "とプレイヤーはあいこ。プレイヤーを反発させます。");
        Rigidbody2D playerRb = collision.rigidbody;

        // myRb のチェックは削除済み
        if (playerRb != null) 
        {
            Vector2 directionToPlayer = (collision.transform.position - transform.position).normalized;
            playerRb.AddForce(directionToPlayer * repulsionForce, ForceMode2D.Impulse);
        }
    }
    
    // OnWin メソッド (変更なし)
    public virtual void OnWin()
    {
        Debug.Log(gameObject.name + "は勝った！ プレイヤーを倒す！");
        if (deathHandler != null)
        {
            deathHandler.TriggerDeath();
        }
        else
        {
            Debug.LogError(gameObject.name + " の Inspector で Death Handler が設定されていません！");
        }
    }
}
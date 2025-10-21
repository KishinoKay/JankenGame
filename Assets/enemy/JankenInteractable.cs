using UnityEngine;

public class JankenInteractable : MonoBehaviour
{
    [Header("チョキでの移動設定")]
    [Tooltip("プレイヤーがチョキで触れた時に移動させるオブジェクト（A地点）")]
    [SerializeField] private GameObject objectToMove;
    [Tooltip("オブジェクトの移動先の位置（B地点）を指定するTransform")]
    [SerializeField] private Transform targetPositionB;

    // ★ 1. アクションを一度だけ実行するためのフラグを追加
    private bool hasActionBeenPerformed = false;

    // ★ 2. メソッド名を OnTriggerEnter2D から OnTriggerStay2D に変更
    void OnTriggerStay2D(Collider2D other)
    {
        // 1. 相手が "Player" タグか確認
        if (!other.gameObject.CompareTag("Player"))
        {
            return; 
        }

        // 2. 相手（プレイヤー）のスクリプトを取得
        PlayerStateController player = other.gameObject.GetComponent<PlayerStateController>();
        if (player == null)
        {
            return; 
        }
        
        // 3. 状態の確認
        // ★ 3. 「チョキ」かつ「まだアクションを実行していない」場合のみ実行
        if (player.CurrentState == JankenState.Choki && !hasActionBeenPerformed)
        {
            // --- チョキの処理 ---
            HandleChokiAction();
        }
    }

    // ★ 4. (推奨) プレイヤーが離れたらフラグをリセットする処理
    // これにより、再度触れたときに再び反応できるようになります
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hasActionBeenPerformed = false;
        }
    }

    // チョキの処理（指定オブジェクトを移動させ、自分を非表示）
    private void HandleChokiAction()
    {
        // ★ 5. アクションを実行したことを記録
        hasActionBeenPerformed = true;

        Debug.Log("プレイヤーがチョキで触れました。");

        // 1. 指定されたオブジェクトをB地点に移動
        if (objectToMove != null && targetPositionB != null)
        {
            Debug.Log($"{objectToMove.name} を {targetPositionB.position} に移動させます。");
            objectToMove.transform.position = targetPositionB.position;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " のチョキ処理: Object To Move または Target Position B が Inspector で設定されていません。");
        }

        // 2. 触れたオブジェクト（自分自身）を非表示にする
        Debug.Log(gameObject.name + " を非表示にします。");
        gameObject.SetActive(false);
    }
}
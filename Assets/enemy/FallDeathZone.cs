using UnityEngine;

/// <summary>
/// プレイヤーが落下したことを検知し、死亡処理を呼び出すスクリプト。
/// シーンの下部などに配置した空のオブジェクトに、Collider2D(Is Trigger=ON) と共にアタッチする。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FallDeathZone : MonoBehaviour
{
    [Header("参照するマネージャー")]
    [Tooltip("PlayerDeathHandler スクリプトがアタッチされているオブジェクト（GameManagerなど）")]
    [SerializeField] private PlayerDeathHandler deathHandler;

    private void Awake()
    {
        // 念のため、アタッチされたコライダーがトリガーであることを確認
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogWarning(gameObject.name + " のコライダーが 'Is Trigger' ではありません。落下死判定が機能するよう、Is Trigger にチェックを入れてください。");
            col.isTrigger = true; // 強制的にトリガーにする
        }
    }

    // 他のオブジェクトがこのトリガーに入った瞬間に呼ばれる
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 入ってきたオブジェクトが "Player" タグを持っているか確認
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("落下死エリアにプレイヤーが侵入しました。");
            
            // 2. Inspector で設定された deathHandler の関数を呼び出す
            if (deathHandler != null)
            {
                // ★ 前の関数の使いまわし
                deathHandler.TriggerDeath();
            }
            else
            {
                Debug.LogError(gameObject.name + " の Inspector で Death Handler が設定されていません！");
            }
        }
    }
}
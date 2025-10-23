using UnityEngine;
using UnityEngine.InputSystem;

public enum JankenState
{
    Guu, Choki, Paa
}

[System.Serializable]
public class JankenStateData
{
    [Header("じゃんけんの種類（グー・チョキ・パー）")]
    public JankenState state;

    [Header("表示スプライト")]
    public Sprite sprite;

    [Header("重さ（gravityScale）")]
    public float gravityScale = 1f;

    // ↓↓↓ ここに追加 ↓↓↓
    [Header("水中の重さ（gravityScale）")]
    public float waterGravityScale = 0.5f; // ★追加（デフォルト値を0.5に設定）
    // ↑↑↑ ここまで追加 ↑↑↑

    [Header("地面の移動速度")]
    public float groundMoveSpeed = 5f;

    [Header("空中の移動速度")]
    public float airMoveSpeed = 4f;

    [Header("水中のジャンプ力")]
    [Tooltip("水中でのジャンプ（推進）力")]
    public float waterJumpForce = 8f;

    [Header("eyeの位置")]
    public Vector3 eyeLocalPosition;
}

public class PlayerStateController : MonoBehaviour
{
    public JankenState CurrentState { get; private set; }

    [Header("コンポーネント")]
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public Transform eye; // eyeオブジェクトをインスペクターで指定
    public PlayerController playerController;

    [Header("グーチョキパーごとの設定")]
    public JankenStateData[] stateDatas = new JankenStateData[3];

    // Player Inputコンポーネントから、このメソッドが呼ばれるようになる
    public void OnChangeState(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchNextState();
        }
    }

    void Start()
    {
        // PlayerController が同じオブジェクトにあると想定
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
        ChangeState(JankenState.Guu);
    }

    void SwitchNextState()
    {
        int nextStateIndex = (int)CurrentState + 1;
        if (nextStateIndex >= stateDatas.Length)
        {
            nextStateIndex = 0;
        }
        ChangeState((JankenState)nextStateIndex);
    }

    void ChangeState(JankenState newState)
    {
        CurrentState = newState;
        var data = GetStateData(newState);
        if (data != null)
        {
            spriteRenderer.sprite = data.sprite;

            // PlayerControllerの速度とジャンプ力を更新
            if (playerController != null)
            {
                playerController.groundMoveSpeed = data.groundMoveSpeed;
                playerController.airMoveSpeed = data.airMoveSpeed;
                playerController.waterJumpForce = data.waterJumpForce;

                // ↓↓↓ ここに追加 ↓↓↓
                // ★ステートごとの「水中の重さ」を PlayerController にも設定
                // (PlayerController側で水中判定時にこの値を使うことを想定)
                playerController.waterGravityScale = data.waterGravityScale;
                // ↑↑↑ ここまで追加 ↑↑↑
            }

            // 重力設定のロジック
            if (playerController != null && playerController.IsInWater())
            {
                // 水中にいる場合：★ステートごとの水中の重力を設定
                rb.gravityScale = data.waterGravityScale; // ★変更
            }
            else
            {
                // 陸上にいる場合：ステートごとの重力を設定
                rb.gravityScale = data.gravityScale;
            }

            if (eye != null)
            {
                eye.localPosition = data.eyeLocalPosition;
            }
        }
    }

    JankenStateData GetStateData(JankenState state)
    {
        foreach (var data in stateDatas)
        {
            if (data.state == state) return data;
        }
        return null;
    }

    // PlayerControllerから呼ばれるメソッド
    // 水から出たときに、現在のステートの重力に戻すために使う
    public void ApplyCurrentStateGravity()
    {
        var data = GetStateData(CurrentState);
        if (data != null)
        {
            // 現在のステートが持つ本来の重力（陸上用）を設定
            rb.gravityScale = data.gravityScale;
        }
    }

    // ↓↓↓ ここに追加 ↓↓↓
    // PlayerControllerから呼ばれるメソッド
    // 水に入ったときに、現在のステートの水中の重力にするために使う
    public void ApplyCurrentStateWaterGravity()
    {
        var data = GetStateData(CurrentState);
        if (data != null)
        {
            // 現在のステートが持つ水中の重力を設定
            rb.gravityScale = data.waterGravityScale;
        }
    }
    // ↑↑↑ ここまで追加 ↑↑↑
}
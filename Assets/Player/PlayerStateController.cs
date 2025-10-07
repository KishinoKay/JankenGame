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

    [Header("移動速度")]
    public float moveSpeed = 5f;

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
    public PlayerController playerController; // ← 追加

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
            rb.gravityScale = data.gravityScale;
            if (eye != null)
            {
                eye.localPosition = data.eyeLocalPosition;
            }
            // ここでPlayerControllerのmoveSpeedも更新
            if (playerController != null)
            {
                playerController.moveSpeed = data.moveSpeed;
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
}
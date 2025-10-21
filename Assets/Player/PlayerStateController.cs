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

    // ↓↓↓ ここを変更 ↓↓↓
    [Header("地面の移動速度")]
    public float groundMoveSpeed = 5f; // 分かりやすくリネーム

    [Header("空中の移動速度")] // ★追加
    public float airMoveSpeed = 4f; // ★追加 (空中は少し遅くするなど)
    // ↑↑↑ ここまで変更 ↑↑↑

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

            // ↓↓↓ ここを変更 ↓↓↓
            // PlayerControllerの「地面」と「空中」の速度を両方更新
            if (playerController != null)
            {
                playerController.groundMoveSpeed = data.groundMoveSpeed; // 地面の速度を設定
                playerController.airMoveSpeed = data.airMoveSpeed;     // 空中の速度を設定
            }
            // ↑↑↑ ここまで変更 ↑↑↑
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
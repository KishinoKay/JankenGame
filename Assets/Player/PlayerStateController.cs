using UnityEngine;
using UnityEngine.InputSystem; // インプットシステムを使うために必要！

public enum JankenState
{
    Guu, Choki, Paa
}

public class PlayerStateController : MonoBehaviour
{
    public JankenState CurrentState { get; private set; }

    [Header("コンポーネント")]
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    [Header("状態ごとの設定")]
    public Sprite[] stateSprites;

    // Player Inputコンポーネントから、このメソッドが呼ばれるようになる
    public void OnChangeState(InputAction.CallbackContext context)
    {
        // context.performed は「ボタンが押された瞬間」を意味する
        if (context.performed)
        {
            Debug.Log("返信！");
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
        if (nextStateIndex >= 3)
        {
            nextStateIndex = 0;
        }
        ChangeState((JankenState)nextStateIndex);
    }

    void ChangeState(JankenState newState)
    {
        CurrentState = newState;
        spriteRenderer.sprite = stateSprites[(int)CurrentState];
        switch (CurrentState)
        {
            case JankenState.Guu:
                rb.gravityScale = 5f;
                break;
            case JankenState.Choki:
                rb.gravityScale = 1f;
                break;
            case JankenState.Paa:
                rb.gravityScale = 0.8f;
                break;
        }
    }
}
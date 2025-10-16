using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("目の操作")]
    [Tooltip("操作したいPupilControllerをここに設定")]
    public PupilController pupilController; // ★追加：PupilControllerへの参照

    [Header("ゲーム管理")]
    [Tooltip("ゲーム管理を行うGameManagerをここに設定")]
    public GameManager GameManager; // ★追加：GameManagerへの参照

    [Header("移動速度")]
    public float moveSpeed = 5f;

    [Header("ジャンプ力")]
    public float jumpForce = 10f;

    [Header("接地判定")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Updateは入力受付など、フレーム毎の処理に使う
    void Update()
    {
        // ここでは何もしない
    }

    // FixedUpdateは物理演算など、一定間隔の処理に使う
    private void FixedUpdate()
    {
        // 接地判定をFixedUpdateに移動
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 移動処理
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        // pupilControllerが設定されているか確認
        if (pupilController != null)
        {
            // 読み取った入力値をPupilControllerのメソッドに渡す
            Vector2 lookInput = context.ReadValue<Vector2>();
            pupilController.UpdatePupilPosition(lookInput);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // ジャンプの入力受付自体はUpdateのタイミングに近いですが、
        // isGroundedの判定がFixedUpdateで正確に行われるため、ここはそのままでOK
        if (context.performed && isGrounded)
        {
            Debug.Log("jannpu！");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // ★追加：Pauseの入力受付メソッド
    public void OnPause(InputAction.CallbackContext context)
    {
        // Pauseの処理はGameManagerに任せる
        Debug.Log("Pause");
        if (GameManager != null)
        {
            GameManager.OnPauseButton(context);
        }
    }
}

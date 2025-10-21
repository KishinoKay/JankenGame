using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("目の操作")]
    [Tooltip("操作したいPupilControllerをここに設定")]
    public PupilController pupilController;

    [Header("ゲーム管理")]
    [Tooltip("ゲーム管理を行うGameManagerをここに設定")]
    public GameManager GameManager;

    // ↓↓↓ ここを変更 ↓↓↓
    [Header("地面の移動速度")]
    [Tooltip("（PlayerStateControllerによって自動設定されます）")]
    public float groundMoveSpeed = 5f; // groundMoveSpeed に変更

    [Header("空中の移動速度")] // ★追加
    [Tooltip("（PlayerStateControllerによって自動設定されます）")]
    public float airMoveSpeed = 4f; // ★追加
    // ↑↑↑ ここまで変更 ↑↑↑

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

    void Update()
    {
        // ここでは何もしない
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // ↓↓↓ ここを変更 ↓↓↓
        // 1. 現在適用すべき移動速度を決定する
        float currentMoveSpeed;
        if (isGrounded)
        {
            currentMoveSpeed = groundMoveSpeed; // 地面にいれば地面の速度
        }
        else
        {
            currentMoveSpeed = airMoveSpeed; // 空中にいれば空中の速度
        }

        // 2. 決定した速度で移動処理
        rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y);
        // ↑↑↑ ここまで変更 ↑↑↑
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (pupilController != null)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();
            pupilController.UpdatePupilPosition(lookInput);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            Debug.Log("jannpu！");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Debug.Log("Pause");
        if (GameManager != null)
        {
            GameManager.OnPauseButton(context);
        }
    }
}
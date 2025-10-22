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

    [Header("ステート管理")]
    [Tooltip("PlayerStateControllerをここに設定（同じオブジェクトにあれば自動取得）")]
    public PlayerStateController playerStateController; 

    [Header("地面の移動速度")]
    [Tooltip("（PlayerStateControllerによって自動設定されます）")]
    public float groundMoveSpeed = 5f;

    [Header("空中の移動速度")]
    [Tooltip("（PlayerStateControllerによって自動設定されます）")]
    public float airMoveSpeed = 4f;

    [Header("ジャンプ力")]
    [Tooltip("陸上にいる時のジャンプ力")]
    public float jumpForce = 10f; // 陸上用

    // ↓↓↓ ここに追加 ↓↓↓
    [Header("水中のジャンプ力")]
    [Tooltip("（PlayerStateControllerによって自動設定されます）")]
    public float waterJumpForce = 8f; // ★水中用（ステートごとに変わる）
    // ↑↑↑ ここまで追加 ↑↑↑

    [Header("接地判定")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("水中設定")]
    [Tooltip("水中の重力（0に近いほど宇宙的）")]
    public float waterGravityScale = 0.1f;
    [Tooltip("「パー」の時の水中の浮力")]
    public float paaBuoyancy = 5f;
    [Tooltip("水中判定に使用するタグ")]
    public string waterTag = "Water";

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private int waterTriggerCount = 0; // 水中カウント

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // PlayerStateController が同じオブジェクトにあると想定
        if (playerStateController == null)
        {
            playerStateController = GetComponent<PlayerStateController>();
        }
    }

    void Update()
    {
        // ここでは何もしない
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 1. 移動速度の決定
        float currentMoveSpeed;
        if (isGrounded)
        {
            currentMoveSpeed = groundMoveSpeed; 
        }
        else
        {
            currentMoveSpeed = airMoveSpeed; 
        }

        // 2. 移動処理
        rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y);

        // 3. 水中処理
        if (waterTriggerCount > 0) 
        {
            // 「パー」の時の浮力処理
            if (playerStateController != null && playerStateController.CurrentState == JankenState.Paa)
            {
                rb.AddForce(Vector2.up * paaBuoyancy, ForceMode2D.Force);
            }
        }
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
        // ↓↓↓ ここを変更 ↓↓↓
        if (context.performed)
        {
            // 判定を「陸上」と「水中」で分ける
            
            if (isGrounded) // 1. 陸上にいる場合
            {
                Debug.Log("jannpu！(陸上)");
                // Y軸の速度をリセット（ジャンプの安定化）
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                // ★陸上のジャンプ力(jumpForce)を使用
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); 
            }
            else if (waterTriggerCount > 0) // 2. 水中にいる場合 (陸上ではない)
            {
                Debug.Log("jannpu！(水中)");
                // Y軸の速度をリセット（水中推進のため）
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                // ★ステートごとの水中のジャンプ力(waterJumpForce)を使用
                rb.AddForce(Vector2.up * waterJumpForce, ForceMode2D.Impulse); 
            }
            // (どちらでもない場合＝空中ではジャンプしない)
        }
        // ↑↑↑ ここまで変更 ↑↑↑
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Debug.Log("Pause");
        if (GameManager != null)
        {
            GameManager.OnPauseButton(context);
        }
    }

    // 水中判定（トリガー）
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(waterTag))
        {
            waterTriggerCount++;
            if (waterTriggerCount == 1)
            {
                rb.gravityScale = waterGravityScale; 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(waterTag))
        {
            waterTriggerCount--;
            if (waterTriggerCount == 0)
            {
                if (playerStateController != null)
                {
                    playerStateController.ApplyCurrentStateGravity();
                }
            }
        }
    }

    // PlayerStateController が水中かどうかを判別するために使用
    public bool IsInWater()
    {
        return waterTriggerCount > 0;
    }
}
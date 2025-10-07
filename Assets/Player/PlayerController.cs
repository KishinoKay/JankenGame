using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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
}
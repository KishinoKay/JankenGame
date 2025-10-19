using UnityEngine;
using UnityEngine.InputSystem; // <-- これを追加！

/// <summary>
/// マウスカーソルやスティックの入力に応じて、UIを滑らかに動かしたり傾けたりする
/// (Input System パッケージ版)
/// </summary>
public class UIParallaxEffect : MonoBehaviour
{
    // (Inspectorの設定項目は変更なし)
    [Header("対象のUI")]
    [Tooltip("このRectTransformを動かします")]
    [SerializeField]
    private RectTransform targetRect;

    [Header("移動（パララックス）設定")]
    [SerializeField]
    private float moveIntensity = 15f; 

    [Header("傾き（チルト）設定")]
    [SerializeField]
    private float tiltIntensity = 5f;

    [Header("スムーズ設定")]
    [SerializeField]
    private float smoothSpeed = 8f;


    private Vector2 initialPosition;
    private Quaternion initialRotation;
    private Vector2 targetPosition;
    private Quaternion targetRotation;


    void Start()
    {
        if (targetRect == null)
        {
            targetRect = GetComponent<RectTransform>();
        }
        initialPosition = targetRect.anchoredPosition;
        initialRotation = targetRect.localRotation;
    }

    void Update()
    {
        // 1. 正規化された入力値（-1.0 ～ 1.0）を取得する
        //    (Input System版のメソッドを呼び出す)
        Vector2 normalizedInput = GetNormalizedInput(); // エラー箇所 (95行目)

        // (2. 3. 4. の処理は変更なし)
        
        // 2. 目標位置を計算
        float targetPosX = initialPosition.x + (normalizedInput.x * moveIntensity);
        float targetPosY = initialPosition.y + (normalizedInput.y * moveIntensity);
        targetPosition = new Vector2(targetPosX, targetPosY);

        // 3. 目標角度を計算
        Quaternion targetRotX = Quaternion.Euler(-normalizedInput.y * tiltIntensity, 0, 0);
        Quaternion targetRotY = Quaternion.Euler(0, normalizedInput.x * tiltIntensity, 0);
        targetRotation = initialRotation * targetRotX * targetRotY;
        
        // 4. 現在位置/角度を、目標位置/角度に向かって滑らかに補間
        targetRect.anchoredPosition = Vector2.Lerp(
            targetRect.anchoredPosition, 
            targetPosition, 
            Time.deltaTime * smoothSpeed
        );

        targetRect.localRotation = Quaternion.Slerp(
            targetRect.localRotation, 
            targetRotation, 
            Time.deltaTime * smoothSpeed
        );
    }


    /// <summary>
    /// 【修正版】Input System を使って -1.0 ～ 1.0 の入力値を取得する
    /// </summary>
    private Vector2 GetNormalizedInput()
    {
        // --- スティック入力 (優先) ---
        // 現在接続されているゲームパッドを取得
        Gamepad gamepad = Gamepad.current; 
        if (gamepad != null)
        {
            // 左スティックの入力をVector2で取得
            Vector2 stickInput = gamepad.leftStick.ReadValue(); 
            
            // ある程度の入力（デッドゾーン）があるかチェック
            // (Input System側でDeadzoneを設定しているなら不要な場合もある)
            if (stickInput.magnitude > 0.1f)
            {
                return stickInput; // スティック入力を返す
            }
        }

        // --- スティック入力がなければマウス位置を使用 ---
        // 現在のマウスを取得
        Mouse mouse = Mouse.current; 
        if (mouse == null)
        {
            // マウスが接続されていない場合は (0, 0) = 中央 として扱う
            return Vector2.zero;
        }

        // マウスの現在位置（スクリーン座標）を取得
        Vector2 mousePos = mouse.position.ReadValue(); 

        // マウス位置を画面中心 (0, 0) とし、画面端 (-1.0 ～ +1.0) に正規化
        float normalizedX = (mousePos.x - (Screen.width / 2f)) / (Screen.width / 2f);
        float normalizedY = (mousePos.y - (Screen.height / 2f)) / (Screen.height / 2f);

        // 値を -1f ～ 1f の範囲に制限
        normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);
        normalizedY = Mathf.Clamp(normalizedY, -1f, 1f);

        return new Vector2(normalizedX, normalizedY);
    }
}
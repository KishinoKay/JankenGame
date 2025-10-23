using UnityEngine;
using UnityEngine.InputSystem;

public class WorldSpaceControlSwitcher : MonoBehaviour
{
    [Header("監視対象")]
    [Tooltip("シーン内の PlayerInput コンポーネント")]
    [SerializeField] private PlayerInput playerInput;

    [Header("スプライトの設定")]
    [Tooltip("スプライトを切り替えたい SpriteRenderer")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Tooltip("キーボード・マウス操作時に表示するスプライト")]
    [SerializeField] private Sprite keyboardMouseSprite;

    [Tooltip("ゲームパッド操作時に表示するスプライト")]
    [SerializeField] private Sprite gamepadSprite;

    // 最後に操作されたデバイスの種類を保持する
    private enum LastDeviceType { Unknown, KeyboardMouse, Gamepad }
    private LastDeviceType lastDevice = LastDeviceType.Unknown;

    void Start()
    {
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput が設定されていません！", this);
            return;
        }

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }
        
        if (targetRenderer == null)
        {
             Debug.LogError("Target Renderer が設定されていません！", this);
             return;
        }

        // 1. スキーム切り替え(onControlsChanged)の代わりに、
        //    全アクションの入力(onActionTriggered)をリッスンする
        playerInput.onActionTriggered += OnActionTriggered;

        // 2. 初期状態としてキーボード/マウスのUIを仮設定
        // (最初にコントローラーを触ればすぐに切り替わる)
        UpdateSprite(LastDeviceType.KeyboardMouse);
        lastDevice = LastDeviceType.KeyboardMouse;
    }

    void OnDestroy()
    {
        // 登録したイベントを解除
        if (playerInput != null)
        {
            playerInput.onActionTriggered -= OnActionTriggered;
        }
    }

    /// <summary>
    /// PlayerInput のいずれかのアクションが実行されたときに毎回呼ばれる
    /// </summary>
private void OnActionTriggered(InputAction.CallbackContext context)
    {
        // 1. デバイスを取得
        InputDevice device = context.control.device;

        if (device == null)
        {
            // デバイスが取得できない場合（非常に稀）
            Debug.LogWarning("入力デバイスが null です。");
            return;
        }
        
        // 2. ★デバイスの「レイアウト名」を取得
        string deviceLayout = device.layout;
        
        // ★★★ デバッグログ ★★★
        // Consoleウィンドウに「入力デバイス検知: DualShock4GamepadHID (Layout: DualShock4GamepadHID)」のように表示されます
        Debug.Log($"入力デバイス検知: {device.displayName} (Layout: {deviceLayout})");
        // ★★★ ここまで ★★★


        // 3. デバイスの種類を「レイアウト名（文字列）」で判別
        LastDeviceType currentDeviceType;

        // "DualShock4GamepadHID" や "XInputControllerWindows" には "Gamepad" が含まれる
        // "Joystick" もコントローラーとして扱う
        if (deviceLayout.Contains("Gamepad") || deviceLayout.Contains("Joystick"))
        {
            currentDeviceType = LastDeviceType.Gamepad;
        }
        else if (deviceLayout.Contains("Keyboard") || deviceLayout.Contains("Mouse"))
        {
            currentDeviceType = LastDeviceType.KeyboardMouse;
        }
        else
        {
             // Input Debuggerで見た "Pen" などが該当
            Debug.Log($"無視するデバイスレイアウト: {deviceLayout}");
            return;
        }

        // 4. 最後に操作したデバイスの種類と「違う」場合のみ、スプライトを更新
        if (currentDeviceType != lastDevice)
        {
            Debug.Log($"デバイスタイプ変更: {lastDevice} -> {currentDeviceType}");
            UpdateSprite(currentDeviceType);
            lastDevice = currentDeviceType;
        }
    }

    /// <summary>
    /// デバイスの種類に応じてスプライトを切り替える
    /// </summary>
    private void UpdateSprite(LastDeviceType deviceType)
    {
        if (deviceType == LastDeviceType.Gamepad)
        {
            targetRenderer.sprite = gamepadSprite;
            // Debug.Log("デバイスを [Gamepad] としてスプライト変更");
        }
        else if (deviceType == LastDeviceType.KeyboardMouse)
        {
            targetRenderer.sprite = keyboardMouseSprite;
            // Debug.Log("デバイスを [Keyboard/Mouse] としてスプライト変更");
        }
    }
}
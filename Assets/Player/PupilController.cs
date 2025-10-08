using UnityEngine;

// InputSystemは不要なので削除
// using UnityEngine.InputSystem; 

public class PupilController : MonoBehaviour
{
    // インスペクターから動かしたい「黒目」のTransformを割り当てる
    public Transform pupilTransform;

    // 白目の半径。黒目が動ける範囲を決める
    [Tooltip("黒目が動ける範囲（白目の半径）")]
    public float eyeballRadius = 0.5f;

    // private Vector2 moveDirection; // 司令塔から直接値を受け取るので不要

    void Start()
    {
        // ゲーム開始時に念のため黒目を中央に配置
        if (pupilTransform != null)
        {
            pupilTransform.localPosition = Vector3.zero;
        }
    }

    // OnLookはPlayerControllerが受け持つので削除

    // UpdateもPlayerControllerから直接指示されるので不要

    // PlayerControllerから呼び出してもらうための公開メソッドを新しく作成
    public void UpdatePupilPosition(Vector2 lookInput)
    {
        if (pupilTransform == null)
        {
            return;
        }
        
        // スティックの入力方向(lookInput)に、動ける範囲(eyeballRadius)を掛け合わせる
        Vector2 newPupilPosition = lookInput * eyeballRadius;

        // 計算した位置を黒目のlocalPositionに設定する
        pupilTransform.localPosition = newPupilPosition;
    }
}
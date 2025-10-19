using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    // パララックスの強さ。0に近いほど奥（ゆっくり動く）、1に近いほど手前（速く動く）。
    // 例えば、遠景は 0.1, 中景は 0.5 などと設定する。
    [SerializeField]
    private float parallaxFactor; 

    void Start()
    {
        // メインカメラのTransformを取得
        cameraTransform = Camera.main.transform;
        // 最初のカメラ位置を記憶
        lastCameraPosition = cameraTransform.position;
    }

    // カメラの移動処理が終わった後に呼び出される LateUpdate を使うのが一般的
    void LateUpdate()
    {
        // カメラがどれだけ動いたか（差分）を計算
        float deltaX = cameraTransform.position.x - lastCameraPosition.x;

        // 背景をカメラの移動量 * パララックス係数 の分だけ動かす
        transform.position += new Vector3(deltaX * parallaxFactor, 0, 0);

        // カメラの現在位置を次のフレームのために記憶
        lastCameraPosition = cameraTransform.position;
    }
}
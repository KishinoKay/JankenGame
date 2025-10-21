using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cameraTransform;
    
    // ★追加：スタート時のそれぞれの位置を記憶する変数
    private Vector3 startCameraPosition;
    private Vector3 startBackgroundPosition;

    // パララックスの強さ。
    // 0 = 全く動かない (一番奥の空など)
    // 0.1 ~ 0.8 = 奥の景色 (推奨)
    // 1 = カメラと全く同じに動く (プレイヤーと同じ)
    [SerializeField]
    private float parallaxFactor; 

    void Start()
    {
        cameraTransform = Camera.main.transform;
        
        // ★変更：スタート時の位置を記憶
        startCameraPosition = cameraTransform.position;
        startBackgroundPosition = transform.position;
    }

    // カメラの移動処理が終わった後に呼び出される LateUpdate
    void LateUpdate()
    {
        // ★★★ ロジックを大幅に変更 ★★★

        // 1. スタート地点からカメラがどれだけ動いたか（X軸）
        float cameraDeltaX = cameraTransform.position.x - startCameraPosition.x;
        
        // 2. それにFactorを掛けた分だけ、背景がスタート地点から動くべき位置を計算
        float newX = startBackgroundPosition.x + (cameraDeltaX * parallaxFactor);

        // 3. 背景のY軸とZ軸は元の位置を維持しつつ、X軸だけを更新
        // (もしY軸も視差効果をつけたい場合は、Y軸も同様に計算します)
        transform.position = new Vector3(newX, startBackgroundPosition.y, startBackgroundPosition.z);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使うために必要

// SceneTransitionButtonの機能をすべて引き継ぐ
public class ReloadSceneButton : SceneTransitionButton
{
    /// <summary>
    /// 親のOnClickメソッドの処理を、シーンリロード処理で上書き(override)する
    /// </summary>
    protected override void OnClick()
    {
        // 2. 現在のシーンをリロードする
        SceneTransition.Instance.StartTransition(SceneManager.GetActiveScene().name, buttonSound);
    }
}
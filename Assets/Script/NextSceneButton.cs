using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使うために必要

// SceneTransitionButtonの機能をすべて引き継ぐ
public class NextSceneButton : SceneTransitionButton
{
    /// <summary>
    /// 親のOnClickメソッドの処理を、次のステージのシーンをロードする処理で上書き(override)する
    /// </summary>
    protected override void OnClick()
    {
        // 1. 現在のシーン名を取得 (例: "Stage1")
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // 2. シーン名から "Stage" という文字列を取り除く (例: "1")
        string stageNumberString = currentSceneName.Replace("Stage", "");
        
        int currentStageNumber;
        
        // 3. 文字列 (例: "1") を数値 (例: 1) に変換できるか試す
        if (int.TryParse(stageNumberString, out currentStageNumber))
        {
            // --- 変換に成功した場合 (Stage1, Stage2 など) ---
            
            // 4. 次のステージ番号を計算 (例: 1 + 1 = 2)
            int nextStageNumber = currentStageNumber + 1;
            
            // 5. 次のシーン名を構築 (例: "Stage2")
            string nextSceneName = "Stage" + nextStageNumber;

            // 6. ★【変更点】次のシーンがビルド設定に存在するかチェック★
            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                // 7-a. [存在する場合] 次のシーンへ遷移
                SceneTransition.Instance.StartTransition(nextSceneName, buttonSound);
            }
            else
            {
                // 7-b. [存在しない場合] (例: "Stage5" がない場合)
                // 最後のステージをクリアした時など、
                // 親クラスで設定された sceneToLoad (インスペクターで指定したシーン) へ遷移
                Debug.Log($"次のシーン '{nextSceneName}' がビルド設定に見つかりません。代わりに '{sceneToLoad}' へ遷移します。");
                SceneTransition.Instance.StartTransition(sceneToLoad, buttonSound);
            }
        }
        else
        {
            // --- 変換に失敗した場合 (例: "TitleScene" など) ---
            
            // "Stage" + 数値 の形式でないシーンで押された場合の処理
            Debug.LogWarning($"シーン名 {currentSceneName} からステージ番号を解析できませんでした。");
            
                Debug.Log($"ステージ名がStageではありません。代わりに '{sceneToLoad}' へ遷移します。");
                SceneTransition.Instance.StartTransition(sceneToLoad, buttonSound);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems; // EventSystemを扱うために必要

public class SelectFirstButton : MonoBehaviour
{
    // インスペクターから最初に選択したいボタンをセットする
    public GameObject firstSelectedButton;

    void OnEnable()
    {
        // このUIが表示されたときに呼ばれる
        // 少し待ってから選択することで、UIの準備が整うのを待つ
        StartCoroutine(SelectButtonAfterFrame());
    }

    private System.Collections.IEnumerator SelectButtonAfterFrame()
    {
        // 1フレーム待つ
        yield return null; 
        
        // EventSystemの現在の選択対象をクリア
        EventSystem.current.SetSelectedGameObject(null);
        // 新しく選択対象を設定
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}
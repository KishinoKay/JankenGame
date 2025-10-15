using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneTransitionButton : MonoBehaviour
{
    [Header("遷移設定")]
    [Tooltip("読み込むシーンの名前")]
    [SerializeField] protected string sceneToLoad; // privateからprotectedへ変更

    [Header("サウンド設定")]
    [Tooltip("再生するボタンのSE")]
    [SerializeField] protected AudioClip buttonSound; // privateからprotectedへ変更

    protected virtual void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    protected virtual void OnClick()
    {
        // SceneTransitionマネージャーを呼び出す
        SceneTransition.Instance.StartTransition(sceneToLoad, buttonSound);
    }
}
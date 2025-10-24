using UnityEngine;
using System.Collections; // コルーチンを使用するために必要

public class WindController : MonoBehaviour
{
    [Header("風のタイミング設定")]
    [Tooltip("風が吹き始めるまでの間隔（秒）")]
    public float windInterval = 10.0f;

    [Tooltip("風が吹いている時間（秒）")]
    public float windDuration = 3.0f;

    [Header("風の強さ")]
    [Tooltip("プレイヤーを押し返す力の強さ")]
    public float windForce = 50.0f;

    [Header("関連オブジェクト")]
    [Tooltip("再生する風のエフェクト（パーティクルシステム）")]
    public ParticleSystem windEffect; // インスペクターから風エフェクトを割り当てる

    private Rigidbody2D playerRb;     // プレイヤーのRigidbody
    private bool isWindBlowing = false; // 現在、風が吹いているか

void Start()
    {
        Debug.Log("Start: プレイヤーを探しています...");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // ★ 見つけたオブジェクトの名前をログに出します
            Debug.Log("Start: '" + player.name + "' という名前の 'Player' タグのオブジェクトを見つけました。");
            
            playerRb = player.GetComponent<Rigidbody2D>();
            
            // ★ Rigidbody 2D が取得できたか、ここで判定します
            if (playerRb != null)
            {
                Debug.Log("Start: Rigidbody 2D を正常に取得しました。");
            }
            else
            {
                Debug.LogWarning("Start: '" + player.name + "' オブジェクトに Rigidbody 2D が見つかりません！");
            }
        }
        else
        {
            Debug.LogWarning("WindController: 'Player' タグのオブジェクトがシーン内に見つかりません。");
        }

        // 風エフェクトが設定されていれば、最初は停止しておく
        if (windEffect != null)
        {
            windEffect.Stop();
        }

        // 風の発生サイクルを開始する
        StartCoroutine(WindCycle());
        Debug.Log("Start: WindCycleコルーチンを開始します。");
    }

void FixedUpdate()
    {
        // このメソッドは物理演算のタイミングで呼ばれる

        // もし風が吹いていて、プレイヤーのRigidbodyが取得できていれば
        if (isWindBlowing && playerRb != null)
        {
            // --- 修正箇所 開始 ---

            // 1. 現在の重力スケールを取得
            float currentGravityScale = playerRb.gravityScale;

            // 2. 0除算や逆風を防ぐため、gravityScaleが0以下の場合は 1 として扱う
            if (currentGravityScale <= 0f)
            {
                currentGravityScale = 1f;
            }

            // 3. 本来の風の力を「重力スケール」で割って、調整後の力を計算
            // (gravityScaleが大きいほど、風の力は弱くなる)
            float adjustedWindForce = windForce / currentGravityScale;

            // 4. 調整後の力で AddForce を実行
            playerRb.AddForce(-Vector2.right * adjustedWindForce, ForceMode2D.Force);

            // デバッグログも調整後の力を見るように変更
            // Debug.Log("AddForce実行中！ 力: " + windForce); // ← 元のコード
            Debug.Log("AddForce実行中！ 調整後の力: " + adjustedWindForce); // ← 修正後

            // --- 修正箇所 終了 ---
        }
    }
    /// <summary>
    /// 風の発生サイクルを管理するコルーチン
    /// </summary>
    private IEnumerator WindCycle()
    {
        // ゲームが続いている間、無限にループ
        while (true)
        {
            // 1. 次の風が吹くまで待機 (windInterval秒 待つ)
            yield return new WaitForSeconds(windInterval);

            // 2. 風を開始
            StartWind();

            // 3. 風が吹いている間待機 (windDuration秒 待つ)
            yield return new WaitForSeconds(windDuration);

            // 4. 風を停止
            StopWind();
        }
    }
/// <summary>
    /// 風を開始する処理
    /// </summary>
    private void StartWind()
    {
        isWindBlowing = true;

        if (windEffect != null)
        {
            windEffect.Play(); // パーティクルエフェクトを再生
        }
        
        // ★★★ 3. StartWindが呼ばれたかログを追加 ★★★
        Debug.Log("StartWind: 風を開始！ isWindBlowing = true にしました。");
    }

    /// <summary>
    /// 風を停止する処理
    /// </summary>
    private void StopWind()
    {
        isWindBlowing = false;

        if (windEffect != null)
        {
            windEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        
        // ★★★ 4. StopWindが呼ばれたかログを追加 ★★★
        Debug.Log("StopWind: 風を停止！ isWindBlowing = false にしました。");
    }
}
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ゲーム起動時に設定を読み込み、シーンをまたいで適用し続けるクラス
/// （シングルトン）
/// </summary>
public class SettingsApplicator : MonoBehaviour
{
    public static SettingsApplicator Instance { get; private set; }

    [Header("オーディオ")]
    [SerializeField]
    private AudioMixer audioMixer; // 必ずインスペクターで設定！

    // (PlayerPrefsのキー定義はそのまま)
    private const string KEY_VOLUME = "MASTER_VOLUME";
    private const string KEY_FULLSCREEN = "IS_FULLSCREEN";
    private const string KEY_QUALITY = "QUALITY_INDEX";

    private void Awake()
    {
        // --- シングルトン化 (これはAwake()に残す) ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
        }
        else
        {
            // すでに起動済みの Applicator があれば、新しい方は破棄
            Destroy(gameObject);
            return;
        }

        // --- ★★★ 削除 ★★★ ---
        // LoadAndApplyAllSettings(); // Awake() から呼び出すのをやめる
        // --- ★★★ 削除 ★★★ ---
    }

    // --- ★★★ Start() メソッドを新規追加 ★★★ ---
    private void Start()
    {
        // --- ゲーム起動時にすべての設定を読み込んで適用 ---
        // Awake()より後、最初のUpdateの前に実行される
        LoadAndApplyAllSettings();
    }
    // --- ★★★ 新規追加ここまで ★★★ ---


    /// <summary>
    /// PlayerPrefsからすべての設定を読み込み、ゲームに適用する
    /// </summary>
    public void LoadAndApplyAllSettings()
    {
        // (デバッグログを仕込んでおくと確実です)
        Debug.Log("--- LoadAndApplyAllSettings() 実行 ---");

        // 1. 画質 (デフォルトは 3)
        SetQuality(PlayerPrefs.GetInt(KEY_QUALITY, 3));

        // 2. フルスクリーン (デフォルトは 1 = true)
        SetFullscreen(PlayerPrefs.GetInt(KEY_FULLSCREEN, 1) == 1);

        // 3. 音量 (デフォルトは 0.5)
        float loadedVolume = PlayerPrefs.GetFloat(KEY_VOLUME, 0.5f);
        Debug.Log($"PlayerPrefsから音量を読み込み: {loadedVolume}");
        SetMasterVolume(loadedVolume);
    }


    // --- 外部（やSettingsManager）から呼び出すための適用ロジック ---

    /// <summary>
    /// AudioMixerの音量を設定する
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        // スライダーの値が0になると-80dB（ほぼ無音）になるようLog10で変換
        float db = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        
        if (audioMixer != null)
        {
            // (デバッグログ)
            Debug.Log($"AudioMixerにデシベル値: {db} dB (元 {volume}) を設定します。");
            audioMixer.SetFloat("MasterVolume", db);
        }
        else
        {
            Debug.LogError("AudioMixerが設定されていません！");
        }
    }
    /// <summary>
    /// フルスクリーンモードを設定する
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    /// <summary>
    /// 画質レベルを設定する
    /// </summary>
    public void SetQuality(int qualityIndex)
    {
        // QualitySettings.namesの範囲外のインデックスが来ないようにガード
        if (qualityIndex >= 0 && qualityIndex < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.Audio; // <-- AudioMixerの参照は不要になる
using TMPro;

/// <summary>
/// 【修正版】設定「シーン」のUIを管理し、変更を保存・通知する
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("UIコンポーネント")]
    // [SerializeField]
    // private AudioMixer audioMixer; // <-- 削除 (Applicatorが担当)

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private Toggle fullscreenToggle;

    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    // PlayerPrefsに保存する際のキー（Applicatorと合わせる）
    private const string KEY_VOLUME = "MASTER_VOLUME";
    private const string KEY_FULLSCREEN = "IS_FULLSCREEN";
    private const string KEY_QUALITY = "QUALITY_INDEX";


    /// <summary>
    /// この「設定画面が」開かれた時に、現在の設定値をUIに反映する
    /// </summary>
    void Start()
    {
        // ----- 画質ドロップダウンの初期設定 -----
        qualityDropdown.ClearOptions();
        List<string> options = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(options);

        // ----- 現在保存されている設定値を「UIに反映」する -----
        // (適用ロジックは Applicator が起動時に行っているので、ここではUIに表示するだけ)

        // 1. 画質
        int qualityIndex = PlayerPrefs.GetInt(KEY_QUALITY, 3);
        qualityDropdown.value = qualityIndex; // UIの表示を更新

        // 2. フルスクリーン
        bool isFullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 1) == 1;
        fullscreenToggle.isOn = isFullscreen;

        // 3. 音量
        float volume = PlayerPrefs.GetFloat(KEY_VOLUME, 0.5f);
        volumeSlider.value = volume;


        // ----- UIが操作された時にメソッドを呼ぶよう「リスナー」を設定 -----
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    // --- UI操作時に呼び出されるメソッド ---

    /// <summary>
    /// スライダーが操作されたら「保存」し、「Applicatorに通知」する
    /// </summary>
    private void OnVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat(KEY_VOLUME, volume);
        SettingsApplicator.Instance.SetMasterVolume(volume); // Applicatorに即時適用を依頼
    }

    /// <summary>
    /// トグルが操作されたら「保存」し、「Applicatorに通知」する
    /// </summary>
    private void OnFullscreenChanged(bool isFullscreen)
    {
        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFullscreen ? 1 : 0);
        SettingsApplicator.Instance.SetFullscreen(isFullscreen); // Applicatorに即時適用を依頼
    }

    /// <summary>
    /// ドロップダウンが操作されたら「保存」し、「Applicatorに通知」する
    /// </summary>
    private void OnQualityChanged(int index)
    {
        PlayerPrefs.SetInt(KEY_QUALITY, index);
        SettingsApplicator.Instance.SetQuality(index); // Applicatorに即時適用を依頼
    }

    // --- 以下の適用処理は Applicator に移動したので削除 ---
    // private void SetMasterVolume(float volume) { ... }
    // private void SetFullscreen(bool isFullscreen) { ... }
    // private void SetQuality(int qualityIndex) { ... }
}
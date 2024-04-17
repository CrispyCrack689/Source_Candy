using _00_GameData.Scripts.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace _00_GameData.Scripts.UI
{
    public class OptionsInitializeViaSave : MonoBehaviour
    {
        [Header("Options Items")]
        [SerializeField, Tooltip("マスター音量スライダー")]
        private Slider masterVolumeSlider;
        [SerializeField, Tooltip("BGM音量スライダー")]
        private Slider bgmVolumeSlider;
        [SerializeField, Tooltip("SE音量スライダー")]
        private Slider sfxVolumeSlider;
        [SerializeField, Tooltip("入力レイアウトドロップダウン")]
        private TMP_Dropdown inputLayoutDropdown;
        [SerializeField, Tooltip("グラフィックス品質ドロップダウン")]
        private TMP_Dropdown graphicsQualityDropdown;
        [SerializeField, Tooltip("言語ドロップダウン")]
        private TMP_Dropdown languageDropdown;

        private void Awake()
        {
            Utils.Load();
        }

        private void Start()
        {
            // セーブデータから初期値を設定
            masterVolumeSlider.value = Utils.saveData.gameVolumeMaster * 100.0f;
            bgmVolumeSlider.value = Utils.saveData.gameVolumeBGM * 100.0f;
            sfxVolumeSlider.value = Utils.saveData.gameVolumeSfx * 100.0f;
            inputLayoutDropdown.value = Utils.saveData.gameInputLayout;
            graphicsQualityDropdown.value = Utils.saveData.gameGraphicsQuality;
            languageDropdown.value = Utils.saveData.gameLanguage;

            //TODO:中国語(簡体字),中国語(繁体字),韓国語は未対応
            switch (Utils.saveData.gameLanguage)
            {
                case 0: // 英語
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                    break;
                case 1: // 日本語
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                    break;
                case 2: // ドイツ語
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[5];
                    break;
                case 3: // フランス語
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[6];
                    break;
                default:
                    DebugLogger.LogError("Invalid language index.", this);
                    break;
            }
        }
    }
}
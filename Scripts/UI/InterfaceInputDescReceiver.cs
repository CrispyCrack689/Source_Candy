using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace _00_GameData.Scripts.UI
{
    public class InterfaceInputDescReceiver : MonoBehaviour
    {
        [SerializeField, Tooltip("アイコンアトラス名(キーボード)")]
        private string spriteNameKeyboard;
        [SerializeField, Tooltip("アイコンアトラス名(Xbox)")]
        private string spriteNameXbox;
        [SerializeField, Tooltip("アイコンアトラス名(PlayStation)")]
        private string spriteNamePlayStation;
        [SerializeField, Tooltip("アイコンアトラス名(Nintendo Switch)")]
        private string spriteNameNintendoSwitch;
        [SerializeField, Tooltip("アイコンアトラス名(Steam Deck)")]
        private string spriteNameSteamDeck;

        private InterfaceIconManager _interfaceIconManager;
        private TextMeshProUGUI _textMeshProUgui;
        private LocalizeStringEvent _localizeStringEvent;

        private LocalizedString _stringReference;

        public InterfaceInputDescReceiver(
            string spriteNameKeyboard,
            string spriteNameXbox,
            string spriteNamePlayStation,
            string spriteNameNintendoSwitch,
            string spriteNameSteamDeck
        )
        {
            this.spriteNameKeyboard = spriteNameKeyboard;
            this.spriteNameXbox = spriteNameXbox;
            this.spriteNamePlayStation = spriteNamePlayStation;
            this.spriteNameNintendoSwitch = spriteNameNintendoSwitch;
            this.spriteNameSteamDeck = spriteNameSteamDeck;
        }

        private void Awake()
        {
            _interfaceIconManager = GetComponentInParent<InterfaceIconManager>();
            _textMeshProUgui = GetComponent<TextMeshProUGUI>();
            _localizeStringEvent = GetComponent<LocalizeStringEvent>();
        }

        private void Update()
        {
            // 現在の言語に合わせて文字列を取得
            _stringReference = _localizeStringEvent.StringReference;

            // Dictionaryからアイコンを取得
            switch (_interfaceIconManager.inputType)
            {
                // SpriteAssetを変更、テキストにアイコンを表示
                case InterfaceIconManager.InputType.Keyboard:
                    _textMeshProUgui.spriteAsset = _interfaceIconManager.IconSpriteDictKeyboard[spriteNameKeyboard];
                    _textMeshProUgui.text = $"<sprite name=\"{spriteNameKeyboard}\" color=#000000>" + _stringReference.GetLocalizedString();
                    break;
                case InterfaceIconManager.InputType.Xbox:
                    _textMeshProUgui.spriteAsset = _interfaceIconManager.IconSpriteDictXboxController[spriteNameXbox];
                    _textMeshProUgui.text = $"<sprite name=\"{spriteNameXbox}\" color=#000000>" + _stringReference.GetLocalizedString();
                    break;
                case InterfaceIconManager.InputType.PlayStation:
                    _textMeshProUgui.spriteAsset = _interfaceIconManager.IconSpriteDictPlaystationController[spriteNamePlayStation];
                    _textMeshProUgui.text = $"<sprite name=\"{spriteNamePlayStation}\" color=#000000>" + _stringReference.GetLocalizedString();
                    break;
                case InterfaceIconManager.InputType.NintendoSwitch:
                    _textMeshProUgui.spriteAsset = _interfaceIconManager.IconSpriteDictSwitchController[spriteNameNintendoSwitch];
                    _textMeshProUgui.text = $"<sprite name=\"{spriteNameNintendoSwitch}\" color=#000000>" + _stringReference.GetLocalizedString();
                    break;
                case InterfaceIconManager.InputType.SteamDeck:
                    _textMeshProUgui.spriteAsset = _interfaceIconManager.IconSpriteDictSteamDeck[spriteNameSteamDeck];
                    _textMeshProUgui.text = $"<sprite name=\"{spriteNameSteamDeck}\" color=#000000>" + _stringReference.GetLocalizedString();
                    break;
            }
        }
    }
}
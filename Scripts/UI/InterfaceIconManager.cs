using System.Collections.Generic;
using _00_GameData.Scripts.Internal;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _00_GameData.Scripts.UI
{
    public class InterfaceIconManager : SerializedMonoBehaviour
    {
        [Tooltip("入力アイコン:キーボード")]
        [DictionaryDrawerSettings(KeyLabel = "Input Name", ValueLabel = "Icon")]
        public readonly Dictionary<string, Sprite> IconDictKeyboard;
        [Tooltip("入力アイコン:Xboxコントローラー")]
        [DictionaryDrawerSettings(KeyLabel = "Input Name", ValueLabel = "Icon")]
        public readonly Dictionary<string, Sprite> IconDictXboxController;
        [Tooltip("入力アイコン:PlayStationコントローラー")]
        [DictionaryDrawerSettings(KeyLabel = "Input Name", ValueLabel = "Icon")]
        public readonly Dictionary<string, Sprite> IconDictPlaystationController;
        [Tooltip("入力アイコン:Nintendo Switchコントローラー")]
        [DictionaryDrawerSettings(KeyLabel = "Input Name", ValueLabel = "Icon")]
        public readonly Dictionary<string, Sprite> IconDictSwitchController;
        [Tooltip("入力アイコン:Steam Deck")]
        [DictionaryDrawerSettings(KeyLabel = "Input Name", ValueLabel = "Icon")]
        public readonly Dictionary<string, Sprite> IconDictSteamDeck;
        [Tooltip("入力アイコンスプライト:キーボード")]
        [DictionaryDrawerSettings(KeyLabel = "Icon Name", ValueLabel = "Sprite Asset")]
        public readonly Dictionary<string, TMP_SpriteAsset> IconSpriteDictKeyboard;
        [Tooltip("入力アイコンスプライト:Xboxコントローラー")]
        [DictionaryDrawerSettings(KeyLabel = "Icon Name", ValueLabel = "Sprite Asset")]
        public readonly Dictionary<string, TMP_SpriteAsset> IconSpriteDictXboxController;
        [Tooltip("入力アイコンスプライト:PlayStationコントローラー")]
        [DictionaryDrawerSettings(KeyLabel = "Icon Name", ValueLabel = "Sprite Asset")]
        public readonly Dictionary<string, TMP_SpriteAsset> IconSpriteDictPlaystationController;
        [Tooltip("入力アイコンスプライト:Nintendo Switchコントローラー")]
        [DictionaryDrawerSettings(KeyLabel = "Icon Name", ValueLabel = "Sprite Asset")]
        public readonly Dictionary<string, TMP_SpriteAsset> IconSpriteDictSwitchController;
        [Tooltip("入力アイコンスプライト:Steam Deck")]
        [DictionaryDrawerSettings(KeyLabel = "Icon Name", ValueLabel = "Sprite Asset")]
        public readonly Dictionary<string, TMP_SpriteAsset> IconSpriteDictSteamDeck;
        [HideInInspector] public InputType inputType = InputType.Keyboard;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InterfaceIconManager(
            Dictionary<string, Sprite> iconDictKeyboard,
            Dictionary<string, Sprite> iconDictXboxController,
            Dictionary<string, Sprite> iconDictPlaystationController,
            Dictionary<string, Sprite> iconDictSwitchController,
            Dictionary<string, Sprite> iconDictSteamDeck,
            Dictionary<string, TMP_SpriteAsset> iconSpriteDictKeyboard,
            Dictionary<string, TMP_SpriteAsset> iconSpriteDictXboxController,
            Dictionary<string, TMP_SpriteAsset> iconSpriteDictPlaystationController,
            Dictionary<string, TMP_SpriteAsset> iconSpriteDictSwitchController,
            Dictionary<string, TMP_SpriteAsset> iconSpriteDictSteamDeck)
        {
            IconDictKeyboard = iconDictKeyboard;
            IconDictXboxController = iconDictXboxController;
            IconDictPlaystationController = iconDictPlaystationController;
            IconDictSwitchController = iconDictSwitchController;
            IconDictSteamDeck = iconDictSteamDeck;
            IconSpriteDictKeyboard = iconSpriteDictKeyboard;
            IconSpriteDictXboxController = iconSpriteDictXboxController;
            IconSpriteDictPlaystationController = iconSpriteDictPlaystationController;
            IconSpriteDictSwitchController = iconSpriteDictSwitchController;
            IconSpriteDictSteamDeck = iconSpriteDictSteamDeck;
        }

        /// <summary>
        /// 入力デバイスタイプ
        /// </summary>
        public enum InputType
        {
            Keyboard,
            Xbox,
            PlayStation,
            NintendoSwitch,
            SteamDeck
        }

        private void Update()
        {
            // キーボード・マウス入力
            if (Utils.DetermineIsKeyboardMouse())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                inputType = InputType.Keyboard;
            }

            // ゲームパッド入力
            if (Utils.DetermineIsController())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Utils.DetermineIsXbox())
                {
                    inputType = InputType.Xbox;
                }
                else if (Utils.DetermineIsPlayStation())
                {
                    inputType = InputType.PlayStation;
                }
                else if (Utils.DetermineIsSwitch())
                {
                    inputType = InputType.NintendoSwitch;
                }
                else if (DetermineIsSteamDeck())
                {
                    inputType = InputType.SteamDeck;
                }
            }
        }

        /// <summary>
        /// Steam Deck上で動作しているかどうかを判断
        /// </summary>
        /// <returns>Steam Deckであれば true</returns>
        private static bool DetermineIsSteamDeck()
        {
            /* 実機がないため未検証 */
            return Steamworks.SteamUtils.IsSteamRunningOnSteamDeck();
        }
    }
}
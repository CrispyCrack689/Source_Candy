using System;
using _00_GameData.Scripts.Internal;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

namespace _00_GameData.Scripts.UI
{
    public class Options : MonoBehaviour
    {
        [Header("UI:Options(Heading)")]
        [Tooltip("オプションUI:ヘッダー選択背景")]
        public RectTransform optionsHeaderSelectBackground;
        [Tooltip("オプションUI:サウンド")]
        public RectTransform optionsSound;
        [Tooltip("オプションUI:コントロール")]
        public RectTransform optionsControls;
        [Tooltip("オプションUI:グラフィックス")]
        public RectTransform optionsGraphics;

        [Header("UI:Options(Main)")]
        [Tooltip("オプションUI:サウンド")]
        public CanvasGroup optionsSoundCanvasGroup;
        [Tooltip("オプションUI:コントロール")]
        public CanvasGroup optionsControlsCanvasGroup;
        [Tooltip("オプションUI:グラフィックス")]
        public CanvasGroup optionsGraphicsCanvasGroup;

        [Header("Parameters")]
        [Tooltip("選択枠線の移動オフセット")]
        public float headerBackgroundMoveOffset = 10f;

        [Header("Debug")]
        [ReadOnly, Tooltip("ヘッダーで選択されている項目")]
        public HeadingGroup selectedHeadingGroup = HeadingGroup.Sound;

        private const string _INPUT_NAME_TABCHANGE = "UI_TabChange";
        private PlayerInput _playerInput;
        private InputAction _tabChangeAction;

        private InterfaceController _interfaceController;

        private GameObject _soundFirstSelected;
        private GameObject _controlsFirstSelected;
        private GameObject _graphicsFirstSelected;

        private void Awake()
        {
            // コンポーネントを取得
            _playerInput = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerInput>();
            _interfaceController = GetComponent<InterfaceController>();

            // アクションを取得
            _tabChangeAction = _playerInput.actions[_INPUT_NAME_TABCHANGE];
        }

        private void Start()
        {
            // 初期化
            optionsHeaderSelectBackground.localPosition = optionsSound.localPosition + new Vector3(headerBackgroundMoveOffset, 0, 0);
            optionsSoundCanvasGroup.alpha = 1;
            optionsControlsCanvasGroup.alpha = 0;
            optionsGraphicsCanvasGroup.alpha = 0;
            optionsSoundCanvasGroup.blocksRaycasts = true;
            optionsControlsCanvasGroup.blocksRaycasts = false;
            optionsGraphicsCanvasGroup.blocksRaycasts = false;

            // 選択フォーカスをキャッシュ
            _soundFirstSelected = Utils.FindChildWithTag(optionsSoundCanvasGroup.gameObject, TagName.UI_First);
            _controlsFirstSelected = Utils.FindChildWithTag(optionsControlsCanvasGroup.gameObject, TagName.UI_First);
            _graphicsFirstSelected = Utils.FindChildWithTag(optionsGraphicsCanvasGroup.gameObject, TagName.UI_First);
        }

        private void Update()
        {
            // オプション画面が表示されていなければ処理しない
            if (!_interfaceController.isShownOptions) return;

            if (!_tabChangeAction.triggered) return;
            // タブ変更ボタンを押したら反応(Left)
            if (_tabChangeAction.ReadValue<Vector2>() == Vector2.left)
            {
                switch (selectedHeadingGroup)
                {
                    case HeadingGroup.Sound:
                        Heading_Graphics();
                        break;
                    case HeadingGroup.Controls:
                        Heading_Sound();
                        break;
                    case HeadingGroup.Graphics:
                        Heading_Controls();
                        break;
                }
            }
            // タブ変更ボタンを押したら反応(Right)
            else if (_tabChangeAction.ReadValue<Vector2>() == Vector2.right)
            {
                switch (selectedHeadingGroup)
                {
                    case HeadingGroup.Sound:
                        Heading_Controls();
                        break;
                    case HeadingGroup.Controls:
                        Heading_Graphics();
                        break;
                    case HeadingGroup.Graphics:
                        Heading_Sound();
                        break;
                }
            }
        }

        /// <summary>
        /// ヘッダー項目
        /// </summary>
        public enum HeadingGroup
        {
            Sound,
            Controls,
            Graphics
        }

        /// <summary>
        /// ヘッダー項目:サウンド
        /// </summary>
        public void Heading_Sound()
        {
            // 選択されている項目を変更
            selectedHeadingGroup = HeadingGroup.Sound;
            OnChanged_HeaderSelect();
        }

        /// <summary>
        /// ヘッダー項目:コントロール
        /// </summary>
        public void Heading_Controls()
        {
            // 選択されている項目を変更
            selectedHeadingGroup = HeadingGroup.Controls;
            OnChanged_HeaderSelect();
        }

        /// <summary>
        /// ヘッダー項目:グラフィックス
        /// </summary>
        public void Heading_Graphics()
        {
            // 選択されている項目を変更
            selectedHeadingGroup = HeadingGroup.Graphics;
            OnChanged_HeaderSelect();
        }

        /// <summary>
        /// ヘッダー選択背景を移動する
        /// </summary>
        private void OnChanged_HeaderSelect()
        {
            // 選択されている項目が変更されたら、選択されている項目に合わせて背景を移動
            switch (selectedHeadingGroup)
            {
                case HeadingGroup.Sound:
                    // 選択背景を移動
                    optionsHeaderSelectBackground.DOLocalMoveX(
                            optionsSound.localPosition.x + headerBackgroundMoveOffset,
                            0.3f
                        )
                        .SetEase(Ease.OutQuint)
                        .SetUpdate(true);

                    // CanvasGroupを切り替え
                    optionsSoundCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);
                    optionsControlsCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);
                    optionsGraphicsCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);

                    // レイキャストを切り替え
                    optionsSoundCanvasGroup.blocksRaycasts = true;
                    optionsControlsCanvasGroup.blocksRaycasts = false;
                    optionsGraphicsCanvasGroup.blocksRaycasts = false;

                    // 選択フォーカスを移動
                    EventSystem.current.SetSelectedGameObject(_soundFirstSelected);
                    break;
                case HeadingGroup.Controls:
                    // 選択背景を移動
                    optionsHeaderSelectBackground.DOLocalMoveX(
                            optionsControls.localPosition.x + headerBackgroundMoveOffset,
                            0.3f
                        )
                        .SetEase(Ease.OutQuint)
                        .SetUpdate(true);

                    // CanvasGroupを切り替え
                    optionsSoundCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);
                    optionsControlsCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);
                    optionsGraphicsCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);

                    // レイキャストを切り替え
                    optionsSoundCanvasGroup.blocksRaycasts = false;
                    optionsControlsCanvasGroup.blocksRaycasts = true;
                    optionsGraphicsCanvasGroup.blocksRaycasts = false;

                    // 選択フォーカスを移動
                    EventSystem.current.SetSelectedGameObject(_controlsFirstSelected);
                    break;
                case HeadingGroup.Graphics:
                    // 選択背景を移動
                    optionsHeaderSelectBackground.DOLocalMoveX(
                            optionsGraphics.localPosition.x + headerBackgroundMoveOffset,
                            0.3f
                        )
                        .SetEase(Ease.OutQuint)
                        .SetUpdate(true);

                    // CanvasGroupを切り替え
                    optionsSoundCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);
                    optionsControlsCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);
                    optionsGraphicsCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);

                    // レイキャストを切り替え
                    optionsSoundCanvasGroup.blocksRaycasts = false;
                    optionsControlsCanvasGroup.blocksRaycasts = false;
                    optionsGraphicsCanvasGroup.blocksRaycasts = true;

                    // 選択フォーカスを移動
                    EventSystem.current.SetSelectedGameObject(_graphicsFirstSelected);
                    break;
            }
        }

        /// <summary>
        /// マスター音量を変更
        /// </summary>
        /// <param name="value">変更値</param>
        public void ChangeGameVolumeMaster(float value)
        {
            Utils.saveData.gameVolumeMaster = value / 100.0f;
            Utils.Save();
        }

        /// <summary>
        /// BGM音量を変更
        /// </summary>
        /// <param name="value">変更値</param>
        public void ChangeGameVolumeBGM(float value)
        {
            Utils.saveData.gameVolumeBGM = value / 100.0f;
            Utils.Save();
        }

        /// <summary>
        /// SFX音量を変更
        /// </summary>
        /// <param name="value">変更値</param>
        public void ChangeGameVolumeSfx(float value)
        {
            Utils.saveData.gameVolumeSfx = value / 100.0f;
            Utils.Save();
        }

        /// <summary>
        /// 入力レイアウトを変更
        /// </summary>
        /// <param name="value">変更値</param>
        public void ChangeInputLayout(int value)
        {
            //TODO:未使用
            Utils.saveData.gameInputLayout = value;
            Utils.Save();
        }

        /// <summary>
        /// グラフィックス品質を変更
        /// </summary>
        /// <param name="value">変更値</param>
        public void ChangeGameGraphicsQuality(int value)
        {
            //TODO:未使用
            Utils.saveData.gameGraphicsQuality = value;
            Utils.Save();
        }

        /// <summary>
        /// ゲーム言語を変更
        /// </summary>
        /// <param name="value">変更値</param>
        public void ChangeGameLanguage(int value)
        {
            //TODO:中国語(簡体字),中国語(繁体字),韓国語は未対応
            LocalizationSettings.SelectedLocale = value switch
            {
                0 => LocalizationSettings.AvailableLocales.Locales[0], // 英語
                1 => LocalizationSettings.AvailableLocales.Locales[1], // 日本語
                2 => LocalizationSettings.AvailableLocales.Locales[5], // ドイツ語
                3 => LocalizationSettings.AvailableLocales.Locales[6], // フランス語
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid language index.")
            };
            Utils.saveData.gameLanguage = value;
            Utils.Save();
        }
    }
}
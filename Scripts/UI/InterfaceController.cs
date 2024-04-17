using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Player;
using _00_GameData.Scripts.Systems;
using AnnulusGames.SceneSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _00_GameData.Scripts.UI
{
    public class InterfaceController : MonoBehaviour
    {
        [Tooltip("フェードパネル")]
        public Image fadePanel;
        [Tooltip("瀕死状態パネル")]
        public RawImage injurePanel;

        [Header("UI:Title")]
        [Tooltip("タイトルUI")]
        public RectTransform titleUI;
        [Tooltip("タイトルUI:ボタン")]
        public RectTransform titleButtons;
        [Tooltip("タイトルUI:テキスト類")]
        public RectTransform titleTexts;
        [Tooltip("タイトルUI:選択枠線")]
        public RectTransform titleSelectBorder;

        [Header("UI:InGame")]
        [Tooltip("インゲームUI:プレイヤー情報")]
        public RectTransform inGamePlayerInfo;
        [Tooltip("インゲームUI:ツール")]
        public RectTransform inGameTool;
        [Tooltip("インゲームUI:操作説明")]
        public RectTransform inGameHowToPlay;
        [Tooltip("インゲームUI:コンボ")]
        public RectTransform inGameCombo;

        [Header("UI:Pause")]
        [Tooltip("ポーズUI:最初に選択されるオブジェクト")]
        public GameObject pauseFirstSelect;
        [Tooltip("ポーズUI:ボタン")]
        public RectTransform pauseButtons;
        [Tooltip("ポーズUI:入力ガイド")]
        public RectTransform pauseInputs;

        [Header("UI:Options")]
        [Tooltip("オプションUI:最初に選択されるオブジェクト(サウンド)")]
        public GameObject optionsSoundFirstSelect;
        [Tooltip("オプションUI:最初に選択されるオブジェクト(コントロール)")]
        public GameObject optionsControlsFirstSelect;
        [Tooltip("オプションUI:最初に選択されるオブジェクト(グラフィックス)")]
        public GameObject optionsGraphicsFirstSelect;
        [Tooltip("オプションUI:画面")]
        public RectTransform optionsScreen;

        [Header("UI:Main")]
        [Tooltip("メインUI:最初に選択されるオブジェクト")]
        public GameObject mainFirstSelect;
        [Tooltip("メインUI:背景")]
        public RectTransform mainBackground;
        [Tooltip("メインUI:入力ガイド")]
        public RectTransform mainInputs;

        [Header("Parameters")]
        [Tooltip("ゲームオーバー時のフェードアウト遷移時間")]
        public float fadeOutTime = 3f;
        [Tooltip("ポーズ画面に遷移する際のUIオフセット")]
        public float pauseOffset = 1000f;
        [Tooltip("メイン画面に遷移する際のUIオフセット")]
        public float mainOffset = 3000f;
        [Tooltip("オプション画面に遷移する際のUIオフセット")]
        public float optionsOffset = 3000f;

        [Header("Debug")]
        [ReadOnly, Tooltip("UI入力中かどうか")]
        public bool isUIInput;
        [ReadOnly, Tooltip("タイトル画面が表示されているかどうか")]
        public bool isShownTitle;
        [ReadOnly, Tooltip("ポーズ画面が表示されているかどうか")]
        public bool isShownPause;
        [ReadOnly, Tooltip("メイン画面が表示されているかどうか")]
        public bool isShownMain;
        [ReadOnly, Tooltip("オプション画面が表示されているかどうか")]
        public bool isShownOptions;

        private const string _INPUT_NAME_PAUSE = "UI_Pause";
        private const string _INPUT_NAME_MAIN = "UI_Main";
        private const string _INPUT_NAME_BACK = "UI_Back";
        private PlayerInput _playerInput;
        private InputAction _pausePressAction;
        private InputAction _mainPressAction;
        private InputAction _backPressAction;

        private PlayerController _playerController;
        private Options _options;

        private Vector3 _titleButtonsPosition;
        private Vector3 _titleTextsPosition;
        private Vector3 _inGamePlayerInfoPosition;
        private Vector3 _inGameToolPosition;
        private Vector3 _inGameHowToPlayPosition;
        private Vector3 _inGameComboPosition;
        private Vector3 _pauseButtonsPosition;
        private Vector3 _pauseInputsPosition;
        private Vector3 _mainBackgroundPosition;
        private Vector3 _mainInputsPosition;
        private Vector3 _optionsScreenPosition;
        private Button[] _pauseButtons;

        private void Awake()
        {
            // 初期地点を保存
            _titleButtonsPosition = titleButtons.GetComponent<LocalTransform>().localPosition;
            _titleTextsPosition = titleTexts.GetComponent<LocalTransform>().localPosition;
            _inGamePlayerInfoPosition = inGamePlayerInfo.GetComponent<LocalTransform>().localPosition;
            _inGameToolPosition = inGameTool.GetComponent<LocalTransform>().localPosition;
            _inGameHowToPlayPosition = inGameHowToPlay.GetComponent<LocalTransform>().localPosition;
            _inGameComboPosition = inGameCombo.GetComponent<LocalTransform>().localPosition;
            _pauseButtonsPosition = pauseButtons.GetComponent<LocalTransform>().localPosition;
            _pauseInputsPosition = pauseInputs.GetComponent<LocalTransform>().localPosition;
            _mainBackgroundPosition = mainBackground.GetComponent<LocalTransform>().localPosition;
            _mainInputsPosition = mainInputs.GetComponent<LocalTransform>().localPosition;
            _optionsScreenPosition = optionsScreen.GetComponent<LocalTransform>().localPosition;

            // コンポーネントを取得
            _playerInput = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerInput>();
            _playerController = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerController>();
            _options = GetComponent<Options>();

            // アクションを取得
            _pausePressAction = _playerInput.actions[_INPUT_NAME_PAUSE];
            _mainPressAction = _playerInput.actions[_INPUT_NAME_MAIN];
            _backPressAction = _playerInput.actions[_INPUT_NAME_BACK];
        }

        private void Start()
        {
            // 初期化
            pauseButtons.localPosition = new Vector3(_pauseButtonsPosition.x + pauseOffset, _pauseButtonsPosition.y, 0f);
            pauseInputs.localPosition = new Vector3(_pauseButtonsPosition.x + pauseOffset, _pauseInputsPosition.y, 0f);
            mainBackground.localPosition = new Vector3(0, _mainBackgroundPosition.y - mainOffset, 0);
            mainInputs.localPosition = new Vector3(0, _mainInputsPosition.y - mainOffset, 0);
            optionsScreen.localPosition = new Vector3(_optionsScreenPosition.x + optionsOffset, _optionsScreenPosition.y, 0);

            injurePanel.enabled = false;
            fadePanel.color = Color.clear;

            // ポーズ画面のボタン無効化
            _pauseButtons = pauseButtons.GetComponentsInChildren<Button>();
            foreach (Button button in _pauseButtons)
            {
                button.interactable = false;
            }

            // タイトル画面処理
            titleUI.gameObject.SetActive(true);
            Initialize();
        }

        private void Update()
        {
            // タイトル画面では以後の処理を行わない
            if (isShownTitle) return;

            // 瀕死時
            injurePanel.enabled = _playerController.currentHp <= 20;
            // ゲームオーバー時
            if (_playerController.currentHp <= 0)
            {
                fadePanel.DOColor(Color.black, fadeOutTime)
                    .SetEase(Ease.OutSine).SetUpdate(true);
                return;
            }

            // キャンセルボタンを押した瞬間のみ反応
            if (_backPressAction.WasPressedThisFrame())
            {
                // ポーズ画面が表示されていたら
                if (isShownPause)
                {
                    PauseOut();
                    return;
                }
                // オプション画面が表示されていたら
                else if (isShownOptions) OptionsOut();
                // メイン画面が表示されていたら
                else if (isShownMain) MainOut();
            }
            // メインボタンを押した瞬間のみ反応
            // if (_mainPressAction.WasPressedThisFrame())
            // {
            //     // ポーズ画面が表示されているなら処理しない
            //     if (isShownPause) return;
            //     // UI画面だったら(TimeScaleが0だったら)
            //     if (Time.timeScale == 0) MainOut();
            //     else MainIn();
            // }
            // ポーズボタンを押した瞬間のみ反応
            if (_pausePressAction.WasPressedThisFrame())
            {
                // メイン画面が表示されているなら処理しない
                if (isShownMain) return;
                // オプション画面が表示されているなら処理しない
                if (isShownOptions) return;
                // ポーズ画面を表示する
                if (!isShownPause) PauseIn();
            }
        }

        private void LateUpdate()
        {
            // ダメージを受けたらUIを揺らす
            if (_playerController.takenDamage) ShakeUITakenDamage();
        }

        /// <summary>
        /// 被ダメージ時にUIを揺らす
        /// </summary>
        private void ShakeUITakenDamage()
        {
            // UIを揺らす
            inGamePlayerInfo.parent.DOShakePosition(0.5f, 70f);
        }

        /// <summary>
        /// UI画面に入る
        /// </summary>
        /// <param name="nextSelected">次に選択状態にするオブジェクト</param>>
        private void EnterUI(GameObject nextSelected = null)
        {
            // ゲーム時間を止める
            Time.timeScale = 0;
            // UI入力フラグを立てる
            isUIInput = true;
            // 選択フォーカスを移動
            EventSystem.current.SetSelectedGameObject(nextSelected);
        }

        /// <summary>
        /// UI画面から出る
        /// </summary>
        /// <param name="nextSelected">次に選択状態にするオブジェクト</param>
        /// <param name="isExitUI">ゲーム時間、UI入力フラグを戻すか</param>
        /// >
        private void ExitUI(GameObject nextSelected = null, bool isExitUI = true)
        {
            if (isExitUI)
            {
                // ゲーム時間を戻す
                Time.timeScale = 1;
                // UI入力フラグを下げる
                isUIInput = false;
            }
            // 選択フォーカスを移動
            EventSystem.current.SetSelectedGameObject(nextSelected);
        }

        /// <summary>
        /// 起動時処理
        /// </summary>
        private void Initialize()
        {
            EnterUI(titleButtons.GetChild(0).gameObject);
            Time.timeScale = 0;
            isShownTitle = true;

            inGameTool.DOLocalMove(
                new Vector3(_inGameToolPosition.x - pauseOffset, _inGameToolPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGamePlayerInfo.DOLocalMove(
                new Vector3(_inGamePlayerInfoPosition.x + pauseOffset, _inGamePlayerInfoPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameHowToPlay.DOLocalMove(
                new Vector3(_inGameHowToPlayPosition.x + pauseOffset, _inGameHowToPlayPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameCombo.DOLocalMove(
                new Vector3(_inGameComboPosition.x + pauseOffset, _inGameComboPosition.y, 0),
                0.5f
            ).SetUpdate(true);
        }

        /// <summary>
        /// タイトルから出る
        /// </summary>
        public void TitleOut()
        {
            ExitUI();
            Time.timeScale = 1;
            isShownTitle = false;

            // 選択枠線を非表示化
            titleSelectBorder.gameObject.SetActive(false);
            // ゲームスタートフラグ
            GameManager.IsGameStarted = true;

            titleButtons.DOLocalMove(
                new Vector3(_titleButtonsPosition.x + pauseOffset, _titleButtonsPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            titleTexts.DOLocalMove(
                new Vector3(_titleTextsPosition.x - pauseOffset, _titleTextsPosition.y, 0),
                0.5f
            ).SetUpdate(true).OnComplete(TitleOut_Complete);
            inGameTool.DOLocalMove(
                new Vector3(_inGameToolPosition.x, _inGameToolPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGamePlayerInfo.DOLocalMove(
                new Vector3(_inGamePlayerInfoPosition.x, _inGamePlayerInfoPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameHowToPlay.DOLocalMove(
                new Vector3(_inGameHowToPlayPosition.x, _inGameHowToPlayPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameCombo.DOLocalMove(
                new Vector3(_inGameComboPosition.x, _inGameComboPosition.y, 0),
                0.5f
            ).SetUpdate(true);
        }

        /// <summary>
        /// タイトルから出る_トゥイーン完了時
        /// </summary>
        private void TitleOut_Complete()
        {
            // タイトルUIを非表示化
            titleUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// ポーズ画面を開く
        /// </summary>
        private void PauseIn()
        {
            EnterUI(pauseFirstSelect);

            inGameTool.DOLocalMove(
                new Vector3(_inGameToolPosition.x - pauseOffset, _inGameToolPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGamePlayerInfo.DOLocalMove(
                new Vector3(_inGamePlayerInfoPosition.x + pauseOffset, _inGamePlayerInfoPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameHowToPlay.DOLocalMove(
                new Vector3(_inGameHowToPlayPosition.x + pauseOffset, _inGameHowToPlayPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameCombo.DOLocalMove(
                new Vector3(_inGameComboPosition.x + pauseOffset, _inGameComboPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            pauseButtons.DOLocalMove(
                new Vector3(_pauseButtonsPosition.x, _pauseButtonsPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            pauseInputs.DOLocalMove(
                new Vector3(_pauseInputsPosition.x, _pauseInputsPosition.y, 0),
                0.5f
            ).SetUpdate(true).OnComplete(PauseIn_Complete);
        }

        /// <summary>
        /// ポーズ画面を開く_トゥイーン完了時
        /// </summary>
        private void PauseIn_Complete()
        {
            // ボタン有効化
            foreach (var button in _pauseButtons)
            {
                button.interactable = true;
            }

            // ポーズ画面フラグ
            isShownPause = true;
        }

        /// <summary>
        /// ポーズ画面を閉じる
        /// </summary>
        public void PauseOut()
        {
            ExitUI();
            isShownPause = false;

            // ボタン無効化
            foreach (var button in _pauseButtons)
            {
                button.interactable = false;
            }

            inGameTool.DOLocalMove(
                new Vector3(_inGameToolPosition.x, _inGameToolPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGamePlayerInfo.DOLocalMove(
                new Vector3(_inGamePlayerInfoPosition.x, _inGamePlayerInfoPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameHowToPlay.DOLocalMove(
                new Vector3(_inGameHowToPlayPosition.x, _inGameHowToPlayPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            inGameCombo.DOLocalMove(
                new Vector3(_inGameComboPosition.x, _inGameComboPosition.y, 0),
                0.5f
            ).SetUpdate(true);
            pauseButtons.DOLocalMove(
                new Vector3(_pauseButtonsPosition.x + pauseOffset, _pauseButtonsPosition.y, 0f),
                0.5f
            ).SetUpdate(true);
            pauseInputs.DOLocalMove(
                new Vector3(_pauseInputsPosition.x + pauseOffset, _pauseInputsPosition.y, 0f),
                0.5f
            ).SetUpdate(true);
        }

        /// <summary>
        /// メイン画面を開く
        /// </summary>
        private void MainIn()
        {
            EnterUI(mainFirstSelect);
            isShownMain = true;

            mainBackground.DOLocalMove(
                Vector3.zero,
                0.5f
            ).SetUpdate(true);
            mainInputs.DOLocalMove(
                Vector3.zero,
                0.5f
            ).SetUpdate(true);
        }

        /// <summary>
        /// メイン画面を閉じる
        /// </summary>
        private void MainOut()
        {
            ExitUI();
            isShownMain = false;

            mainBackground.DOLocalMove(
                new Vector3(0, _mainBackgroundPosition.y - mainOffset, 0),
                0.5f
            ).SetUpdate(true);
            mainInputs.DOLocalMove(
                new Vector3(0, _mainInputsPosition.y - mainOffset, 0),
                0.5f
            ).SetUpdate(true);
        }

        /// <summary>
        /// オプション画面を開く
        /// </summary>
        public void OptionsIn()
        {
            switch (_options.selectedHeadingGroup)
            {
                case Options.HeadingGroup.Sound:
                    EnterUI(optionsSoundFirstSelect);
                    break;
                case Options.HeadingGroup.Controls:
                    EnterUI(optionsControlsFirstSelect);
                    break;
                case Options.HeadingGroup.Graphics:
                    EnterUI(optionsGraphicsFirstSelect);
                    break;
            }
            isShownPause = false;
            isShownOptions = true;

            optionsScreen.DOLocalMove(
                new Vector3(_optionsScreenPosition.x, _optionsScreenPosition.y, 0),
                0.5f
            ).SetUpdate(true);
        }

        /// <summary>
        /// オプション画面を閉じる
        /// </summary>
        private void OptionsOut()
        {
            ExitUI(pauseFirstSelect, false);
            isShownPause = true;
            isShownOptions = false;

            optionsScreen.DOLocalMove(
                new Vector3(_optionsScreenPosition.x + optionsOffset, _optionsScreenPosition.y, 0),
                0.5f
            ).SetUpdate(true);
        }

        /// <summary>
        /// シーンを再読み込み
        /// </summary>
        public void RestartGame()
        {
            Scenes.LoadScene(SceneName.LoadingScene, LoadSceneMode.Single);
        }

        /// <summary>
        /// ゲームを終了
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
}
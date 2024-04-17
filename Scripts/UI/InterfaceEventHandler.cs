using System;
using System.Threading;
using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Systems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _00_GameData.Scripts.UI
{
    public class InterfaceEventHandler : MonoBehaviour
    {
        [Header("UI:Title")]
        [Tooltip("タイトルUI:選択枠線")]
        public RectTransform titleSelectBorder;

        [Header("UI:Pause")]
        [Tooltip("ポーズUI:選択枠線")]
        public RectTransform pauseSelectBorder;

        [Header("UI:Options")]
        [Tooltip("オプションUI(サウンド):選択枠線")]
        public RectTransform soundSelectBorder;
        [Tooltip("オプションUI(コントロール):選択枠線")]
        public RectTransform controlsSelectBorder;
        [Tooltip("オプションUI(グラフィックス):選択枠線")]
        public RectTransform graphicsSelectBorder;

        [Header("UI:Main")]
        [Tooltip("メインUI:選択枠線")]
        public RectTransform mainSelectBorder;

        [Header("Parameters")]
        [Tooltip("選択枠線の移動速度")]
        public float borderMoveSpeed = 0.1f;
        [Tooltip("選択枠線の拡大率")]
        public float borderExpandScale = 1.02f;

        private InputHandler _inputHandler;
        private InterfaceController _interfaceController;
        private Options _options;

        private void Awake()
        {
            _inputHandler = EventSystem.current.gameObject.GetComponent<InputHandler>();
            _interfaceController = GetComponent<InterfaceController>();
            _options = GetComponent<Options>();
        }

        private void Start()
        {
            // タスク呼び出し
            var cancellationToken = this.GetCancellationTokenOnDestroy();
            // SelectBorderMove_Retarget(cancellationToken);
            SelectBorderMove_Keyboard(cancellationToken);
            SelectBorderMove_Controller(cancellationToken);

            // 枠線を動かす
            pauseSelectBorder.DOScale(pauseSelectBorder.localScale * borderExpandScale, 0.3f)
                .SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }

        private void Update()
        {
            // 選択枠線を表示
            pauseSelectBorder.gameObject.SetActive(_interfaceController.isShownPause);
            soundSelectBorder.gameObject.SetActive(_interfaceController.isShownOptions);
            controlsSelectBorder.gameObject.SetActive(_interfaceController.isShownOptions);
            graphicsSelectBorder.gameObject.SetActive(_interfaceController.isShownOptions);
            // mainSelectBorder.gameObject.SetActive(_interfaceController.isShownMain);
        }

        /// <summary>
        /// 選択枠線を戻す
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void SelectBorderMove_Retarget(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // いずれかのキー or ボタンが押されるまで待機
                    await UniTask.WaitUntil(() => Keyboard.current.anyKey.isPressed || InputHandler.DetermineIsController(), cancellationToken: cancellationToken);

                    // 入力タイプがマウス以外になるまで待機
                    await UniTask.WaitUntil(() => _inputHandler.currentInputType != InputHandler.InputType.Mouse, cancellationToken: cancellationToken);

                    // EventSystemの選択オブジェクトがなくなるまで待機
                    await UniTask.WaitUntil(() => EventSystem.current.currentSelectedGameObject == null, cancellationToken: cancellationToken);
                }
            } catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("SelectBorderMove_Retarget() was canceled.", this);
            }
        }

        /// <summary>
        /// 選択枠線を移動させる(キーボード選択時)
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void SelectBorderMove_Keyboard(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // 入力タイプがキーボードになるまで待機
                    await UniTask.WaitUntil(() => _inputHandler.currentInputType == InputHandler.InputType.Keyboard, cancellationToken: cancellationToken);

                    // EventSystemの選択オブジェクトが変更されるまで待機
                    await UniTask.WaitUntil(() => EventSystem.current.currentSelectedGameObject != null, cancellationToken: cancellationToken);

                    // 選択オブジェクトが変更されたら選択枠線を移動
                    SelectBorderMove(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
                }
            } catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("SelectBorderMove_Keyboard() was canceled.", this);
            }
        }

        /// <summary>
        /// 選択枠線を移動させる(コントローラー選択時)
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void SelectBorderMove_Controller(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // 入力タイプがコントローラーになるまで待機
                    await UniTask.WaitUntil(() => _inputHandler.currentInputType == InputHandler.InputType.Controller, cancellationToken: cancellationToken);

                    // EventSystemの選択オブジェクトが変更されるまで待機
                    await UniTask.WaitUntil(() => EventSystem.current.currentSelectedGameObject != null, cancellationToken: cancellationToken);

                    // 選択オブジェクトが変更されたら選択枠線を移動
                    SelectBorderMove(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
                }
            } catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("SelectBorderMove_Controller() was canceled.", this);
            }
        }

        /// <summary>
        /// 選択枠線を移動させる
        /// </summary>
        /// <param name="rectTransform">対象のRectTransform</param>
        public void SelectBorderMove(RectTransform rectTransform)
        {
            // タイトル画面
            if (_interfaceController.isShownTitle)
            {
                titleSelectBorder.DOMove(rectTransform.position, borderMoveSpeed)
                    .SetEase(Ease.OutCubic).SetUpdate(true);
            }
            // ポーズ画面
            if (_interfaceController.isShownPause)
            {
                pauseSelectBorder.DOMove(rectTransform.position, borderMoveSpeed)
                    .SetEase(Ease.OutCubic).SetUpdate(true);
            }
            // オプション画面
            else if (_interfaceController.isShownOptions)
            {
                switch (_options.selectedHeadingGroup)
                {
                    // サウンド設定
                    case Options.HeadingGroup.Sound:
                        var pointSnd = rectTransform.Find("Handle Slide Area/Handle");
                        if (pointSnd == null) return;
                        soundSelectBorder.DOMove(pointSnd.position, borderMoveSpeed)
                            .SetEase(Ease.INTERNAL_Zero).SetUpdate(true);
                        break;
                    // コントロール設定
                    case Options.HeadingGroup.Controls:
                        var pointCont = rectTransform.Find("BorderFocusPoint");
                        if (pointCont == null) return;
                        controlsSelectBorder.DOMove(pointCont.position, borderMoveSpeed)
                            .SetEase(Ease.INTERNAL_Zero).SetUpdate(true);
                        break;
                    // グラフィックス設定
                    case Options.HeadingGroup.Graphics:
                        var pointGraph = rectTransform.Find("BorderFocusPoint");
                        if (pointGraph == null) return;
                        graphicsSelectBorder.DOMove(pointGraph.position, borderMoveSpeed)
                            .SetEase(Ease.INTERNAL_Zero).SetUpdate(true);
                        break;
                }
            }
            // else if (_interfaceController.isShownMain)
            // {
            //     mainSelectBorder.DOMove(rectTransform.position, borderMoveSpeed)
            //         .SetEase(Ease.OutCubic).SetUpdate(true);
            // }
        }
    }
}
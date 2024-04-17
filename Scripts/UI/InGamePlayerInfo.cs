using System;
using System.Threading;
using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _00_GameData.Scripts.UI
{
    public class InGamePlayerInfo : MonoBehaviour
    {
        [Header("Player Info")]
        [Tooltip("HPゲージ")]
        public Slider hpGauge;
        [SerializeField, Tooltip("HPゲージの遅延表示")]
        private Image hpFillDelay;
        [Tooltip("STゲージ")]
        public Slider stGauge;
        [Tooltip("TOXICゲージ")]
        public Image toxicGauge;

        [Header("Tool")]
        [Tooltip("現在の武器の属性")]
        public Image currentElementImage;
        [Tooltip("ツール上アイコン")]
        public Image toolUpIcon;
        [Tooltip("ツール下アイコン")]
        public Image toolDownIcon;
        [Tooltip("ツール右アイコン")]
        public Image toolRightIcon;
        
        [Header("Combo")]
        [Tooltip("コンボ数")]
        public TextMeshProUGUI comboCount;

        /* Components */
        private PlayerController _playerController;
        private RectTransform _hpFillDelayRectTransform;

        /* Task */
        private const float _HP_DELAY_WAIT_TIME = 0.75f;

        private void Awake()
        {
            // コンポーネントを取得
            _playerController = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerController>();
            _hpFillDelayRectTransform = hpFillDelay.GetComponent<RectTransform>();

            // 初期化
            _hpFillDelayRectTransform.sizeDelta = new Vector2(160, 10);
            toxicGauge.fillAmount = 0;
            currentElementImage.rectTransform.position = toolUpIcon.rectTransform.position;
        }

        private void Start()
        {
            // タスク呼び出し
            var cancellationToken = destroyCancellationToken;
            OnValueChanged_HP(cancellationToken);
            OnValueChanged_TOXIC(cancellationToken);
            OnChanged_CurrentElement(cancellationToken);
        }

        private void Update()
        {
            // HP,ST値をスライダーに反映
            var hpGaugeValue = _playerController.currentHp / _playerController.maxHp;
            var stGaugeValue = _playerController.currentSt / _playerController.maxSt;
            hpGauge.value = hpGaugeValue;
            stGauge.value = stGaugeValue;
        }

        /// <summary>
        /// HPゲージの遅延表示を更新するタスク
        /// </summary>
        /// <param name="token">キャンセルトークン</param>
        private async void OnValueChanged_HP(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    // 現在のHPが変わるまで待つ
                    var currentHp = _playerController.currentHp;
                    const double tolerance = 0.01f;
                    await UniTask.WaitUntil(() => Math.Abs(currentHp - _playerController.currentHp) > tolerance, cancellationToken:token);

                    // HPゲージの遅延表示を更新
                    // 最大を160とする
                    var hpGaugeValue = (_playerController.currentHp / _playerController.maxHp) * 160f;
                    _hpFillDelayRectTransform.DOSizeDelta(new Vector2(hpGaugeValue, 10), 0.5f)
                        .SetEase(Ease.OutQuint).SetDelay(_HP_DELAY_WAIT_TIME);
                }
            }
            catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("OnValueChanged_HP() was cancelled.", this);
            }
        }

        /// <summary>
        /// TOXICゲージの値を更新するタスク
        /// </summary>
        /// <param name="token">キャンセルトークン</param>
        private async void OnValueChanged_TOXIC(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    // 現在のTOXIC値が変わるまで待つ
                    var currentToxic = _playerController.currentToxic;
                    const double tolerance = 0.01f;
                    await UniTask.WaitUntil(() => Math.Abs(currentToxic - _playerController.currentToxic) > tolerance, cancellationToken:token);

                    // TOXICゲージの値を更新
                    var toxicGaugeValue = _playerController.currentToxic / _playerController.maxToxic;
                    toxicGauge.DOFillAmount(toxicGaugeValue, 0.75f).SetEase(Ease.OutQuad);
                }
            }
            catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("OnValueChanged_TOXIC() was cancelled.", this);
            }
        }
        
        /* [ボツ]
        /// <summary>
        /// コンボ数を更新するタスク
        /// </summary>
        /// <param name="token">キャンセルトークン</param>
        private async void OnValueChanged_ComboCount(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    // 現在のコンボ数が変わるまで待つ
                    // int currentComboCount = _playerController.currentComboCount;
                    // await UniTask.WaitUntil(() => currentComboCount != _playerController.currentComboCount, cancellationToken:token);

                    // コンボ数を更新
                    // comboCount.text = $"<size=50><b>{_playerController.currentComboCount}</b></size> Combo";
                }
            }
            catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("OnValueChanged_ComboCount() was cancelled.", this);
            }
        }
        */

        /// <summary>
        /// 現在の武器の属性が変わった時に呼び出されるタスク
        /// </summary>
        /// <param name="token">キャンセルトークン</param>
        private async void OnChanged_CurrentElement(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    // 現在の属性が変わるまで待つ
                    var currentElement = _playerController.currentElement;
                    await UniTask.WaitUntil(() => currentElement != _playerController.currentElement, cancellationToken:token);

                    // 属性アイコンを変更
                    // Tweenで移動
                    switch (_playerController.currentElement)
                    {
                        case PlayerController.PlayerElement.Fire:
                            currentElementImage.rectTransform.DOAnchorPos(toolUpIcon.rectTransform.anchoredPosition, 0.5f).SetEase(Ease.OutQuint);
                            break;
                        case PlayerController.PlayerElement.Water:
                            currentElementImage.rectTransform.DOAnchorPos(toolRightIcon.rectTransform.anchoredPosition, 0.5f).SetEase(Ease.OutQuint);
                            break;
                        case PlayerController.PlayerElement.Spark:
                            currentElementImage.rectTransform.DOAnchorPos(toolDownIcon.rectTransform.anchoredPosition, 0.5f).SetEase(Ease.OutQuint);
                            break;
                        default:
                            DebugLogger.LogError("Invalid PlayerElement.", this);
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("OnChanged_CurrentElement() was cancelled.", this);
            }
        }
    }
}
using System;
using System.Threading;
using _00_GameData.Scripts.Internal;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _00_GameData.Scripts.UI
{
    public class InterfaceIconTweener : MonoBehaviour
    {
        [Header("How To Play")]
        [SerializeField, Tooltip("軽攻撃:アイコン画像")]
        private Image lightAttackIcon;
        [SerializeField, Tooltip("重攻撃:アイコン画像")]
        private Image heavyAttackIcon;
        [SerializeField, Tooltip("ジャンプ:アイコン画像")]
        private Image jumpIcon;
        [SerializeField, Tooltip("回避:アイコン画像")]
        private Image dodgeIcon;
        [SerializeField, Tooltip("ターゲット切り替え:アイコン画像")]
        private Image targetLockIcon;

        [Header("Options")]
        [SerializeField, Tooltip("タブ左切り替え:アイコン画像")]
        private Image tabLeftIcon;
        [SerializeField, Tooltip("タブ右切り替え:アイコン画像")]
        private Image tabRightIcon;

        private PlayerInput _playerInput;
        private InputAction _lightAttackAction;
        private InputAction _heavyAttackAction;
        private InputAction _jumpAction;
        private InputAction _dodgeAction;
        private InputAction _targetLockAction;
        private InputAction _tabAction;

        private void Awake()
        {
            _playerInput = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerInput>();
            _lightAttackAction = _playerInput.actions["Attack_Light"];
            _heavyAttackAction = _playerInput.actions["Attack_Heavy"];
            _jumpAction = _playerInput.actions["Jump"];
            _dodgeAction = _playerInput.actions["Dodge"];
            _targetLockAction = _playerInput.actions["ChangeTargeting"];
            _tabAction = _playerInput.actions["UI_TabChange"];
        }

        private void Start()
        {
            var cancellationToken = this.GetCancellationTokenOnDestroy();
            OnPress_AttackLight(cancellationToken);
            OnPress_AttackHeavy(cancellationToken);
            OnPress_Jump(cancellationToken);
            OnPress_Dodge(cancellationToken);
            OnPress_TargetLock(cancellationToken);
            OnPress_TabChange_Left(cancellationToken);
        }

        /// <summary>
        /// 軽攻撃ボタンが押された時の処理
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void OnPress_AttackLight(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // ボタンが押されるまで待機
                    await UniTask.WaitUntil(() => _lightAttackAction.triggered, cancellationToken: cancellationToken);

                    // アイコンアニメーション
                    var seq = DOTween.Sequence();
                    seq.Append(lightAttackIcon.transform.DOScale(0.8f, 0.1f));
                    seq.Append(lightAttackIcon.transform.DOScale(1.0f, 0.1f));
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("OnPress_AttackLight() was canceled.", this);
            }
        }

        /// <summary>
        /// 重攻撃ボタンが押された時の処理
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void OnPress_AttackHeavy(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // ボタンが押されるまで待機
                    await UniTask.WaitUntil(() => _heavyAttackAction.triggered, cancellationToken: cancellationToken);

                    // アイコンアニメーション
                    var seq = DOTween.Sequence();
                    seq.Append(heavyAttackIcon.transform.DOScale(0.8f, 0.1f));
                    seq.Append(heavyAttackIcon.transform.DOScale(1.0f, 0.1f));
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("OnPress_AttackHeavy() was canceled.", this);
            }
        }

        /// <summary>
        /// ジャンプボタンが押された時の処理
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void OnPress_Jump(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // ボタンが押されるまで待機
                    await UniTask.WaitUntil(() => _jumpAction.triggered, cancellationToken: cancellationToken);

                    // アイコンアニメーション
                    var seq = DOTween.Sequence();
                    seq.Append(jumpIcon.transform.DOScale(0.8f, 0.1f));
                    seq.Append(jumpIcon.transform.DOScale(1.0f, 0.1f));
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("OnPress_Jump() was canceled.", this);
            }
        }

        /// <summary>
        /// 回避ボタンが押された時の処理
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void OnPress_Dodge(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // ボタンが押されるまで待機
                    await UniTask.WaitUntil(() => _dodgeAction.triggered, cancellationToken: cancellationToken);

                    // アイコンアニメーション
                    var seq = DOTween.Sequence();
                    seq.Append(dodgeIcon.transform.DOScale(0.8f, 0.1f));
                    seq.Append(dodgeIcon.transform.DOScale(1.0f, 0.1f));
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("OnPress_Dodge() was canceled.", this);
            }
        }

        /// <summary>
        /// ターゲット切り替えボタンが押された時の処理
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void OnPress_TargetLock(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // ボタンが押されるまで待機
                    await UniTask.WaitUntil(() => _targetLockAction.triggered, cancellationToken: cancellationToken);

                    // アイコンアニメーション
                    var seq = DOTween.Sequence();
                    seq.Append(targetLockIcon.transform.DOScale(0.8f, 0.1f));
                    seq.Append(targetLockIcon.transform.DOScale(1.0f, 0.1f));
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("OnPress_TargetLock() was canceled.", this);
            }
        }

        /// <summary>
        /// タブ切り替えボタンが押された時の処理
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        private async void OnPress_TabChange_Left(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // 左ボタンが押されるまで待機
                    await UniTask.WaitUntil(() => _tabAction.triggered, cancellationToken: cancellationToken);

                    // アイコンアニメーション
                    var seq = DOTween.Sequence().SetUpdate(true);
                    if (_tabAction.ReadValue<Vector2>() == Vector2.left)
                    {
                        seq.Append(tabLeftIcon.transform.DOScale(1.8f, 0.1f));
                        seq.Append(tabLeftIcon.transform.DOScale(2.0f, 0.1f));
                    }
                    else if (_tabAction.ReadValue<Vector2>() == Vector2.right)
                    {
                        seq.Append(tabRightIcon.transform.DOScale(1.8f, 0.1f));
                        seq.Append(tabRightIcon.transform.DOScale(2.0f, 0.1f));
                    }
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("OnPress_TabChange() was canceled.", this);
            }
        }
    }
}
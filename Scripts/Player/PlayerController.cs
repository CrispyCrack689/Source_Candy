using System;
using System.Threading;
using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Systems;
using AnnulusGames.LucidTools.Audio;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _00_GameData.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [Tooltip("プレイヤーのカメラ")]
        public CinemachineFreeLook playerCamera;
        [Tooltip("プレイヤーの死亡カメラ")]
        public PlayerDeathCamera playerDeathCamera;
        [Tooltip("プレイヤーのアニメーター")]
        public Animator animator;

        [Header("Parameters")]
        [Tooltip("最大HP(ヒットポイント)")]
        public int maxHp = 100;
        [Tooltip("HP回復ディレイ")]
        public float hpHealDelay = 0.9f;
        [Tooltip("最大ST(スタミナ)")]
        public int maxSt = 100;
        [Tooltip("ST回復マルチプライヤー")]
        public float stHealMultiplier = 30.0f;
        [Tooltip("最大TOXIC")]
        public int maxToxic = 100;

        [Header("Sounds")]
        [Tooltip("ダメージ音")]
        public AudioClip[] damageSounds;
        [Tooltip("回復音")]
        public AudioClip[] healSounds;

        /* Static */
        public static bool IsAttacking;
        public static bool IsFlying;

        /* Cinemachine */
        private CinemachineVirtualCamera _playerDeathVirtualCamera;
        private const float _CAMERA_TOPRIG_HEIGHT_IDLE = 5.0f;
        private const float _CAMERA_TOPRIG_HEIGHT_ATTACK = 6.0f;
        private const float _CAMERA_TOPRIG_RADIUS_IDLE = 3.0f;
        private const float _CAMERA_TOPRIG_RADIUS_ATTACK = 3.0f;
        private const float _CAMERA_MIDDLERIG_HEIGHT_IDLE = 1.0f;
        private const float _CAMERA_MIDDLERIG_HEIGHT_ATTACK = 2.0f;
        private const float _CAMERA_MIDDLERIG_RADIUS_IDLE = 5.0f;
        private const float _CAMERA_MIDDLERIG_RADIUS_ATTACK = 7.5f;

        /* Animator */
        private static readonly int AnimationIsAttackPhase = Animator.StringToHash("IsAttackPhase");
        private static readonly int AnimationIsDeath = Animator.StringToHash("isDeath");
        private static readonly int AnimationDamage = Animator.StringToHash("Damage");

        [Header("Debug")]
        [ReadOnly, Tooltip("現在のHP")]
        public float currentHp;
        [ReadOnly, Tooltip("現在のST")]
        public float currentSt;
        [ReadOnly, Tooltip("現在のTOXIC")]
        public float currentToxic;
        [ReadOnly, Tooltip("現在の属性")]
        public PlayerElement currentElement;
        [ReadOnly, Tooltip("ST回復フラグ")]
        public bool canHealSt;
        [ReadOnly, Tooltip("ダメージフラグ")]
        public bool takenDamage;
        [ReadOnly, Tooltip("ダメージディレイ")]
        public float canDamageDelay;
        [ReadOnly, Tooltip("回復残り数")]
        public int healRemaining;

        private void Awake()
        {
            // 初期化
            IsAttacking = false;
            IsFlying = false;

            currentHp = maxHp;
            currentSt = maxSt;
            currentToxic = 0;
            currentElement = PlayerElement.Fire;
            canHealSt = true;
            healRemaining = 3;

            playerCamera.m_Orbits[0].m_Height = _CAMERA_TOPRIG_HEIGHT_IDLE;
            playerCamera.m_Orbits[0].m_Radius = _CAMERA_TOPRIG_RADIUS_IDLE;
            playerCamera.m_Orbits[1].m_Height = _CAMERA_MIDDLERIG_HEIGHT_IDLE;
            playerCamera.m_Orbits[1].m_Radius = _CAMERA_MIDDLERIG_RADIUS_IDLE;

            _playerDeathVirtualCamera = playerDeathCamera.GetComponent<CinemachineVirtualCamera>();
            _playerDeathVirtualCamera.enabled = false;
        }

        private void Start()
        {
            // タスク呼び出し
            CancellationToken cancellationToken = destroyCancellationToken;
            DetermineHealPlayerSt(cancellationToken);
            ChangeCameraDistance(cancellationToken);
        }

        private void Update()
        {
            // HP,ST値を0~最大値の間に収める
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);
            currentSt = Mathf.Clamp(currentSt, 0, maxSt);
            currentToxic = Mathf.Clamp(currentToxic, 0, maxToxic);

            // ダメージディレイ更新
            if (canDamageDelay > 0)
            {
                canDamageDelay -= Time.deltaTime;
                takenDamage = false;
            }

            // ゲームオーバー制御
            if (currentHp <= 0)
            {
                // スローモーションにする
                GameManager.ChangeGameTimeScale(0.5f);
                // 死亡カメラを有効化、徐々に上げていく
                _playerDeathVirtualCamera.enabled = true;
                playerDeathCamera.transform.position += Vector3.up * (playerDeathCamera.cameraRiseSpeed * Time.deltaTime);
            }

            // STを回復
            if (canHealSt)
            {
                currentSt += stHealMultiplier * Time.deltaTime;
            }

            // ゲームフェーズに応じてアニメーションを変更
            switch (GameManager.CurrentGamePhase)
            {
                case GameManager.GamePhase.Normal:
                    animator.SetBool(AnimationIsAttackPhase, false);
                    break;

                case GameManager.GamePhase.Attack:
                    animator.SetBool(AnimationIsAttackPhase, true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// プレイヤーの属性
        /// </summary>
        public enum PlayerElement
        {
            Fire,
            Water,
            Spark
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage">ダメージ量</param>
        public void Damage(float damage)
        {
            //TODO:予め敵にPlayerControllerをGetComponentしておき、攻撃判定がプレイヤーと被ったらDamage()を呼び出すようにする
            // プレイヤーのHPが0以上だったら
            if (!(currentHp > 0)) return;
            // ダメージディレイが0以下だったら
            if (!(canDamageDelay <= 0)) return;

            // ダメージを受ける
            currentHp -= damage;
            takenDamage = true;
            canDamageDelay = hpHealDelay;

            // SEを再生
            var random = Random.Range(0, damageSounds.Length);
            LucidAudio.PlaySE(damageSounds[random])
                .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster);

            // HPが0以下だったら
            if (currentHp <= 0)
                // 死亡アニメーションを再生
                animator.SetBool(AnimationIsDeath, true);
            else
                // ダメージアニメーションを再生
                animator.SetTrigger(AnimationDamage);
        }

        /// <summary>
        /// プレイヤーのSTを回復して良いかどうかを判定するタスク
        /// </summary>
        /// <param name="token">キャンセルトークン</param>
        private async void DetermineHealPlayerSt(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    // canHealStがfalseになるまで待つ
                    await UniTask.WaitUntil(() => canHealSt == false, cancellationToken: token);
                    // 1秒待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);

                    canHealSt = true;
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("HealPlayerST() was canceled.", this);
            }
        }

        /// <summary>
        /// カメラの距離を変更するタスク
        /// </summary>
        /// <param name="token">キャンセルトークン</param>
        private async void ChangeCameraDistance(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    switch (GameManager.CurrentGamePhase)
                    {
                        case GameManager.GamePhase.Normal:
                            // ゲームフェーズが変わるまで待つ
                            await UniTask.WaitUntil(() => GameManager.CurrentGamePhase != GameManager.GamePhase.Normal, cancellationToken: token);

                            // ゲームフェーズが変わったらカメラの距離を変更
                            DOVirtual.Float(
                                playerCamera.m_Orbits[0].m_Height,
                                _CAMERA_TOPRIG_HEIGHT_ATTACK,
                                1f,
                                value => playerCamera.m_Orbits[0].m_Height = value
                            );
                            DOVirtual.Float(
                                playerCamera.m_Orbits[0].m_Radius,
                                _CAMERA_TOPRIG_RADIUS_ATTACK,
                                1f,
                                value => playerCamera.m_Orbits[0].m_Radius = value
                            );
                            DOVirtual.Float(
                                playerCamera.m_Orbits[1].m_Height,
                                _CAMERA_MIDDLERIG_HEIGHT_ATTACK,
                                1f,
                                value => playerCamera.m_Orbits[1].m_Height = value
                            );
                            DOVirtual.Float(
                                playerCamera.m_Orbits[1].m_Radius,
                                _CAMERA_MIDDLERIG_RADIUS_ATTACK,
                                1f,
                                value => playerCamera.m_Orbits[1].m_Radius = value
                            );
                            break;

                        case GameManager.GamePhase.Attack:
                            // ゲームフェーズが変わるまで待つ
                            await UniTask.WaitUntil(() => GameManager.CurrentGamePhase != GameManager.GamePhase.Attack, cancellationToken: token);

                            // ゲームフェーズが変わったらカメラの距離を変更
                            DOVirtual.Float(
                                playerCamera.m_Orbits[0].m_Height,
                                _CAMERA_TOPRIG_HEIGHT_IDLE,
                                1f,
                                value => playerCamera.m_Orbits[0].m_Height = value
                            );
                            DOVirtual.Float(
                                playerCamera.m_Orbits[0].m_Radius,
                                _CAMERA_TOPRIG_RADIUS_IDLE,
                                1f,
                                value => playerCamera.m_Orbits[0].m_Radius = value
                            );
                            DOVirtual.Float(
                                playerCamera.m_Orbits[1].m_Height,
                                _CAMERA_MIDDLERIG_HEIGHT_IDLE,
                                1f,
                                value => playerCamera.m_Orbits[1].m_Height = value
                            );
                            DOVirtual.Float(
                                playerCamera.m_Orbits[1].m_Radius,
                                _CAMERA_MIDDLERIG_RADIUS_IDLE,
                                1f,
                                value => playerCamera.m_Orbits[1].m_Radius = value
                            );
                            break;

                        default:
                            DebugLogger.LogError("ChangeCameraDistance: operation failed.", this);
                            break;
                    }
                }
            } catch (OperationCanceledException)
            {
                DebugLogger.LogWarning("ChangeCameraDistance() was canceled.", this);
            }
        }
    }
}
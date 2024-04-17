using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Systems;
using _00_GameData.Scripts.UI;
using _00_GameData.Scripts.Weapon;
using AnnulusGames.LucidTools.Audio;
using Cinemachine;
using UnityEngine;

namespace _00_GameData.Scripts.Player
{
    public partial class PlayerCombat : MonoBehaviour
    {
        [Header("Components")]
        [Tooltip("プレイヤーのターゲティング用カメラ")]
        public CinemachineVirtualCamera targetingCamera;
        [Tooltip("プレイヤーのターゲティング用アイコン")]
        public TargetingIcon targetingIcon;
        [Tooltip("プレイヤーのアニメーター")]
        public Animator animator;

        [Header("Parameters")]
        [Tooltip("プレイヤーの接敵判定範囲")]
        public float enemyContactRange = 10.0f;
        [Tooltip("回復ツールの回復量")]
        public int healAmount = 50;

        [Header("Sounds")]
        [Tooltip("ロックオン音")]
        public AudioClip lockOnSound;

        private PlayerController _playerController;
        private WeaponController _weaponController;
        private PoolManager _poolManager;

        [HideInInspector] public float attackDelay;
        [HideInInspector] public float magicDelay;
        [HideInInspector] public float weaponChangeDelay;
        [HideInInspector] public float healDelay;
        private bool _isAttackedInAir;
        private bool _attackLightInput;
        private bool _attackHeavyInput;
        private bool _magicInput;
        private const float _ATTACK_LIGHT_DELAY_TIME = 0.5f;
        private const float _ATTACK_HEAVY_DELAY_TIME = 0.8f;
        private const float _MAGIC_DELAY_TIME = 0.95f;
        private const float _WEAPON_CHANGE_DELAY_TIME = 1.0f;
        private const float _HEAL_DELAY_TIME = 1.0f;
        private const string _ENEMY_CAM_TARGET_NAME = "EnemyCamTarget";

        /* Animator */
        private static readonly int AnimationWeaponSheatheDelay = Animator.StringToHash("WeaponSheatheDelay");
        private static readonly int AnimationLightAttack = Animator.StringToHash("LightAttack");
        private static readonly int AnimationHeavyAttack = Animator.StringToHash("HeavyAttack");
        private const string _ATTACK_COMBO1_01_ANIMATION_NAME = "Combo_Attack_01_01_Edited";
        private const string _ATTACK_COMBO1_02_ANIMATION_NAME = "Combo_Attack_01_02_Edited";
        private const string _ATTACK_COMBO1_03_ANIMATION_NAME = "Combo_Attack_01_03_Edited";
        private const string _ATTACK_COMBO1_04_ANIMATION_NAME = "Combo_Attack_01_04_Edited";
        private const string _ATTACK_COMBO3_01_ANIMATION_NAME = "Combo_Attack_03_01";
        private const string _ATTACK_COMBO3_02_ANIMATION_NAME = "Combo_Attack_03_02";
        private const string _ATTACK_COMBO3_03_ANIMATION_NAME = "Combo_Attack_03_03";
        private const string _ATTACK_COMBO3_04_ANIMATION_NAME = "Combo_Attack_03_04";
        private const string _ATTACK_COMBO_AIR_01_ANIMATION_NAME = "Combo_Attack_Air_01";
        private const string _ATTACK_COMBO_AIR_02_ANIMATION_NAME = "Combo_Attack_Air_02";
        private const string _ATTACK_COMBO_AIR_03_ANIMATION_NAME = "Combo_Attack_Air_03";
        private const string _ATTACK_COMBO_AIR_04_ANIMATION_NAME = "Combo_Attack_Air_04";

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _weaponController = GetComponentInChildren<WeaponController>();
        }

        private void Start()
        {
            _poolManager = GameObject.FindGameObjectWithTag(TagName.GameController).GetComponent<PoolManager>();
        }

        private void Update()
        {
            UpdateDelay();
            DetermineIsAttacking();
            Attack();
            AttackMagic();

            // 索敵判定を更新
            GameManager.CurrentGamePhase = IsEnemyInRange() ? GameManager.GamePhase.Attack : GameManager.GamePhase.Normal;

            // 空中判定を更新
            if (!PlayerController.IsFlying)
            {
                _isAttackedInAir = false;
            }

            // ターゲットアイコン座標を更新
            if (targetingCamera.enabled && targetingCamera.LookAt != null)
            {
                targetingIcon.transform.position = targetingCamera.LookAt.position + new Vector3(0, 1.5f, 0);
                targetingIcon.gameObject.SetActive(true);
            }
            else
            {
                targetingIcon.transform.position = Vector3.zero;
                targetingIcon.gameObject.SetActive(false);
            }

            // 攻撃フェーズを抜けたらカメラを初期化
            if (GameManager.CurrentGamePhase != GameManager.GamePhase.Attack)
            {
                ChangePlayerCamera();
            }
            // ターゲット中の敵が死んだ(非表示化)らカメラを初期化
            else if (targetingCamera.enabled && !targetingCamera.LookAt.parent.gameObject.activeSelf)
            {
                ChangePlayerCamera();
            }
        }

        /// <summary>
        /// ディレイ更新
        /// </summary>
        private void UpdateDelay()
        {
            // 攻撃ディレイを更新
            attackDelay += Time.deltaTime;
            attackDelay = Mathf.Clamp(attackDelay, 0f, 1.0f);

            // 魔法ディレイを更新
            magicDelay += Time.deltaTime;
            magicDelay = Mathf.Clamp(magicDelay, 0f, 1.0f);

            // 武器変更ディレイを更新
            weaponChangeDelay += Time.deltaTime;
            weaponChangeDelay = Mathf.Clamp(weaponChangeDelay, 0f, 1.0f);

            // 回復ディレイを更新
            healDelay += Time.deltaTime;
            healDelay = Mathf.Clamp(healDelay, 0f, 1.0f);
        }

        /// <summary>
        /// 敵が索敵範囲内にいるかどうかを判定
        /// </summary>
        /// <returns>範囲内にいたらtrue</returns>
        private bool IsEnemyInRange()
        {
            return Physics.CheckSphere(
                transform.position,
                enemyContactRange,
                LayerMask.GetMask(LayerMask.LayerToName(LayerName.Enemy))
            );
        }

        /// <summary>
        /// プレイヤーが攻撃中かどうかを判定
        /// </summary>
        private void DetermineIsAttacking()
        {
            var animName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            PlayerController.IsAttacking = animName
                is _ATTACK_COMBO1_01_ANIMATION_NAME
                or _ATTACK_COMBO1_02_ANIMATION_NAME
                or _ATTACK_COMBO1_03_ANIMATION_NAME
                or _ATTACK_COMBO1_04_ANIMATION_NAME
                or _ATTACK_COMBO3_01_ANIMATION_NAME
                or _ATTACK_COMBO3_02_ANIMATION_NAME
                or _ATTACK_COMBO3_03_ANIMATION_NAME
                or _ATTACK_COMBO3_04_ANIMATION_NAME
                or _ATTACK_COMBO_AIR_01_ANIMATION_NAME
                or _ATTACK_COMBO_AIR_02_ANIMATION_NAME
                or _ATTACK_COMBO_AIR_03_ANIMATION_NAME
                or _ATTACK_COMBO_AIR_04_ANIMATION_NAME;

            // 納刀ディレイを初期化
            if (animName
                is _ATTACK_COMBO1_01_ANIMATION_NAME
                or _ATTACK_COMBO3_01_ANIMATION_NAME
                or _ATTACK_COMBO_AIR_01_ANIMATION_NAME)
            {
                animator.SetFloat(AnimationWeaponSheatheDelay, 0f);
            }
        }

        /// <summary>
        /// プレイヤー攻撃
        /// </summary>
        private void Attack()
        {
            // 弱攻撃
            if (_attackLightInput)
            {
                // 攻撃アニメーションを再生
                animator.SetTrigger(AnimationLightAttack);
                // 攻撃入力をリセット
                _attackLightInput = false;
                attackDelay = 0f;
            }
            // 強攻撃
            else if (_attackHeavyInput)
            {
                // 攻撃アニメーションを再生
                animator.SetTrigger(AnimationHeavyAttack);
                // 攻撃入力をリセット
                _attackHeavyInput = false;
                attackDelay = 0f;
            }
        }

        /// <summary>
        /// プレイヤー魔法
        /// </summary>
        private void AttackMagic()
        {
            // 魔法発動
            if (_magicInput)
            {
                //TODO:魔法アニメーションを再生
                // 魔法エフェクトを発動
                //TODO:属性ごとに分ける
                _poolManager.InstantiateFireEffect(transform);
                //TODO:効果音を再生
                // STを消費
                _playerController.currentSt -= 40;
                _playerController.canHealSt = false;
                // 魔法入力をリセット
                _magicInput = false;
                magicDelay = 0f;
            }
        }

        /// <summary>
        /// プレイヤー属性変更
        /// </summary>
        /// <param name="element">変更先の属性</param>
        private void ChangePlayerElement(PlayerController.PlayerElement element)
        {
            // プレイヤーの属性を変更
            _playerController.currentElement = element;
            // 武器のメッシュを変更
            _weaponController.ChangeWeapon(element);
            // 武器変更ディレイをリセット
            weaponChangeDelay = 0f;
        }

        /// <summary>
        /// 現在のカメラをターゲティングカメラに変更
        /// </summary>
        private void ChangeTargetingCamera()
        {
            // ターゲティングカメラのターゲットを変更
            // プレイヤーから一番近い敵をターゲットとする
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(TagName.Enemy);
            float minDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;
            foreach (var enemy in enemies)
            {
                // ルート計算を軽減するため距離の二乗で計算
                float distMagnitude = (transform.position - enemy.transform.position).sqrMagnitude;
                if (distMagnitude < minDistance)
                {
                    minDistance = distMagnitude;
                    nearestEnemy = enemy;
                }
            }

            // 敵が見つからなかったら処理を終了
            if (nearestEnemy == null)
            {
                return;
            }

            // 子オブジェクトのEnemyCamTargetをターゲットに設定
            Transform enemyCam = nearestEnemy.transform.Find(_ENEMY_CAM_TARGET_NAME);
            if (enemyCam == null || !enemyCam.parent.gameObject.activeSelf)
            {
                DebugLogger.LogWarning("EnemyCamTarget not found!", this);
                return;
            }
            targetingCamera.LookAt = enemyCam;

            // ターゲティングカメラを有効化
            targetingCamera.enabled = true;

            // ターゲティングUIを表示
            targetingIcon.gameObject.SetActive(true);

            // SEを再生
            LucidAudio.PlaySE(lockOnSound)
                .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster * 20.0f);
        }

        /// <summary>
        /// 現在のカメラをプレイヤーカメラに変更
        /// </summary>
        private void ChangePlayerCamera()
        {
            // ターゲッティングカメラを無効化
            targetingCamera.enabled = false;

            // ターゲティングUIを非表示
            targetingIcon.gameObject.SetActive(false);
        }

        /// <summary>
        /// ツール上
        /// </summary>
        private void ToolUp()
        {
            ChangePlayerElement(PlayerController.PlayerElement.Fire);
        }

        /// <summary>
        /// ツール下
        /// </summary>
        private void ToolDown()
        {
            ChangePlayerElement(PlayerController.PlayerElement.Spark);
        }

        /// <summary>
        /// ツール左
        /// </summary>
        private void ToolLeft()
        {
            // HPを回復
            _playerController.currentHp += healAmount;
            _playerController.healRemaining--;
            _poolManager.InstantiateHealEffect(transform.position + Vector3.down * 0.5f);
            healDelay = 0f;

            // SEを再生
            var random = Random.Range(0, _playerController.healSounds.Length);
            LucidAudio.PlaySE(_playerController.healSounds[random])
                .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster);
        }

        /// <summary>
        /// ツール右
        /// </summary>
        private void ToolRight()
        {
            ChangePlayerElement(PlayerController.PlayerElement.Water);
        }

        private void OnDrawGizmos()
        {
            // 索敵範囲を描画
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyContactRange);

            // カメラのターゲティング位置を描画
            if (targetingCamera == null) return;
            if (targetingCamera.enabled)
            {
                Gizmos.color = Color.magenta;
                var offset = new Vector3(0, 1.5f, 0);
                Gizmos.DrawSphere(targetingCamera.LookAt.position + offset, 0.2f);
            }
        }
    }
}
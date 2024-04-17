using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Systems;
using AnnulusGames.LucidTools.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _00_GameData.Scripts.Player
{
    public partial class PlayerMovement : MonoBehaviour
    {
        [Header("Components")]
        [Tooltip("プレイヤーのアニメーター")]
        public Animator animator;

        [Header("Parameters")]
        [SerializeField, Tooltip("移動速度")]
        private float walkSpeed = 5f;
        [SerializeField, Tooltip("ジャンプの強さ")]
        private float jumpPower = 2f;

        [Header("Sounds")]
        [Tooltip("ジャンプ音")]
        public AudioClip[] jumpSounds;
        [Tooltip("着地音")]
        public AudioClip[] landSounds;

        private PlayerController _playerController;

        /* Input System */
        [HideInInspector]
        public float moveHorizontalInput;
        [HideInInspector]
        public float moveVerticalInput;
        [HideInInspector]
        public float dodgeDelay;
        private bool _jumpPressInput;
        private bool _dodgeInput;
        private bool _wasGroundedLastFrame;
        private const float _DODGE_DELAY_TIME = 0.95f;

        /* Rigidbody */
        [HideInInspector]
        public Rigidbody playerRigidbody;
        private float _moveSmoothVelocity;
        private float _turnSmoothVelocity;
        private RaycastHit _slopeHit;
        private const float _MOVE_SMOOTH_TIME = 0.15f;
        private const float _TURN_SMOOTH_TIME = 0.15f;
        private const float _VEROCITY_THRESHOLD = 0.55f;
        private const float _SLOPE_DETECTION_DISTANCE = 1.2f;
        private const float _SLOPE_GRAVITY_CORRECTION = 9.5f;

        /* Animator */
        private static readonly int AnimationMovementSpeed = Animator.StringToHash("MovementSpeed");
        private static readonly int AnimationWeaponSheatheDelay = Animator.StringToHash("WeaponSheatheDelay");
        private static readonly int AnimationIsFlying = Animator.StringToHash("IsFlying");
        private static readonly int AnimationDodge = Animator.StringToHash("Dodge");

        /* Ground Cast */
        private const float _GROUND_CAST_RADIUS = 0.3f;
        private const float _GROUND_CAST_DISTANCE = 1.0f;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            playerRigidbody = GetComponent<Rigidbody>();

            _wasGroundedLastFrame = true;
        }

        private void Update()
        {
            DetermineIsFlying();
            Dodge();
            PlayerLand();

            // 回避ディレイを更新
            dodgeDelay += Time.deltaTime;
            dodgeDelay = Mathf.Clamp(dodgeDelay, 0f, 1f);

            // 攻撃中
            if (PlayerController.IsAttacking)
            {
                // 攻撃中は移動速度を0にし、アニメーションステートの暴走を防ぐ
                animator.SetFloat(AnimationMovementSpeed, 0);
                // 納刀ディレイを初期化
                animator.SetFloat(AnimationWeaponSheatheDelay, 0);

                // 空中攻撃中は重力を無効化
                if (!PlayerController.IsFlying) return;
                playerRigidbody.useGravity = false;
            }
            else
            {
                // 重力を有効化
                playerRigidbody.useGravity = true;

                // 納刀ディレイを更新
                animator.SetFloat(AnimationWeaponSheatheDelay, animator.GetFloat(AnimationWeaponSheatheDelay) + Time.deltaTime);
            }

            // ジャンプ中
            if (PlayerController.IsFlying)
            {
                // ジャンプ中は移動速度を0にし、アニメーションステートの暴走を防ぐ
                animator.SetFloat(AnimationMovementSpeed, 0);
                // 納刀ディレイを初期化
                animator.SetFloat(AnimationWeaponSheatheDelay, 0);
            }
        }

        private void FixedUpdate()
        {
            // HPが0 or ダメージディレイ中
            if (_playerController.currentHp <= 0 || _playerController.canDamageDelay > 0)
            {
                // 移動速度を初期化
                animator.SetFloat(AnimationMovementSpeed, 0);
                // 納刀ディレイを初期化
                animator.SetFloat(AnimationWeaponSheatheDelay, 0);
                return;
            }

            PlayerMove(moveHorizontalInput, moveVerticalInput);
            PlayerJump(_jumpPressInput);
        }

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        /// <param name="horizontalInput">前後入力</param>
        /// <param name="verticalInput">左右入力</param>
        private void PlayerMove(float horizontalInput, float verticalInput)
        {
            // 0~1の範囲に正規化
            var direction = new Vector3(horizontalInput, 0, verticalInput);
            if (direction.magnitude >= _VEROCITY_THRESHOLD)
            {
                // 移動方向を取得
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                // 移動方向に向く
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _TURN_SMOOTH_TIME);
                transform.rotation = Quaternion.Euler(0, angle, 0);

                // 攻撃中は移動しない
                if (PlayerController.IsAttacking) return;
                // 移動
                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                float moveSpeed = walkSpeed * direction.magnitude;

                if (PlayerOnSlope())
                {
                    // 斜面を考慮した移動
                    var slopeForce = new Vector3(GetSlopeVector(moveDir, _slopeHit).x * moveSpeed, _SLOPE_GRAVITY_CORRECTION, GetSlopeVector(moveDir, _slopeHit).z * moveSpeed);
                    slopeForce = Vector3.ClampMagnitude(slopeForce, moveSpeed);
                    playerRigidbody.AddForce(slopeForce, ForceMode.Force);
                }
                else
                {
                    // 通常の移動
                    playerRigidbody.AddForce(new Vector3(moveDir.x * moveSpeed, 0, moveDir.z * moveSpeed), ForceMode.Force);
                }
            }

            // アニメーション
            // 移動速度が一定以上の場合はアニメーションを再生
            if (playerRigidbody.velocity.magnitude > _VEROCITY_THRESHOLD)
            {
                var speed = playerRigidbody.velocity.magnitude * direction.magnitude;
                var smoothed = Mathf.SmoothDamp(animator.GetFloat(AnimationMovementSpeed), speed, ref _moveSmoothVelocity, _MOVE_SMOOTH_TIME);
                animator.SetFloat(AnimationMovementSpeed, smoothed);
            }
            else
            {
                animator.SetFloat(AnimationMovementSpeed, 0);
            }
        }

        /// <summary>
        /// プレイヤーが坂道にいるかどうか
        /// </summary>
        /// <returns>坂道なら true</returns>
        public bool PlayerOnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _SLOPE_DETECTION_DISTANCE))
            {
                // 坂道の角度が一定以上なら坂道と判定
                if (_slopeHit.normal != Vector3.up && _slopeHit.normal.y > 0.5f)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 坂道での移動方向を取得
        /// </summary>
        /// <param name="moveDir">移動方向</param>
        /// <param name="hit">坂道判定レイキャスト</param>
        /// <returns>坂道での移動方向</returns>
        private Vector3 GetSlopeVector(Vector3 moveDir, RaycastHit hit)
        {
            return Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
        }

        /// <summary>
        /// プレイヤーのジャンプ
        /// </summary>
        /// <param name="jumpPressInput">ジャンプ入力</param>>
        private void PlayerJump(bool jumpPressInput)
        {
            // ジャンプ入力がない場合、または接地していない場合はジャンプしない
            if (!jumpPressInput) return;
            if (!CheckIfGrounded()) return;

            // ジャンプ
            playerRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            _jumpPressInput = false;

            // SEを再生
            var random = Random.Range(0, jumpSounds.Length);
            LucidAudio.PlaySE(jumpSounds[random])
                .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster * 0.25f);
        }

        /// <summary>
        /// プレイヤー回避
        /// </summary>
        private void Dodge()
        {
            if (!_dodgeInput) return;

            // 回避アニメーションを再生
            animator.SetTrigger(AnimationDodge);
            // ST値を減らす
            _playerController.currentSt -= 30f;
            // 回避入力をリセット
            _dodgeInput = false;
            dodgeDelay = 0f;
            _playerController.canHealSt = false;
        }

        /// <summary>
        /// プレイヤーが着地した瞬間の処理
        /// </summary>
        private void PlayerLand()
        {
            var isGroundedThisFrame = CheckIfGrounded();
            if (!_wasGroundedLastFrame && isGroundedThisFrame)
            {
                // SEを再生
                var random = Random.Range(0, landSounds.Length);
                LucidAudio.PlaySE(landSounds[random])
                    .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster * 0.25f);
            }

            _wasGroundedLastFrame = isGroundedThisFrame;
        }

        /// <summary>
        /// プレイヤーが空中にいる場合の判定分岐
        /// </summary>
        private void DetermineIsFlying()
        {
            // 接地しているかどうかでアニメーションを切り替える
            if (CheckIfGrounded())
            {
                animator.SetBool(AnimationIsFlying, false);
                PlayerController.IsFlying = false;
            }
            else
            {
                animator.SetBool(AnimationIsFlying, true);
                PlayerController.IsFlying = true;
            }
        }

        /// <summary>
        /// 地面との接触判定(足元)
        /// </summary>
        /// <returns>接地していればtrue</returns>
        private bool CheckIfGrounded()
        {
            // SphereCastで地面との接触判定
            return Physics.SphereCast(
                transform.position,
                _GROUND_CAST_RADIUS,
                Vector3.down,
                out _,
                _GROUND_CAST_DISTANCE,
                LayerMask.GetMask(LayerMask.LayerToName(LayerName.Ground)),
                QueryTriggerInteraction.Ignore
            );
        }

        private void OnDrawGizmos()
        {
            // 地面との接触判定を可視化
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * _GROUND_CAST_DISTANCE, _GROUND_CAST_RADIUS);

            // 坂道判定を可視化
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _SLOPE_DETECTION_DISTANCE);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace _00_GameData.Scripts.Player
{
    public partial class PlayerMovement
    {
        /// <summary>
        /// プレイヤーの移動入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_Move(InputAction.CallbackContext context)
        {
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0)
            {
                moveHorizontalInput = 0;
                moveVerticalInput = 0;
                return;
            }

            moveHorizontalInput = context.ReadValue<Vector2>().x;
            moveVerticalInput = context.ReadValue<Vector2>().y;
        }

        /// <summary>
        /// プレイヤーのジャンプ入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_Jump_Press(InputAction.CallbackContext context)
        {
            // ジャンプボタンを押した瞬間のみジャンプ
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 攻撃中ならキャンセル
            if (PlayerController.IsAttacking) return;
            // 空中にいたらキャンセル
            if (PlayerController.IsFlying) return;

            _jumpPressInput = context.ReadValue<float>() > 0.5f;
        }

        /// <summary>
        /// プレイヤーの回避入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChanged_Dodge_Press(InputAction.CallbackContext context)
        {
            // 回避ボタンを押した瞬間のみ回避
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 空中にいたらキャンセル
            if (PlayerController.IsFlying) return;
            // プレイヤーのSTが足りなければキャンセル
            if (_playerController.currentSt < 30) return;
            // 回避ディレイ中ならキャンセル
            if (dodgeDelay < _DODGE_DELAY_TIME) return;
            if (!_playerController.canHealSt) return;

            _dodgeInput = context.ReadValue<float>() > 0.5f;
        }
    }
}
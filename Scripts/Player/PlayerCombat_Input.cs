using _00_GameData.Scripts.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _00_GameData.Scripts.Player
{
    public partial class PlayerCombat
    {
        /// <summary>
        /// プレイヤーの弱攻撃入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_AttackLight_Press(InputAction.CallbackContext context)
        {
            // 攻撃ボタンを押した瞬間のみ攻撃
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 攻撃ディレイ中なら処理しない
            if (attackDelay < _ATTACK_LIGHT_DELAY_TIME) return;
            // 空中攻撃を既に行っていたら処理しない
            if (_isAttackedInAir) return;
            // 最後の攻撃中なら処理しない
            string animName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            switch (animName)
            {
                case _ATTACK_COMBO1_04_ANIMATION_NAME:
                case _ATTACK_COMBO_AIR_03_ANIMATION_NAME:
                    // 着地するまで、空中攻撃を出せないようにする
                    if (!PlayerController.IsFlying) return;
                    _isAttackedInAir = true;
                    return;

                default:
                    _attackLightInput = context.ReadValue<float>() > 0.5f;
                    break;
            }
        }

        /// <summary>
        /// プレイヤーの強攻撃入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_AttackHeavy_Press(InputAction.CallbackContext context)
        {
            // 攻撃ボタンを押した瞬間のみ攻撃
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 攻撃ディレイ中なら処理しない
            if (attackDelay < _ATTACK_HEAVY_DELAY_TIME) return;
            // 最後の攻撃中なら処理しない
            string animName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            switch (animName)
            {
                case _ATTACK_COMBO3_04_ANIMATION_NAME:
                    return;

                default:
                    _attackHeavyInput = context.ReadValue<float>() > 0.1f;
                    break;
            }
        }

        /// <summary>
        /// プレイヤーの魔法入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChanged_Magic(InputAction.CallbackContext context)
        {
            // 魔法ボタンを押した瞬間のみ発動
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 攻撃フェーズでなければ処理しない
            // if (GameManager.CurrentGamePhase != GameManager.GamePhase.Attack) return;
            // STが足りなければ処理しない
            if (_playerController.currentSt < 40) return;
            // 魔法ディレイ中なら処理しない
            if (magicDelay < _MAGIC_DELAY_TIME) return;

            _magicInput = context.ReadValue<float>() > 0.5f;
        }

        /// <summary>
        /// プレイヤーのターゲッティング入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChanged_Targeting(InputAction.CallbackContext context)
        {
            // ターゲットボタンを押した瞬間のみ発動
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 攻撃フェーズでなければ処理しない
            if (GameManager.CurrentGamePhase != GameManager.GamePhase.Attack) return;

            // ターゲットカメラの有効/無効に応じて処理
            if (targetingCamera.enabled)
                ChangePlayerCamera();
            else
                ChangeTargetingCamera();
        }

        /// <summary>
        /// ツール上入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_ToolUp_Press(InputAction.CallbackContext context)
        {
            /* 火属性武器に変更 */

            // ボタンを押した瞬間のみ処理
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 現在の武器が火属性なら処理しない
            if (_playerController.currentElement == PlayerController.PlayerElement.Fire) return;
            // 武器変更ディレイ中なら処理しない
            if (weaponChangeDelay < _WEAPON_CHANGE_DELAY_TIME) return;

            ToolUp();
        }

        /// <summary>
        /// ツール下入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_ToolDown_Press(InputAction.CallbackContext context)
        {
            /* 雷属性武器に変更 */

            // ボタンを押した瞬間のみ処理
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 現在の武器が雷属性なら処理しない
            if (_playerController.currentElement == PlayerController.PlayerElement.Spark) return;
            // 武器変更ディレイ中なら処理しない
            if (weaponChangeDelay < _WEAPON_CHANGE_DELAY_TIME) return;

            ToolDown();
        }

        /// <summary>
        /// ツール左入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_ToolLeft_Press(InputAction.CallbackContext context)
        {
            /* 回復 */

            // ボタンを押した瞬間のみ処理
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // HPが0なら処理しない
            if (_playerController.currentHp <= 0) return;
            // HPが最大なら処理しない
            if (_playerController.currentHp >= _playerController.maxHp) return;
            // 回復回数が0なら処理しない
            if (_playerController.healRemaining <= 0) return;
            // 回復ディレイ中なら処理しない
            if (healDelay < _HEAL_DELAY_TIME) return;

            ToolLeft();
        }

        /// <summary>
        /// ツール右入力
        /// </summary>
        /// <param name="context">入力コンテクスト</param>
        public void OnValueChange_ToolRight_Press(InputAction.CallbackContext context)
        {
            /* 水属性武器に変更 */

            // ボタンを押した瞬間のみ処理
            if (!context.started) return;
            // TimeScaleが止まっていたら処理しない
            if (Time.timeScale == 0) return;
            // 現在の武器が水属性なら処理しない
            if (_playerController.currentElement == PlayerController.PlayerElement.Water) return;
            // 武器変更ディレイ中なら処理しない
            if (weaponChangeDelay < _WEAPON_CHANGE_DELAY_TIME) return;

            ToolRight();
        }
    }
}
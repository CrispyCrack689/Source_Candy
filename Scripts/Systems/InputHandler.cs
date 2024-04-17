using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _00_GameData.Scripts.Systems
{
    public class InputHandler : MonoBehaviour
    {
        [Header("Debug")]
        [ReadOnly, Tooltip("現在の入力タイプ")]
        public InputType currentInputType = InputType.Keyboard;

        private void Update()
        {
            // 入力タイプを切り替える
            // マウスが動いたらMouse
            // いずれかのキーが押されたらKeyboard
            // コントローラーのスティック or ボタンが押されたらController
            if (Mouse.current.delta.ReadValue().magnitude > 0)
                currentInputType = InputType.Mouse;
            else if (Keyboard.current.anyKey.isPressed)
                currentInputType = InputType.Keyboard;
            else if (DetermineIsController())
                currentInputType = InputType.Controller;
        }

        /// <summary>
        /// 入力タイプ
        /// </summary>
        public enum InputType
        {
            Mouse,
            Keyboard,
            Controller
        }

        /// <summary>
        /// コントローラー入力を判定する
        /// </summary>
        /// <returns>コントローラー入力</returns>
        public static bool DetermineIsController()
        {
            // コントローラーが接続されていない場合はnull
            if (Gamepad.current == null) return false;

            // コントローラーのスティック or いずれかのボタンが押されたらController
            // 方向キーが押されてもControllerになる
            return Gamepad.current.buttonNorth.isPressed || Gamepad.current.buttonSouth.isPressed ||
                   Gamepad.current.buttonEast.isPressed || Gamepad.current.buttonWest.isPressed ||
                   Gamepad.current.leftStick.ReadValue().magnitude > 0 || Gamepad.current.rightStick.ReadValue().magnitude > 0 ||
                   Gamepad.current.leftTrigger.isPressed || Gamepad.current.rightTrigger.isPressed ||
                   Gamepad.current.leftShoulder.isPressed || Gamepad.current.rightShoulder.isPressed ||
                   Gamepad.current.dpad.up.isPressed || Gamepad.current.dpad.down.isPressed ||
                   Gamepad.current.dpad.left.isPressed || Gamepad.current.dpad.right.isPressed ||
                   Gamepad.current.startButton.isPressed || Gamepad.current.selectButton.isPressed;
        }
    }
}
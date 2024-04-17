using System.IO;
using _00_GameData.Scripts.Save;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
#endif

namespace _00_GameData.Scripts.Internal
{
    public static class Utils
    {
        /* Common */

        /// <summary>
        /// 指定した親オブジェクトの子オブジェクトから指定したタグのオブジェクトを取得する
        /// </summary>
        /// <param name="parent">指定した親オブジェクト</param>
        /// <param name="tag">指定したタグ</param>
        /// <returns>指定したタグの子オブジェクト</returns>
        public static GameObject FindChildWithTag(GameObject parent, string tag)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
            }

            DebugLogger.LogError("Child object that you specified is not found.", parent);
            return null;
        }

        /* Data Save */
        public static SaveData saveData;

        /// <summary>
        /// ローカルにセーブデータを保存
        /// </summary>
        public static void Save()
        {
            var filePath = Application.persistentDataPath + "/" + "SaveData.json";
            var json = JsonUtility.ToJson(saveData);
            var streamWriter = new StreamWriter(filePath);
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();

            Debug.Log("Saved.");
        }

        /// <summary>
        /// ローカルからセーブデータを読み込み
        /// </summary>
        public static void Load()
        {
            var filePath = Application.persistentDataPath + "/" + "SaveData.json";
            if (File.Exists(filePath))
            {
                var streamReader = new StreamReader(filePath);
                var data = streamReader.ReadToEnd();
                streamReader.Close();
                saveData = JsonUtility.FromJson<SaveData>(data);

                Debug.Log("Save data found in your local directory.");
            }
            else
            {
                saveData = new SaveData();
                var json = JsonUtility.ToJson(saveData);
                var streamWriter = new StreamWriter(filePath);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                Debug.LogWarning("Save data is missing. Making new one...");
            }
        }

#if ENABLE_INPUT_SYSTEM
        /* Input System */

        /// <summary>
        /// 現在の入力デバイスがXboxコントローラーかどうかを判断
        /// </summary>
        /// <returns>Xboxコントローラーであれば true</returns>
        public static bool DetermineIsXbox()
        {
            var gamepad = Gamepad.current;
            return gamepad is XInputController;
        }

        /// <summary>
        /// 現在の入力デバイスがPlayStationコントローラーかどうかを判断
        /// </summary>
        /// <returns>PlayStationコントローラーであれば true</returns>
        public static bool DetermineIsPlayStation()
        {
            var gamepad = Gamepad.current;
            return gamepad is DualShockGamepad;
        }

        /// <summary>
        /// 現在の入力デバイスがNintendo Switchコントローラーかどうかを判断
        /// </summary>
        /// <returns>Switchコントローラーであれば true</returns>
        public static bool DetermineIsSwitch()
        {
            var gamepad = Gamepad.current;
            return gamepad is SwitchProControllerHID;
        }

        /// <summary>
        /// 現在の入力デバイスがキーボード・マウスかどうかを判断
        /// </summary>
        /// <returns>キーボード・マウスであれば true</returns>
        public static bool DetermineIsKeyboardMouse()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return false;

            var anyKey = keyboard.anyKey.wasPressedThisFrame;
            var mouseDrag = mouse.delta.ReadValue();
            if (anyKey || mouseDrag != Vector2.zero)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 現在の入力デバイスがコントローラーかどうかを判断
        /// </summary>
        /// <returns>コントローラーであれば true</returns>
        public static bool DetermineIsController()
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return false;

            var leftStick = gamepad.leftStick.ReadValue();
            var rightStick = gamepad.rightStick.ReadValue();
            var north = gamepad.buttonNorth.wasPressedThisFrame;
            var south = gamepad.buttonSouth.wasPressedThisFrame;
            var west = gamepad.buttonWest.wasPressedThisFrame;
            var east = gamepad.buttonEast.wasPressedThisFrame;
            var dPad = gamepad.dpad.ReadValue();
            var lestStickPress = gamepad.leftStickButton.wasPressedThisFrame;
            var rightStickPress = gamepad.rightStickButton.wasPressedThisFrame;
            var leftShoulder = gamepad.leftShoulder.wasPressedThisFrame;
            var rightShoulder = gamepad.rightShoulder.wasPressedThisFrame;
            var leftTrigger = gamepad.leftTrigger.wasPressedThisFrame;
            var rightTrigger = gamepad.rightTrigger.wasPressedThisFrame;
            var start = gamepad.startButton.wasPressedThisFrame;
            var select = gamepad.selectButton.wasPressedThisFrame;
            if (leftStick != Vector2.zero || rightStick != Vector2.zero
                                          || north || south || west || east
                                          || dPad != Vector2.zero
                                          || lestStickPress || rightStickPress
                                          || leftShoulder || rightShoulder
                                          || leftTrigger || rightTrigger
                                          || start || select
            )
            {
                return true;
            }
            return false;
        }
#endif
    }
}
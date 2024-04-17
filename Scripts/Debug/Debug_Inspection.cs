using _00_GameData.Scripts.Player;
using _00_GameData.Scripts.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class Debug_Inspection : MonoBehaviour
{
    private PlayerController _playerController;
    private PlayerMovement _playerMovement;
    private PlayerCombat _playerCombat;

    private void Awake()
    {
        _playerController = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerController>();
        _playerMovement = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerMovement>();
        _playerCombat = GameObject.FindWithTag(TagName.Player).GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        DebugCommand_Keyboard();
        DebugCommand_Gamepad();
    }

    private void OnGUI()
    {
        if (_playerMovement != null)
        {
            // プレイヤー速度を表示
            // 左上
            Vector3 playerVelocity = _playerMovement.playerRigidbody.velocity;
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 10, 1000, 1000), "Player velocity magnitude: " + playerVelocity.magnitude);

            // プレイヤーの現在のアニメーションを表示
            // 左上
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 30, 1000, 1000), "Player animation: " + _playerMovement.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);

            // プレイヤーの攻撃ディレイを表示
            // 左上
            if (_playerCombat.attackDelay < 0.5f) GUI.color = Color.red;
            else if (_playerCombat.attackDelay < 0.8f) GUI.color = Color.yellow;
            else GUI.color = Color.black;
            GUI.Label(new Rect(10, 50, 1000, 1000), "Player attack delay time: " + _playerCombat.attackDelay);

            // プレイヤーの回避ディレイを表示
            // 左上
            if (_playerMovement.dodgeDelay < 0.95f) GUI.color = Color.red;
            else GUI.color = Color.black;
            GUI.Label(new Rect(10, 70, 1000, 1000), "Player dodge delay time: " + _playerMovement.dodgeDelay);

            // プレイヤーのHP,ST,TOXIC,属性を表示
            // 左上
            GUI.color = _playerController.currentHp <= 30 ? Color.red : Color.black;
            GUI.Label(new Rect(10, 90, 1000, 1000), "Player HP: " + _playerController.currentHp);
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 110, 1000, 1000), "Player ST: " + _playerController.currentSt);
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 130, 1000, 1000), "Player TOXIC: " + _playerController.currentToxic);
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 150, 1000, 1000), "Player element: " + _playerController.currentElement);

            // プレイヤーの属性切り替えディレイを表示
            // 左上
            if (_playerCombat.weaponChangeDelay < 1.0f) GUI.color = Color.red;
            else GUI.color = Color.black;
            GUI.Label(new Rect(10, 170, 1000, 1000), "Player weapon change delay time: " + _playerCombat.weaponChangeDelay);

            // プレイヤーの魔法ディレイを表示
            // 左上
            if (_playerCombat.magicDelay < 0.95f) GUI.color = Color.red;
            else GUI.color = Color.black;
            GUI.Label(new Rect(10, 190, 1000, 1000), "Player magic delay time: " + _playerCombat.magicDelay);

            // プレイヤー座標を表示
            // 左上
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 210, 1000, 1000), "XYZ: " + _playerController.transform.position);
            
            // プレイヤーの斜面接触判定を表示
            // 左上
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 230, 1000, 1000), "Slope contact: " + _playerMovement.PlayerOnSlope());

            // シーンをリスタート
            // 左下
            GUI.color = Color.white;
            if (GUI.Button(new Rect(10, Screen.height - 30, 150, 20), "Restart this scene"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // ゲーム終了
            // 左下
            GUI.color = Color.white;
            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 20), "Quit game"))
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

            // ゲームフェーズを表示
            // 右上
            GUI.color = Color.black;
            GUI.Label(new Rect(Screen.width - 160, 10, 1000, 1000), "Game phase: " + GameManager.CurrentGamePhase);

            // プレイヤー攻撃中フラグを表示
            // 右上
            GUI.color = Color.black;
            GUI.Label(new Rect(Screen.width - 160, 30, 1000, 1000), "Player attacking: " + PlayerController.IsAttacking);

            // プレイヤー空中フラグを表示
            // 右上
            GUI.color = Color.black;
            GUI.Label(new Rect(Screen.width - 160, 50, 1000, 1000), "Player flying: " + PlayerController.IsFlying);
        }
        else
        {
            // プレイヤーが見つからない場合
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 1000, 1000), "Player not found");
        }
    }

    /// <summary>
    /// デバッグコマンド:キーボード
    /// </summary>
    private void DebugCommand_Keyboard()
    {
        // ゲームフェーズを変更
        // Pキー
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (GameManager.CurrentGamePhase == GameManager.GamePhase.Normal)
            {
                GameManager.CurrentGamePhase = GameManager.GamePhase.Attack;
            }
            else if (GameManager.CurrentGamePhase == GameManager.GamePhase.Attack)
            {
                GameManager.CurrentGamePhase = GameManager.GamePhase.Normal;
            }
        }

        // プレイヤーHPを変更
        // Iキー
        if (Input.GetKeyDown(KeyCode.I))
        {
            _playerController.Damage(10);
        }
        // Oキー
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (_playerController.currentHp <= 0) return;
            _playerController.currentHp += 10;
        }

        // プレイヤーTOXICを変更
        // Kキー
        if (Input.GetKeyDown(KeyCode.K))
        {
            _playerController.currentToxic -= 10;
        }
        // Lキー
        if (Input.GetKeyDown(KeyCode.L))
        {
            _playerController.currentToxic += 10;
        }
    }

    /// <summary>
    /// デバッグコマンド:ゲームパッド
    /// </summary>
    private void DebugCommand_Gamepad()
    {
        // ゲームパッドが接続されていない場合は処理しない
        if (Gamepad.current == null) return;
        // Viewボタン長押し中はデバッグコマンド受付
        if (!Gamepad.current.selectButton.isPressed) return;

        // ゲームフェーズを変更
        // Dパッド上
        if (Gamepad.current.dpad.up.wasPressedThisFrame)
        {
            if (GameManager.CurrentGamePhase == GameManager.GamePhase.Normal)
            {
                GameManager.CurrentGamePhase = GameManager.GamePhase.Attack;
            }
            else if (GameManager.CurrentGamePhase == GameManager.GamePhase.Attack)
            {
                GameManager.CurrentGamePhase = GameManager.GamePhase.Normal;
            }
        }
    }
}
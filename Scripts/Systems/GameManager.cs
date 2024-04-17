using Sirenix.OdinInspector;
using UnityEngine;

namespace _00_GameData.Scripts.Systems
{
    public class GameManager : MonoBehaviour
    {
        /* Static */
        public static GamePhase CurrentGamePhase = GamePhase.Normal;
        public static bool IsGameStarted = false;

        [Header("Debug")]
        [SerializeField, ReadOnly]
        private GamePhase currentGamePhase = GamePhase.Normal;

        private void Awake()
        {
            CurrentGamePhase = GamePhase.Normal;
        }

        private void Update()
        {
            currentGamePhase = CurrentGamePhase;
        }

        /// <summary>
        /// ゲームフェーズ
        /// </summary>
        public enum GamePhase
        {
            Normal,
            Attack
        }

        /// <summary>
        /// ゲームスピード変更
        /// </summary>
        /// <param name="timeScale">変更先のゲームスピード</param>
        public static void ChangeGameTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }
    }
}
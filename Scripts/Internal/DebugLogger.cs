using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace _00_GameData.Scripts.Internal
{
    public static class DebugLogger
    {
        /// <summary>
        /// デバッグログを出力する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="context">対象オブジェクト(通常はthis)</param>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, UnityEngine.Object context)
        {
            Debug.Log(message, context);
        }

        /// <summary>
        /// デバッグログを出力する(警告)
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="context">対象オブジェクト(通常はthis)</param>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, UnityEngine.Object context)
        {
            Debug.LogWarning(message, context);
        }

        /// <summary>
        /// デバッグログを出力する(エラー)
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="context">対象オブジェクト(通常はthis)</param>
        public static void LogError(object message, UnityEngine.Object context)
        {
            Debug.LogError(message, context);
        }
    }
}
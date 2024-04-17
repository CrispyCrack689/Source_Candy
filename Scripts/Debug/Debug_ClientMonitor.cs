using System;
using System.Threading;
using _00_GameData.Scripts.Internal;
using Cysharp.Threading.Tasks;
using UnityEngine;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class Debug_ClientMonitor : MonoBehaviour
{
    private float currentFPS;

    private void Awake()
    {
        CancellationToken cancellationToken = destroyCancellationToken;
        GetFPS(cancellationToken);
    }

    private void OnGUI()
    {
        // 現在のFPSを表示
        // 右下
        GUI.color = Color.black;
        GUI.Label(new Rect(Screen.width - 250, Screen.height - 50, 1000, 1000), "FPS: " + (int)currentFPS);

        // 現在のメモリ使用量を表示
        // 右下
        GUI.color = Color.black;
        GUI.Label(new Rect(Screen.width - 375, Screen.height - 50, 1000, 1000), "Memory: " + (int)(GC.GetTotalMemory(false) / 1000000) + "MB");
        
        // 現在のバージョンを表示
        // 中央上
        GUI.color = Color.black;
        GUI.Label(new Rect(Screen.width / 2 - 50, 0, 1000, 1000), "Version: " + Application.version);
    }

    /// <summary>
    /// FPSを取得するタスク
    /// </summary>
    /// <param name="token">キャンセルトークン</param>
    private async void GetFPS(CancellationToken token)
    {
        try
        {
            while (true)
            {
                // 0.5秒待機
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken:token);
                // FPSを返す
                currentFPS = 1f / Time.deltaTime;
            }
        }
        catch (OperationCanceledException)
        {
            DebugLogger.LogWarning("GetFPS() was canceled.", this);
        }
    }
}
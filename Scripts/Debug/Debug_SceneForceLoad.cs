using AnnulusGames.SceneSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class Debug_SceneForceLoad : MonoBehaviour
{
    /*
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ForceLoad()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // 特定シーンでは除外
        if (SceneManager.GetSceneByName(SceneName.MainMenu).isLoaded) return;
        if (SceneManager.GetSceneByName(SceneName.LoadingScene).isLoaded) return;

        // シーンを強制ロード
        if (SceneManager.GetSceneByName(SceneName.Dev).isLoaded) return;
        Scenes.LoadScenesAsync(SceneName.Dev);
        if (SceneManager.GetSceneByName(SceneName.Managers).isLoaded) return;
        Scenes.LoadScenesAsync(SceneName.Managers);
        if (SceneManager.GetSceneByName(SceneName.UserInterface).isLoaded) return;
        Scenes.LoadScenesAsync(SceneName.UserInterface);
#endif
    }
    */
}
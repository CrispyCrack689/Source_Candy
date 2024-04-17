using System;
using AnnulusGames.SceneSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _00_GameData.Scripts.Systems
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Loading Screen")]
        public LoadingScreen loadingScreenPrefab;
        public SceneReference[] scenesToLoad;

        [Header("UI")]
        public Image fadePanel;

        private void Start()
        {
            LoadScene();
            fadePanel.DOFade(0, 0.5f)
                .SetEase(Ease.Linear).SetUpdate(true);
        }

        /// <summary>
        /// ゲームシーンをロード
        /// </summary>
        public void LoadScene()
        {
            // ロード画面のPrefabを生成、永続的にする
            var loadScreen = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(loadScreen.gameObject);

            // 複数シーンをロード
            Scenes.LoadScenesAsync(scenesToLoad)
                .WithLoadingScreen(loadScreen);
        }
    }
}
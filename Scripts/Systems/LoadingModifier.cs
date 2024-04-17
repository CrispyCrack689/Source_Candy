using System;
using _00_GameData.Scripts.Internal;
using AnnulusGames.SceneSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _00_GameData.Scripts.Systems
{
    public class LoadingModifier : MonoBehaviour
    {
        public Image fadePanel;
        public RectTransform contents;

        private LoadingScreen _loadingScreenPrefab;

        private void Awake()
        {
            _loadingScreenPrefab = GetComponent<LoadingScreen>();
        }

        public void UnloadPreviousScene(string sceneName)
        {
            DebugLogger.Log("UnloadPreviousScene", this);

            // 暗転
            fadePanel.DOFade(1, 0.5f)
                .SetEase(Ease.Linear).SetUpdate(true)
                .onComplete += () =>
            {
                // ロードしたシーンをアクティブ化
                _loadingScreenPrefab.AllowCompletion();

                // コンテンツを非アクティブ化
                contents.gameObject.SetActive(false);

                // 明転
                fadePanel.DOFade(0, 0.5f)
                    .SetEase(Ease.Linear).SetUpdate(true)
                    .onComplete += () =>
                {
                    // 削除
                    Destroy(gameObject);
                };
            };

            // 前のシーンをアンロード
            // Scenes.UnloadSceneAsync(sceneName);
        }
    }
}
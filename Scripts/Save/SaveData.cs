using System;

namespace _00_GameData.Scripts.Save
{
    [Serializable]
    public class SaveData
    {
        /// <summary>
        /// マスター音量
        /// </summary>
        public float gameVolumeMaster = 0.8f;
        /// <summary>
        /// BGM音量
        /// </summary>
        public float gameVolumeBGM = 0.8f;
        /// <summary>
        /// SE音量
        /// </summary>
        public float gameVolumeSfx = 0.8f;
        /// <summary>
        /// 入力レイアウト
        /// </summary>
        public int gameInputLayout = 0;
        /// <summary>
        /// グラフィックス品質
        /// </summary>
        public int gameGraphicsQuality = 0;
        /// <summary>
        /// 言語
        /// </summary>
        public int gameLanguage = 1;
    }
}
using TMPro;
using UnityEngine;

namespace _00_GameData.Scripts.UI
{
    public class GameVersion : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUgui;

        private void Start()
        {
            // TMP取得、バージョン番号を反映
            _textMeshProUgui = GetComponent<TextMeshProUGUI>();
            _textMeshProUgui.text = "v" + Application.version;
        }
    }
}

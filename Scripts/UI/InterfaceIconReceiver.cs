using UnityEngine;
using UnityEngine.UI;

namespace _00_GameData.Scripts.UI
{
    public class InterfaceIconReceiver : MonoBehaviour
    {
        [SerializeField,Tooltip("アイコンの種類(キー値)")]
        private string iconType;
        
        private InterfaceIconManager _interfaceIconManager;
        private Image image;
        
        private void Awake()
        {
            _interfaceIconManager = GetComponentInParent<InterfaceIconManager>();
            image = GetComponent<Image>();
        }

        private void Update()
        {
            // Empty時
            if (string.IsNullOrEmpty(iconType))
            {
                image.sprite = null;
                return;
            }
            
            // Dictionaryからアイコンを取得
            switch (_interfaceIconManager.inputType)
            {
                case InterfaceIconManager.InputType.Keyboard:
                    image.sprite = _interfaceIconManager.IconDictKeyboard[iconType];
                    break;
                case InterfaceIconManager.InputType.Xbox:
                    image.sprite = _interfaceIconManager.IconDictXboxController[iconType];
                    break;
                case InterfaceIconManager.InputType.PlayStation:
                    image.sprite = _interfaceIconManager.IconDictPlaystationController[iconType];
                    break;
                case InterfaceIconManager.InputType.NintendoSwitch:
                    image.sprite = _interfaceIconManager.IconDictSwitchController[iconType];
                    break;
                case InterfaceIconManager.InputType.SteamDeck:
                    image.sprite = _interfaceIconManager.IconDictSteamDeck[iconType];
                    break;
            }
        }
    }
}

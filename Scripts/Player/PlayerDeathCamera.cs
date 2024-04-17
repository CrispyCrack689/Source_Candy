using Cinemachine;
using UnityEngine;

namespace _00_GameData.Scripts.Player
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class PlayerDeathCamera : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("カメラ上昇速度")]
        public float cameraRiseSpeed = 3f;
    }
}

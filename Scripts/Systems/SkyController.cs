using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace _00_GameData.Scripts.Systems
{
    [RequireComponent(typeof(DirectionalLight))]
    public class SkyController : MonoBehaviour
    {
        [Tooltip("回転速度")]
        [SerializeField] private float rotateSpeed = 0.1f;

        private Vector3 _initialRotation;

        private void Awake()
        {
            // 初期回転値を取得
            _initialRotation = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            // Directional Lightを回転
            transform.Rotate(_initialRotation * Time.deltaTime * rotateSpeed);
        }
    }
}
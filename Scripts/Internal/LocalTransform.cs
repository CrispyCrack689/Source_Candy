using Sirenix.OdinInspector;
using UnityEngine;

namespace _00_GameData.Scripts.Internal
{
    [ExecuteInEditMode]
    public class LocalTransform : MonoBehaviour
    {
        [ReadOnly, Tooltip("ローカル座標")]
        public Vector3 localPosition;
        [ReadOnly, Tooltip("ローカル回転")] 
        public Vector3 localRotation;
        [ReadOnly, Tooltip("ローカルスケール")] 
        public Vector3 localScale;

        private void Update()
        {
            localPosition = transform.localPosition;
            localRotation = transform.localEulerAngles;
            localScale = transform.localScale;
        }
    }
}

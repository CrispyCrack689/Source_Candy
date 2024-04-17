using System;
using UnityEngine;

namespace _00_GameData.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerFootIK : MonoBehaviour
    {
        [SerializeField]
        private float maxDistance = 1.5f;

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            // 両足を地面に固定
            // 左足
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, GetLeftFootPosition());
            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, GetLeftFootRotation());
            // 右足
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, GetRightFootPosition());
            _animator.SetIKRotation(AvatarIKGoal.RightFoot, GetRightFootRotation());
        }

        private Vector3 GetLeftFootPosition()
        {
            RaycastHit hit;
            Vector3 leftFootPosition = _animator.GetIKPosition(AvatarIKGoal.LeftFoot);
            if (Physics.Raycast(leftFootPosition + Vector3.up, Vector3.down, out hit, maxDistance))
            {
                leftFootPosition.y = hit.point.y;
            }
            return leftFootPosition;
        }

        private Quaternion GetLeftFootRotation()
        {
            RaycastHit hit;
            Quaternion leftFootRotation = _animator.GetIKRotation(AvatarIKGoal.LeftFoot);
            if (Physics.Raycast(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down, out hit, maxDistance))
            {
                leftFootRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            }
            return leftFootRotation;
        }

        private Vector3 GetRightFootPosition()
        {
            RaycastHit hit;
            Vector3 rightFootPosition = _animator.GetIKPosition(AvatarIKGoal.RightFoot);
            if (Physics.Raycast(rightFootPosition + Vector3.up, Vector3.down, out hit, maxDistance))
            {
                rightFootPosition.y = hit.point.y;
            }
            return rightFootPosition;
        }

        private Quaternion GetRightFootRotation()
        {
            RaycastHit hit;
            Quaternion rightFootRotation = _animator.GetIKRotation(AvatarIKGoal.RightFoot);
            if (Physics.Raycast(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down, out hit, maxDistance))
            {
                rightFootRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            }
            return rightFootRotation;
        }
        
        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawRay(GetLeftFootPosition() + Vector3.up, Vector3.down * maxDistance);
        //     Gizmos.DrawRay(GetRightFootPosition() + Vector3.up, Vector3.down * maxDistance);
        // }
    }
}
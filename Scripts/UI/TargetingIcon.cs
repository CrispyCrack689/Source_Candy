using System;
using DG.Tweening;
using UnityEngine;

namespace _00_GameData.Scripts.UI
{
    public class TargetingIcon : MonoBehaviour
    {
        private void Start()
        {
            // 上下に揺らす
            transform.GetChild(0).transform.DOLocalMoveY(0.1f, 0.5f)
                .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        private void Update()
        {
            // カメラの方向を向く
            Vector3 lookPos = Camera.main.transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }
}
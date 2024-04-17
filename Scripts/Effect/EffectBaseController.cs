using System;
using _00_GameData.Scripts.Internal;
using UnityEngine;

namespace _00_GameData.Scripts.Effect
{
    public class EffectBaseController : MonoBehaviour
    {
        public virtual void OnTriggerEnter(Collider effectCollider)
        {
            switch (effectCollider.gameObject.layer)
            {
                // 壁に当たったら
                case LayerName.Wall:
                    DebugLogger.Log("Hit base effect to wall.", this);
                    break;
                // 床に当たったら
                case LayerName.Ground:
                    DebugLogger.Log("Hit base effect to ground.", this);
                    break;
            }
        }
    }
}

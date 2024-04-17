using _00_GameData.Scripts.Internal;
using UnityEngine;

namespace _00_GameData.Scripts.Effect
{
    public class EffectFireController : EffectBaseController
    {
        public override void OnTriggerEnter(Collider effectCollider)
        {
            switch (effectCollider.gameObject.layer)
            {
                // 壁に当たったら
                case LayerName.Wall:
                    DebugLogger.Log("Hit fire effect to wall.", this);
                    break;
                // 床に当たったら
                case LayerName.Ground:
                    DebugLogger.Log("Hit fire effect to ground.", this);
                    break;
            }
        }
    }
}

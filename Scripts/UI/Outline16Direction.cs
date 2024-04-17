using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace _00_GameData.Scripts.UI
{
    public class Outline16Direction : Outline
    {
        /* 16方向に画像アウトラインを描画 */

        protected Outline16Direction() { }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) return;

            base.ModifyMesh(vh);

            List<UIVertex> vert = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(vert);

            var outline8VertexCount = vert.Count;
            var baseCount = outline8VertexCount / 9;
            var neededCapacity = baseCount * 17; // 本描画 + 16方向 の頂点数
            if (vert.Capacity < neededCapacity) vert.Capacity = neededCapacity;

            // ずらす回転角度
            var rot = 90f / 4f;
            var rotVectorA = Quaternion.AngleAxis(rot, Vector3.forward) * effectDistance;

            var start = outline8VertexCount - baseCount;
            var end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, rotVectorA.x, rotVectorA.y);

            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, rotVectorA.x, -rotVectorA.y);

            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, -rotVectorA.x, rotVectorA.y);

            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, -rotVectorA.x, -rotVectorA.y);

            // 反対側
            var rotVectorB = Quaternion.AngleAxis(-rot, Vector3.forward) * effectDistance;
            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, rotVectorB.x, rotVectorB.y);

            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, rotVectorB.x, -rotVectorB.y);

            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, -rotVectorB.x, rotVectorB.y);

            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, -rotVectorB.x, -rotVectorB.y);


            vh.Clear();
            vh.AddUIVertexTriangleStream(vert);
            ListPool<UIVertex>.Release(vert);
        }
    }
}
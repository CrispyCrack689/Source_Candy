using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace _00_GameData.Scripts.UI
{
    public class Outline8Direction : Outline
    {
        /* 8方向に画像アウトラインを描画 */

        protected Outline8Direction() { }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) return;

            // 標準コンポーネントで処理
            base.ModifyMesh(vh);

            List<UIVertex> vert = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(vert);

            var outline4VertexCount = vert.Count;
            var baseCount = outline4VertexCount / 5; // 中心点を除いた頂点数

            var neededCapacity = baseCount * 9; // 本描画 + 8方向 の頂点数
            if (vert.Capacity < neededCapacity) vert.Capacity = neededCapacity;

            // ずらす幅
            var length = effectDistance.magnitude;

            // 上方向
            var start = outline4VertexCount - baseCount;
            var end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, 0f, length);

            // 右方向
            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, length, 0f);

            // 下方向
            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, 0f, -length);

            // 左方向
            start = end;
            end = vert.Count;
            ApplyShadowZeroAlloc(vert, effectColor, start, end, -length, 0f);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vert);
            ListPool<UIVertex>.Release(vert);
        }
    }
}
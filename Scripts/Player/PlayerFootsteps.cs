using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Systems;
using AnnulusGames.LucidTools.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
[RequireComponent(typeof(Animator))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Tooltip("地面との距離")]
    private float offsetY = 0.1f;

    [Header("Sounds")]
    [SerializeField, Tooltip("移動音(通常)")]
    private AudioClip[] footstepsNormal;
    [SerializeField, Tooltip("移動音(草地)")]
    private AudioClip[] footstepsGrass;

    private int _randomValue;

    //TODO:TerrainのAlphaMapを取得して、地面の種類を判定する

    /// <summary>
    /// 地面のTag名を判定する
    /// </summary>
    /// <param name="origin">判定レイ原点</param>
    /// <returns>地面のTag名</returns>
    private static string DetermineGroundTag(Vector3 origin)
    {
        var ray = new Ray(origin, Vector3.down);
        Debug.DrawRay(origin, Vector3.down * 0.5f, Color.red, 1.0f);
        if (Physics.Raycast(ray, out var hit, 1.0f))
        {
            return hit.collider.tag;
        }

        return null;
    }

    /// <summary>
    /// フットステップを再生する
    /// </summary>
    private void PlayFootsteps()
    {
        // 地面のTagによってSEを変える
        // Utils.Load();
        var position = transform.position;
        var rayOrigin = new Vector3(position.x, position.y + offsetY, position.z);
        switch (DetermineGroundTag(rayOrigin))
        {
            case TagName.GrGrass:
                _randomValue = Random.Range(0, footstepsGrass.Length);
                LucidAudio.PlaySE(footstepsGrass[_randomValue])
                    .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster * 0.25f);
                break;

            default:
                _randomValue = Random.Range(0, footstepsNormal.Length);
                LucidAudio.PlaySE(footstepsNormal[_randomValue])
                    .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster * 0.25f);
                break;
        }
    }
}
using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Systems;
using _00_GameData.Scripts.Weapon;
using AnnulusGames.LucidTools.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
[RequireComponent(typeof(Animator))]
public class WeaponAnimEvent : MonoBehaviour
{
    [Header("Components")]
    public WeaponController weaponController;
    public Collider weaponCollider;

    [Header("Holders")]
    [SerializeField, Tooltip("背負う位置")]
    private Transform backHolder;
    [SerializeField, Tooltip("手に持つ位置")]
    private Transform handHolder;

    [Header("Sounds")]
    [SerializeField, Tooltip("抜刀音")]
    private AudioClip[] drawSound;
    [SerializeField, Tooltip("納刀音")]
    private AudioClip[] sheatheSound;

    /// <summary>
    /// 武器を手に持つ
    /// </summary>
    private void WeaponHold()
    {
        // 手に持つ位置に移動
        var mesh = weaponController.meshRenderer.transform;
        mesh.parent = handHolder.parent;
        mesh.position = handHolder.position;
        mesh.rotation = handHolder.rotation;

        // SEを再生
        // if(GameManager.CurrentGamePhase != GameManager.GamePhase.Attack) return;
        // var random = Random.Range(0,drawSound.Length);
        // LucidAudio.PlaySE(drawSound[random]);
    }

    /// <summary>
    /// 武器を背負う
    /// </summary>
    private void WeaponRelease()
    {
        // 背負う位置に移動
        var mesh = weaponController.meshRenderer.transform;
        mesh.parent = backHolder.parent;
        mesh.position = backHolder.position;
        mesh.rotation = backHolder.rotation;

        // SEを再生
        var random = Random.Range(0, sheatheSound.Length);
        LucidAudio.PlaySE(sheatheSound[random])
            .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster);
    }

    /// <summary>
    /// 空振り音を再生
    /// </summary>
    private void PlayMissSound()
    {
        // ヒットフラグが立っていたら処理しない
        if (weaponController.isHit) return;

        // SEを再生
        // Utils.Load();
        var random = Random.Range(0, weaponController.attackMissSounds.Length);
        LucidAudio.PlaySE(weaponController.attackMissSounds[random])
            .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster);
    }

    /// <summary>
    /// 武器の攻撃を有効化
    /// </summary>
    private void EnableWeaponAttack()
    {
        weaponCollider.enabled = true;
    }

    /// <summary>
    /// 武器の攻撃を無効化
    /// </summary>
    private void DisableWeaponAttack()
    {
        weaponCollider.enabled = false;

        // ヒットフラグをリセット
        weaponController.isHit = false;
    }
}
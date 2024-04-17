using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Systems;
using AnnulusGames.LucidTools.Audio;
using UnityEngine;

// ReSharper disable once CheckNamespace
[RequireComponent(typeof(Animator))]
public class PlayerAnimEvent : MonoBehaviour
{
    [Header("Components")]
    [SerializeField, Tooltip("プレイヤーのRigidbody")]
    private Rigidbody playerRigidbody;

    [Header("Parameters")]
    [SerializeField, Tooltip("弱攻撃時の前進力")]
    private float lightForwardPower = 7.0f;
    [SerializeField, Tooltip("強攻撃時の前進力")]
    private float heavyForwardPower = 8.5f;
    [SerializeField, Tooltip("空中攻撃時の前進力")]
    private float airForwardPower = 5.0f;
    [SerializeField, Tooltip("回避時の前進力(待機中)")]
    private float dodgeForwardIdle = 15.0f;
    [SerializeField, Tooltip("回避地の前進力(移動中)")]
    private float dodgeForwardMoving = 10.0f;

    [Header("Sounds")]
    [SerializeField, Tooltip("プレイヤー回避音")]
    private AudioClip[] dodgeSounds;

    /* Rigidbody */
    private const float _DODGE_VELOCITY_THRESHOLD = 3.5f;

    /// <summary>
    /// 弱攻撃時に前進する
    /// </summary>
    private void AttackLightForward()
    {
        playerRigidbody.AddForce(transform.forward * lightForwardPower, ForceMode.Impulse);
    }

    /// <summary>
    /// 強攻撃時に前進する
    /// </summary>
    private void AttackHeavyForward()
    {
        playerRigidbody.AddForce(transform.forward * heavyForwardPower, ForceMode.Impulse);
    }

    /// <summary>
    /// 空中攻撃時に前進する
    /// </summary>
    private void AttackAirForward()
    {
        playerRigidbody.AddForce(transform.forward * airForwardPower, ForceMode.Impulse);
    }

    /// <summary>
    /// 回避時に前進する
    /// </summary>
    private void DodgeForward()
    {
        // 待機中と移動中で前進力を変える
        // Utils.Load();
        if (playerRigidbody.velocity.magnitude < _DODGE_VELOCITY_THRESHOLD)
        {
            // 待機中
            playerRigidbody.AddForce(transform.forward * dodgeForwardIdle, ForceMode.Impulse);
            // 回避SEを再生
            var random = Random.Range(0, dodgeSounds.Length);
            LucidAudio.PlaySE(dodgeSounds[random])
                .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster);
        }
        else
        {
            // 移動中
            playerRigidbody.AddForce(transform.forward * dodgeForwardMoving, ForceMode.Impulse);
            // 回避SEを再生
            var random = Random.Range(0, dodgeSounds.Length);
            LucidAudio.PlaySE(dodgeSounds[random])
                .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster);
        }
    }
}
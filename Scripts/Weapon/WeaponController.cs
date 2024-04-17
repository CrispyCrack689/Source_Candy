using System.Collections.Generic;
using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Player;
using _00_GameData.Scripts.Systems;
using AnnulusGames.LucidTools.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _00_GameData.Scripts.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Components")]
        [Tooltip("エフェクト位置ソース")]
        public Transform effectPosition;

        [Header("Meshes")]
        [SerializeField, Tooltip("武器メッシュ:火属性")]
        private Mesh fireWeaponMesh;
        [SerializeField, Tooltip("武器メッシュ:水属性")]
        private Mesh waterWeaponMesh;
        [SerializeField, Tooltip("武器メッシュ:雷属性")]
        private Mesh sparkWeaponMesh;

        [Header("Sounds")]
        [Tooltip("攻撃ヒット音")]
        public AudioClip[] attackHitSounds;
        [Tooltip("攻撃空振り音")]
        public AudioClip[] attackMissSounds;

        [Header("Debug")]
        [ReadOnly, Tooltip("攻撃ヒットフラグ")]
        public bool isHit;
        [ReadOnly, Tooltip("攻撃の接触地点")]
        public Vector3 hitContactPoint;

        [HideInInspector]
        public MeshFilter meshFilter;
        [HideInInspector]
        public MeshRenderer meshRenderer;

        private PoolManager _poolManager;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            _poolManager = GameObject.FindGameObjectWithTag(TagName.GameController).GetComponent<PoolManager>();
        }

        private void OnTriggerEnter(Collider weaponCollider)
        {
            // 当たった相手が敵かどうか判定
            if (!weaponCollider.CompareTag(TagName.Enemy)) return;
            DebugLogger.Log($"Hit {weaponCollider.gameObject.name}", this);

            // コンタクトポイントを取得
            hitContactPoint = weaponCollider.ClosestPointOnBounds(transform.position);
            // 攻撃エフェクトを出現
            _poolManager.InstantiateAttackEffect(hitContactPoint);
            // ヒットサウンドを再生
            PlaySoundEffect(attackHitSounds);
            //TODO:対象にダメージを与える
            // ヒットフラグを立てる
            isHit = true;
        }

        /// <summary>
        /// 武器を変更する
        /// </summary>
        /// <param name="element">プレイヤーの属性</param>
        public void ChangeWeapon(PlayerController.PlayerElement element)
        {
            // メッシュを変更
            switch (element)
            {
                case PlayerController.PlayerElement.Fire:
                    meshFilter.mesh = fireWeaponMesh;
                    break;

                case PlayerController.PlayerElement.Water:
                    meshFilter.mesh = waterWeaponMesh;
                    break;

                case PlayerController.PlayerElement.Spark:
                    meshFilter.mesh = sparkWeaponMesh;
                    break;

                default:
                    DebugLogger.LogError("Invalid weapon element!", this);
                    break;
            }

            // エフェクトを生成
            _poolManager.InstantiateWeaponChangeEffect(effectPosition.position);
            //TODO:SEを再生
        }

        /// <summary>
        /// ランダムにサウンドを再生する
        /// </summary>
        /// <param name="clips">効果音</param>
        private void PlaySoundEffect(IReadOnlyList<AudioClip> clips)
        {
            var random = Random.Range(0, clips.Count);
            LucidAudio.PlaySE(clips[random])
                .SetVolume(Utils.saveData.gameVolumeSfx * Utils.saveData.gameVolumeMaster);
        }

        private void OnDrawGizmos()
        {
            // コンタクトポイントを表示
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hitContactPoint, 0.1f);
        }
    }
}
using System;
using System.Threading;
using _00_GameData.Scripts.Internal;
using _00_GameData.Scripts.Player;
using CartoonFX;
using Cysharp.Threading.Tasks;
using UnityEngine;
using uPools;
using Random = UnityEngine.Random;

namespace _00_GameData.Scripts.Systems
{
    public class PoolManager : MonoBehaviour
    {
        [Header("Please disable CFX scripts")]
        [Header("Parameters")]
        [SerializeField, Tooltip("エフェクト消滅までの遅延時間")]
        private float disposeTime = 2f;

        [Header("Effects")]
        [SerializeField, Tooltip("武器切り替えエフェクト")]
        private GameObject weaponChangeEffect;
        [SerializeField, Tooltip("攻撃エフェクト")]
        private GameObject attackEffect;
        [SerializeField, Tooltip("回復エフェクト")]
        private GameObject healEffect;
        [SerializeField, Tooltip("火属性魔法エフェクト")]
        private GameObject fireEffect;
        [SerializeField, Tooltip("スライム")]
        private GameObject suraimu;
        [SerializeField, Tooltip("カメ")]
        private GameObject gingerman;
        [SerializeField, Tooltip("イヌ")]
        private GameObject dog;

        private const int _PREWARM_AMOUNT = 3;
        private const int _ENEMY_AMOUNT = 10;
        private CancellationToken _token;

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();

            // プールを事前生成
            SharedGameObjectPool.Prewarm(weaponChangeEffect, _PREWARM_AMOUNT);
            SharedGameObjectPool.Prewarm(attackEffect, _PREWARM_AMOUNT);
            SharedGameObjectPool.Prewarm(healEffect, _PREWARM_AMOUNT);
            SharedGameObjectPool.Prewarm(fireEffect, _PREWARM_AMOUNT);
            SharedGameObjectPool.Prewarm(suraimu, _ENEMY_AMOUNT);
            SharedGameObjectPool.Prewarm(gingerman, _ENEMY_AMOUNT);
            SharedGameObjectPool.Prewarm(dog, _ENEMY_AMOUNT);
        }

        /// <summary>
        /// 武器切り替えエフェクトを生成
        /// </summary>
        /// <param name="position">生成位置</param>
        public async void InstantiateWeaponChangeEffect(Vector3 position)
        {
            try
            {
                // エフェクトを生成
                var instance = SharedGameObjectPool.Rent(weaponChangeEffect, position, Quaternion.identity);

                // 一定時間後にエフェクトを返却
                await UniTask.Delay(TimeSpan.FromSeconds(disposeTime), cancellationToken: _token);
                SharedGameObjectPool.Return(instance);
            } catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("InstantiateWeaponChangeEffect() was cancelled.", this);
            }
        }

        /// <summary>
        /// 攻撃エフェクトを生成
        /// </summary>
        /// <param name="position">生成位置</param>
        public async void InstantiateAttackEffect(Vector3 position)
        {
            try
            {
                // エフェクトを生成
                var instance = SharedGameObjectPool.Rent(attackEffect, position, Quaternion.identity);

                // 一定時間後にエフェクトを返却
                await UniTask.Delay(TimeSpan.FromSeconds(disposeTime), cancellationToken: _token);
                SharedGameObjectPool.Return(instance);
            } catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("InstantiateAttackEffect() was cancelled.", this);
            }
        }

        /// <summary>
        /// 回復エフェクトを生成
        /// </summary>
        /// <param name="position">生成位置</param>
        public async void InstantiateHealEffect(Vector3 position)
        {
            try
            {
                // エフェクトを生成
                var instance = SharedGameObjectPool.Rent(healEffect, position, Quaternion.identity);

                // 一定時間後にエフェクトを返却
                await UniTask.Delay(TimeSpan.FromSeconds(disposeTime), cancellationToken: _token);
                SharedGameObjectPool.Return(instance);
            } catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("InstantiateHealEffect() was cancelled.", this);
            }
        }

        /// <summary>
        /// 火属性魔法エフェクトを生成
        /// </summary>
        /// <param name="transform">生成Transform</param>
        public async void InstantiateFireEffect(Transform transform)
        {
            //TODO:GetComponent()乱立でやばい
            try
            {
                // エフェクトを生成
                var instance = SharedGameObjectPool.Rent(fireEffect, transform.position, Quaternion.identity);
                // プレイヤーの前方向に飛ばす
                instance.GetComponent<Rigidbody>().AddForce(transform.forward * 15f, ForceMode.Impulse);
                // 消滅時間を設定
                var main = instance.GetComponent<ParticleSystem>().main;
                main.startLifetime = disposeTime;
                var sub = instance.transform.GetChild(0).GetComponent<ParticleSystem>().main;
                sub.startLifetime = disposeTime;

                // 一定時間待つ
                await UniTask.Delay(TimeSpan.FromSeconds(disposeTime), cancellationToken: _token);
                // 慣性をリセットする
                instance.GetComponent<Rigidbody>().velocity = Vector3.zero;

                // 一定時間待つ
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: _token);
                // エフェクトを返却
                SharedGameObjectPool.Return(instance);
            } catch (OperationCanceledException)
            {
                // エラー処理
                DebugLogger.LogWarning("InstantiateFireEffect() was cancelled.", this);
            }
        }

        /// <summary>
        /// スライム生成
        /// </summary>
        /// <param name="amount">生成個体数</param>
        /// <param name="position">生成箇所</param>
        /// <param name="density">スポーン間隔</param>
        public void InstantiateSuraimu(int amount, Vector3 position, float density)
        {
            for ( int i = 0; i < amount; i++ )
            {
                var player = GameObject.FindGameObjectWithTag(TagName.Player);
                var instance = SharedGameObjectPool.Rent(suraimu);
                instance.transform.position = new Vector3(position.x + Random.Range(-density, density), position.y, position.z + Random.Range(-density, density));
                instance.GetComponent<EnemySuraimu>().HP = EnemySuraimu._HP;
                instance.GetComponent<EnemyBase>().playerController = player.GetComponent<PlayerController>();
                instance.GetComponent<EnemyBase>().target = player.transform;
            }
        }
        public void InstantiateGingerman(int amount, Vector3 position, float density)
        {
            for ( int i = 0; i < amount; i++ )
            {
                var player = GameObject.FindGameObjectWithTag(TagName.Player);
                var instance = SharedGameObjectPool.Rent(gingerman);
                instance.transform.position = new Vector3(position.x + Random.Range(-density, density), position.y, position.z + Random.Range(-density, density));
                instance.GetComponent<EnemyGingerMan>().HP = EnemyGingerMan._HP;
                instance.GetComponent<EnemyBase>().playerController = player.GetComponent<PlayerController>();
                instance.GetComponent<EnemyBase>().target = player.transform;
            }
        }
        public void InstantiateHumanoid(int amount, Vector3 position, float density)
        {
            for ( int i = 0; i < amount; i++ )
            {
                var player = GameObject.FindGameObjectWithTag(TagName.Player);
                var instance = SharedGameObjectPool.Rent(dog);
                instance.transform.position = new Vector3(position.x + Random.Range(-density, density), position.y, position.z + Random.Range(-density, density));
                instance.GetComponent<HumanoidEnemy>().HP = EnemyGingerMan._HP;
                instance.GetComponent<EnemyBase>().playerController = player.GetComponent<PlayerController>();
                instance.GetComponent<EnemyBase>().target = player.transform;
            }
        }
        public void ReturnEnemy(GameObject _gameobject)
        {
            SharedGameObjectPool.Return(_gameobject);
        }
    }
}
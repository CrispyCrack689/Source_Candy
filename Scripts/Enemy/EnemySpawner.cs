using _00_GameData.Scripts.Systems;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    PoolManager _poolManager;
    public static int enemyCount = 0;
    const int MAXENEMYAMOUNT = 10;

    public enum spawnEnemy
    {
        slime,
        turtle,
        // dog
    }

    private spawnEnemy enemyType;
    [SerializeField, Tooltip("スポーン場所")]
    private Transform[] spawnPoints;
    void Start()
    {
        _poolManager = GameObject.FindGameObjectWithTag(TagName.GameController).GetComponent<PoolManager>();
        enemyCount = 0;

        /*
        var spawnAmount = Random.Range(0, 4);                              // 出現数
        var point = Random.Range(0, spawnPoints.Length);                   //  出現位置
        var maxCount = Enum.GetNames(typeof(spawnEnemy)).Length;           //  列挙の数の取得
        var number = Random.Range(0, maxCount);                            //  ランダムなナンバーの取得
        enemyType = (spawnEnemy)Enum.ToObject(typeof(spawnEnemy), number); //  数字を列挙型に変換
        switch (enemyType)
        {
            case spawnEnemy.slime:
                _poolManager.InstantiateSuraimu(spawnAmount, spawnPoints[point].position, 4);
                break;
            case spawnEnemy.turtle:
                _poolManager.InstantiateGingerman(spawnAmount, spawnPoints[point].position, 4);
                break;
            case spawnEnemy.dog:
                _poolManager.InstantiateHumanoid(spawnAmount, spawnPoints[point].position, 4);
                break;
        }
        */
    }

    void Update()
    {
        print(enemyCount);
        if (spawnPoints == null) return;
        if (enemyCount <= MAXENEMYAMOUNT)
        {
            var spawnAmount = Random.Range(0, 4);    // 出現数
            var point = Random.Range(0, spawnPoints.Length); //  出現位置
            var maxCount = Enum.GetNames(typeof(spawnEnemy)).Length;    //  列挙の数の取得
            var number = Random.Range(0, maxCount);  //  ランダムなナンバーの取得
            enemyType = (spawnEnemy)Enum.ToObject(typeof(spawnEnemy), number);   //  数字を列挙型に変換
            switch (enemyType)
            {
                case spawnEnemy.slime:
                    _poolManager.InstantiateSuraimu(spawnAmount, spawnPoints[point].position, 4);
                    enemyCount += spawnAmount;
                    break;
                case spawnEnemy.turtle:
                    _poolManager.InstantiateGingerman(spawnAmount, spawnPoints[point].position, 4);
                    enemyCount += spawnAmount;
                    break;
                // case spawnEnemy.dog:
                //     _poolManager.InstantiateHumanoid(spawnAmount, spawnPoints[point].position, 4);
                //     enemyCount += spawnAmount;
                //     break;
            }
        }
    }
}
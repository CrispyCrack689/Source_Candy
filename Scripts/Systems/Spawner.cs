using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private enum ItemName
    {
        Item1,
        Item2,
        Item3
    }
    [SerializeField, Tooltip("アイテム種類")]
    private ItemName itemName;
    [SerializeField, Tooltip("湧き位置")]
    private Transform[] generatePoint;
    [SerializeField, Tooltip("個数制限")]
    private int limitCount;
    [SerializeField, Tooltip("ディレイタイム")]
    private float dlayTime;
    private int itemCount;
    private int length;
    private void Start()
    {
        length = generatePoint.Length;
    }

    /// <summary>
    /// アイテムを生成
    /// </summary>
    /// <param name="Delay">遅延時間</param>
    /// <returns></returns>
    private async void CandyGenerate(int Delay)
    {
        for (; itemCount < limitCount; itemCount++)
        {
            GameObject candyPrefab = (GameObject)Resources.Load<GameObject>(itemName.ToString());

            Transform randomPoint = generatePoint[Random.Range(0, length)];
            Instantiate(candyPrefab, randomPoint.position, Quaternion.identity);
            await Task.Delay(Delay);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (itemCount < limitCount)
            {
                CandyGenerate((int)(dlayTime * 1000));
            }
        }
        print(itemCount);

        // private void OnTriggerEnter(Collider other)
        // {
        //     //  プレイヤーかつitemCount<limitItemならという条件式
        //     {
        //         CandyGenerate((int)(dlayTime * 1000));
        //     }
        // }
    }
}

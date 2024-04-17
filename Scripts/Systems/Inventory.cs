using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    static List<Tuple<string, int>> Items = new List<Tuple<string, int>>
    {
        new Tuple<string, int>("アイテム1",0),
        new Tuple<string, int>("アイテム2",0),
        new Tuple<string, int>("アイテム3",0)
    };

    /// <summary>
    /// アイテムの個数更新
    /// </summary>
    /// <param name="itemName">更新したいアイテム名</param>
    /// <param name="newCount">更新後の個数</param>
    static public void UpdateItem(string itemName, int newCount)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Item1 == itemName)
            {
                Items[i] = Tuple.Create(itemName, Items[i].Item2 + newCount);
                break;
            }
        }
    }

    /// <summary>
    /// アイテムの数を取得
    /// </summary>
    /// <param name="itemName">取得したいアイテムの名前</param>
    /// <returns>個数、検索できなかった時は-1</returns>
    static public int GetItemCount(string itemName)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Item1 == itemName)
            {
                return Items[i].Item2;
            }
        }
        return -1;
    }

    /// <summary>
    /// アイテム使用
    /// </summary>
    /// <param name="itemName">使用したアイテム</param>
    /// <param name="itemCount">使用した個数</param>
    static public void UseItem(string itemName, int itemCount)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Item1 == itemName)
            {
                Items[i] = Tuple.Create(itemName, Items[i].Item2 - itemCount);
                break;
            }
        }
    }
}

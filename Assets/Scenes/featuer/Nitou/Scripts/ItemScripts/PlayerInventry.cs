using System.Collections.Generic;
using UnityEngine;

public class PlayerInventry : MonoBehaviour
{
    [Header("所持アイテム(最大4つ)")]
    public List<ItemData> items = new List<ItemData>();

    [Header("生成位置")]
    public Transform[] itemSlots;

    [Header("生成したアイテムオブジェクト")]
    public List<GameObject> spawnItems = new List<GameObject>();

    [Header("アイテムの所持可能数")]
    public int maxItems;

    //インベントリーにアイテムを追加
    public void AddItems(ItemData item)
    {
        //5つ目以降のアイテムは捨てる
        if (items.Count > maxItems)
        {
            Debug.Log($"新しいアイテム{item.name}は破棄されます");
            return;
        }

        //アイテムの追加
        items.Add(item);

        //追加したアイテムオブジェクトの生成
        SpawnItemObject();
    }

    //アイテムオブジェクトの生成処理
    private void SpawnItemObject()
    {
        //入手したアイテムの番号
        int index = items.Count - 1;

        if (index < itemSlots.Length)
        {
            ItemData itemData = items[index];
            if (itemData.spawnObj != null)
            {
                //指定の位置にアイテムオブジェクトを生成する
                GameObject obj = Instantiate(itemData.spawnObj,
                                             itemSlots[index].position,
                                             itemSlots[index].rotation);
                //整理のために子オブジェクトにする
                obj.transform.SetParent(itemSlots[index]);
                //生成したアイテムオブジェクトのListに加える
                spawnItems.Add(obj);
            }
        }
    }

    //ターン経過時用にアイテムの表示・非表示を切り替える
    public void SetActiveObjects(bool isActive)
    {
        foreach(GameObject itemObj in spawnItems)
        {
            if (itemObj != null)
            {
                itemObj.SetActive(isActive);
            }
        }
    }
}

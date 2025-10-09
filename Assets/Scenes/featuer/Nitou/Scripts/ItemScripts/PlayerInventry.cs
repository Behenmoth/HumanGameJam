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

    [Header("このインベントリーが対応するプレイヤー")]
    public GameManager.PlayerTurn ownerTurn;

    //インベントリーにアイテムを追加
    public void AddItems(ItemData item)
    {
        //5つ目以降のアイテムは捨てる
        if (items.Count > maxItems)
        {
            Debug.Log($"新しいアイテム{item.name}は破棄されます");
            RemoveNewestItem();
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
                //アイテムの角度情報
                Quaternion rotation = Quaternion.Euler(itemData.spawnRotation);

                //指定の位置にアイテムオブジェクトを生成する
                GameObject obj = Instantiate(itemData.spawnObj,
                                             itemSlots[index].position,
                                             rotation);
                //整理のために子オブジェクトにする
                obj.transform.SetParent(itemSlots[index]);
                //生成したアイテムオブジェクトのListに加える
                spawnItems.Add(obj);
            }
        }
    }

    // 最新のアイテムを削除する処理
    private void RemoveNewestItem()
    {
        if (items.Count > 0)
        {
            // 最新のアイテムを取得
            int lastIndex = items.Count - 1;

            // 対応するオブジェクトを削除
            if (lastIndex < spawnItems.Count && spawnItems[lastIndex] != null)
            {
                Destroy(spawnItems[lastIndex]);
                spawnItems.RemoveAt(lastIndex);
            }

            // リストから削除
            items.RemoveAt(lastIndex);
        }
    }

    //ターン経過時用にアイテムの表示・非表示を切り替える処理
    public void SetActiveObjects(GameManager.PlayerTurn currentPlayerTurn)
    {
        //対応するターンと現在のターンが一致したらtrueを返す
        bool isActive = (currentPlayerTurn == ownerTurn);

        foreach(GameObject itemObj in spawnItems)
        {
            if (itemObj != null)
            {
                itemObj.SetActive(isActive);
            }
        }
        Debug.Log("アイテムを入れ替えた");
    }
}

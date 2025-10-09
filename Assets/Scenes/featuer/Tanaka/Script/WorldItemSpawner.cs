using UnityEngine;
using System.Collections.Generic;

public class WorldItemSpawner : MonoBehaviour
{
    [Header("3Dオブジェクトの生成位置（空オブジェクトを指定）")]
    public Transform[] spawnPoints;

    private Dictionary<Transform, GameObject> spawnedObjects = new Dictionary<Transform, GameObject>();

    /// <summary>
    /// 指定されたItemListのItemObjectを生成
    /// </summary>
    public GameObject Spawn(ItemList item)
    {
        if (item == null || item.ItemObject == null)
        {
            Debug.LogWarning($"Spawn失敗: ItemObjectが未設定 ({item?.ItemName})");
            return null;
        }

        Transform spawn = GetAvailableSpawnPoint();
        if (spawn == null)
        {
            Debug.LogWarning($"全てのスポーンポイントが埋まっています。({item.ItemName})");
            return null;
        }

        GameObject obj = Instantiate(item.ItemObject, spawn.position, Quaternion.identity);
        obj.name = $"World_{item.ItemName}";
        spawnedObjects.Add(spawn, obj);
        return obj;
    }

    /// <summary>
    /// 現在の生成物をすべて削除
    /// </summary>
    public void ClearAll()
    {
        foreach (var kv in spawnedObjects)
        {
            if (kv.Value != null)
                Destroy(kv.Value);
        }
        spawnedObjects.Clear();
    }

    private Transform GetAvailableSpawnPoint()
    {
        foreach (var point in spawnPoints)
        {
            if (!spawnedObjects.ContainsKey(point))
                return point;
        }
        return null;
    }

    /// <summary>
    /// 対応するアイテムオブジェクトを削除
    /// </summary>
    public void RemoveItem(ItemList item)
    {
        if (item == null)
            return;

        // 名前に ItemName が含まれている生成物を探す
        Transform targetKey = null;

        foreach (var kv in spawnedObjects)
        {
            GameObject obj = kv.Value;
            if (obj != null && obj.name.Contains(item.ItemName))
            {
                Destroy(obj);
                targetKey = kv.Key;
                break;
            }
        }

        if (targetKey != null)
            spawnedObjects.Remove(targetKey);
    }

}

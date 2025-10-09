using UnityEngine;
using System.Collections.Generic;

public class WorldItemSpawner : MonoBehaviour
{
    [Header("3Dオブジェクトの生成位置（空オブジェクトを指定）")]
    public Transform[] spawnPoints;

    private Dictionary<Transform, GameObject> spawnedObjects = new Dictionary<Transform, GameObject>();
    private int lastSpawnIndex = -1; // 最後に使用したインデックス

    public GameObject Spawn(ItemList item)
    {
        if (item == null || item.ItemObject == null)
        {
            Debug.LogWarning($"Spawn失敗: ItemObjectが未設定 ({item?.ItemName})");
            return null;
        }

        Transform spawn = GetNextAvailableSpawnPoint();
        if (spawn == null)
        {
            Debug.LogWarning($"全てのスポーンポイントが埋まっています。({item.ItemName})");
            return null;
        }

        GameObject obj = Instantiate(item.ItemObject, spawn.position, Quaternion.identity);
        obj.name = $"World_{item.ItemName}";
        spawnedObjects[spawn] = obj;

        return obj;
    }

    /// <summary>
    /// 空いているスポーンポイントを順番に探す
    /// </summary>
    private Transform GetNextAvailableSpawnPoint()
    {
        int startIndex = (lastSpawnIndex + 1) % spawnPoints.Length;

        // spawnPointsを順にチェックして、空いている場所を探す
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int index = (startIndex + i) % spawnPoints.Length;
            Transform point = spawnPoints[index];

            if (!spawnedObjects.ContainsKey(point) || spawnedObjects[point] == null)
            {
                lastSpawnIndex = index;
                return point;
            }
        }

        return null; // 全て埋まっている
    }

    /// <summary>
    /// 対応するアイテムオブジェクトを削除
    /// </summary>
    public void RemoveItem(ItemList item)
    {
        if (item == null)
            return;

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

    public void ClearAll()
    {
        foreach (var kv in spawnedObjects)
        {
            if (kv.Value != null)
                Destroy(kv.Value);
        }
        spawnedObjects.Clear();
        lastSpawnIndex = -1; // リセット
    }
}

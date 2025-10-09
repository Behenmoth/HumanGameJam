using UnityEngine;
using System.Collections.Generic;

public class WorldItemSpawner : MonoBehaviour
{
    [Header("3Dオブジェクトの生成位置（空オブジェクトを指定）")]
    // アイテムを出現させる座標（空オブジェクトなど）を複数指定
    public Transform[] spawnPoints;

    // 各スポーンポイントと生成済みオブジェクトの対応表（辞書）
    private Dictionary<Transform, GameObject> spawnedObjects = new Dictionary<Transform, GameObject>();

    // 最後に使ったスポーンポイントのインデックス（順番に使うために記録）
    private int lastSpawnIndex = -1;

    // ===============================
    // ✅ アイテムをスポーン（生成）する
    // ===============================
    public GameObject Spawn(ItemList item)
    {
        // --- ① 引数チェック（null対策） ---
        if (item == null || item.ItemObject == null)
        {
            Debug.LogWarning($"Spawn失敗: ItemObjectが未設定 ({item?.ItemName})");
            return null;
        }

        // --- ② 空いているスポーンポイントを取得 ---
        Transform spawn = GetNextAvailableSpawnPoint();
        if (spawn == null)
        {
            Debug.LogWarning($"全てのスポーンポイントが埋まっています。({item.ItemName})");
            return null;
        }

        // --- ③ アイテムを生成 ---
        // ItemList に設定された 3D プレハブをスポーン位置に生成
        GameObject obj = Instantiate(item.ItemObject, spawn.position, Quaternion.identity);

        // 名前をわかりやすく設定（例：World_Beer）
        obj.name = $"World_{item.ItemName}";

        // 辞書に登録（どのスポーンポイントに何があるかを記録）
        spawnedObjects[spawn] = obj;

        return obj;
    }

    // ===============================
    // ✅ 次に空いているスポーンポイントを探す
    // ===============================
    private Transform GetNextAvailableSpawnPoint()
    {
        // 前回のインデックスから順番にチェック
        int startIndex = (lastSpawnIndex + 1) % spawnPoints.Length;

        // --- 全スポーンポイントを1周チェック ---
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int index = (startIndex + i) % spawnPoints.Length;
            Transform point = spawnPoints[index];

            // そのスポーンポイントに何もない（または削除済み）なら使用可能
            if (!spawnedObjects.ContainsKey(point) || spawnedObjects[point] == null)
            {
                lastSpawnIndex = index; // 次回のために記録
                return point;
            }
        }

        // すべて埋まっている場合は null を返す
        return null;
    }

    // ===============================
    // ✅ 指定したアイテムを削除する
    // ===============================
    public void RemoveItem(ItemList item)
    {
        if (item == null)
            return;

        Transform targetKey = null;

        // --- 辞書から対象アイテムを探す ---
        foreach (var kv in spawnedObjects)
        {
            GameObject obj = kv.Value;

            // オブジェクト名にアイテム名が含まれていれば対象
            if (obj != null && obj.name.Contains(item.ItemName))
            {
                // オブジェクトを削除
                Destroy(obj);
                targetKey = kv.Key;
                break;
            }
        }

        // --- 対応するキーを辞書から削除 ---
        if (targetKey != null)
            spawnedObjects.Remove(targetKey);
    }

    // ===============================
    // ✅ 全ての生成物を削除（リセット用）
    // ===============================
    public void ClearAll()
    {
        // 全ての生成済みオブジェクトを破棄
        foreach (var kv in spawnedObjects)
        {
            if (kv.Value != null)
                Destroy(kv.Value);
        }

        // 辞書をクリア
        spawnedObjects.Clear();

        // インデックスをリセット
        lastSpawnIndex = -1;
    }
}

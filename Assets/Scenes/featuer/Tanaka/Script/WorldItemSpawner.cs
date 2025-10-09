using UnityEngine;
using System.Collections.Generic;

public class WorldItemSpawner : MonoBehaviour
{
    [Header("3D�I�u�W�F�N�g�̐����ʒu�i��I�u�W�F�N�g���w��j")]
    public Transform[] spawnPoints;

    private Dictionary<Transform, GameObject> spawnedObjects = new Dictionary<Transform, GameObject>();

    /// <summary>
    /// �w�肳�ꂽItemList��ItemObject�𐶐�
    /// </summary>
    public GameObject Spawn(ItemList item)
    {
        if (item == null || item.ItemObject == null)
        {
            Debug.LogWarning($"Spawn���s: ItemObject�����ݒ� ({item?.ItemName})");
            return null;
        }

        Transform spawn = GetAvailableSpawnPoint();
        if (spawn == null)
        {
            Debug.LogWarning($"�S�ẴX�|�[���|�C���g�����܂��Ă��܂��B({item.ItemName})");
            return null;
        }

        GameObject obj = Instantiate(item.ItemObject, spawn.position, Quaternion.identity);
        obj.name = $"World_{item.ItemName}";
        spawnedObjects.Add(spawn, obj);
        return obj;
    }

    /// <summary>
    /// ���݂̐����������ׂč폜
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
    /// �Ή�����A�C�e���I�u�W�F�N�g���폜
    /// </summary>
    public void RemoveItem(ItemList item)
    {
        if (item == null)
            return;

        // ���O�� ItemName ���܂܂�Ă��鐶������T��
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

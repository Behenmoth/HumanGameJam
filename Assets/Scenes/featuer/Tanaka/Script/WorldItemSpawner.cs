using UnityEngine;
using System.Collections.Generic;

public class WorldItemSpawner : MonoBehaviour
{
    [Header("3D�I�u�W�F�N�g�̐����ʒu�i��I�u�W�F�N�g���w��j")]
    public Transform[] spawnPoints;

    private Dictionary<Transform, GameObject> spawnedObjects = new Dictionary<Transform, GameObject>();
    private int lastSpawnIndex = -1; // �Ō�Ɏg�p�����C���f�b�N�X

    public GameObject Spawn(ItemList item)
    {
        if (item == null || item.ItemObject == null)
        {
            Debug.LogWarning($"Spawn���s: ItemObject�����ݒ� ({item?.ItemName})");
            return null;
        }

        Transform spawn = GetNextAvailableSpawnPoint();
        if (spawn == null)
        {
            Debug.LogWarning($"�S�ẴX�|�[���|�C���g�����܂��Ă��܂��B({item.ItemName})");
            return null;
        }

        GameObject obj = Instantiate(item.ItemObject, spawn.position, Quaternion.identity);
        obj.name = $"World_{item.ItemName}";
        spawnedObjects[spawn] = obj;

        return obj;
    }

    /// <summary>
    /// �󂢂Ă���X�|�[���|�C���g�����ԂɒT��
    /// </summary>
    private Transform GetNextAvailableSpawnPoint()
    {
        int startIndex = (lastSpawnIndex + 1) % spawnPoints.Length;

        // spawnPoints�����Ƀ`�F�b�N���āA�󂢂Ă���ꏊ��T��
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

        return null; // �S�Ė��܂��Ă���
    }

    /// <summary>
    /// �Ή�����A�C�e���I�u�W�F�N�g���폜
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
        lastSpawnIndex = -1; // ���Z�b�g
    }
}

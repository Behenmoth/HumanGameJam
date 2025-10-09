using System.Collections.Generic;
using UnityEngine;

public class PlayerInventry : MonoBehaviour
{
    [Header("�����A�C�e��(�ő�4��)")]
    public List<ItemData> items = new List<ItemData>();

    [Header("�����ʒu")]
    public Transform[] itemSlots;

    [Header("���������A�C�e���I�u�W�F�N�g")]
    public List<GameObject> spawnItems = new List<GameObject>();

    [Header("�A�C�e���̏����\��")]
    public int maxItems;

    //�C���x���g���[�ɃA�C�e����ǉ�
    public void AddItems(ItemData item)
    {
        //5�ڈȍ~�̃A�C�e���͎̂Ă�
        if (items.Count > maxItems)
        {
            Debug.Log($"�V�����A�C�e��{item.name}�͔j������܂�");
            return;
        }

        //�A�C�e���̒ǉ�
        items.Add(item);

        //�ǉ������A�C�e���I�u�W�F�N�g�̐���
        SpawnItemObject();
    }

    //�A�C�e���I�u�W�F�N�g�̐�������
    private void SpawnItemObject()
    {
        //���肵���A�C�e���̔ԍ�
        int index = items.Count - 1;

        if (index < itemSlots.Length)
        {
            ItemData itemData = items[index];
            if (itemData.spawnObj != null)
            {
                //�w��̈ʒu�ɃA�C�e���I�u�W�F�N�g�𐶐�����
                GameObject obj = Instantiate(itemData.spawnObj,
                                             itemSlots[index].position,
                                             itemSlots[index].rotation);
                //�����̂��߂Ɏq�I�u�W�F�N�g�ɂ���
                obj.transform.SetParent(itemSlots[index]);
                //���������A�C�e���I�u�W�F�N�g��List�ɉ�����
                spawnItems.Add(obj);
            }
        }
    }

    //�^�[���o�ߎ��p�ɃA�C�e���̕\���E��\����؂�ւ���
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

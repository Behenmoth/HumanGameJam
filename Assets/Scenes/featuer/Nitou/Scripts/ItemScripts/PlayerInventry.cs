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

    [Header("���̃C���x���g���[���Ή�����v���C���[")]
    public GameManager.PlayerTurn ownerTurn;

    //�C���x���g���[�ɃA�C�e����ǉ�
    public void AddItems(ItemData item)
    {
        //5�ڈȍ~�̃A�C�e���͎̂Ă�
        if (items.Count > maxItems)
        {
            Debug.Log($"�V�����A�C�e��{item.name}�͔j������܂�");
            RemoveNewestItem();
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
                //�A�C�e���̊p�x���
                Quaternion rotation = Quaternion.Euler(itemData.spawnRotation);

                //�w��̈ʒu�ɃA�C�e���I�u�W�F�N�g�𐶐�����
                GameObject obj = Instantiate(itemData.spawnObj,
                                             itemSlots[index].position,
                                             rotation);
                //�����̂��߂Ɏq�I�u�W�F�N�g�ɂ���
                obj.transform.SetParent(itemSlots[index]);
                //���������A�C�e���I�u�W�F�N�g��List�ɉ�����
                spawnItems.Add(obj);
            }
        }
    }

    // �ŐV�̃A�C�e�����폜���鏈��
    private void RemoveNewestItem()
    {
        if (items.Count > 0)
        {
            // �ŐV�̃A�C�e�����擾
            int lastIndex = items.Count - 1;

            // �Ή�����I�u�W�F�N�g���폜
            if (lastIndex < spawnItems.Count && spawnItems[lastIndex] != null)
            {
                Destroy(spawnItems[lastIndex]);
                spawnItems.RemoveAt(lastIndex);
            }

            // ���X�g����폜
            items.RemoveAt(lastIndex);
        }
    }

    //�^�[���o�ߎ��p�ɃA�C�e���̕\���E��\����؂�ւ��鏈��
    public void SetActiveObjects(GameManager.PlayerTurn currentPlayerTurn)
    {
        //�Ή�����^�[���ƌ��݂̃^�[������v������true��Ԃ�
        bool isActive = (currentPlayerTurn == ownerTurn);

        foreach(GameObject itemObj in spawnItems)
        {
            if (itemObj != null)
            {
                itemObj.SetActive(isActive);
            }
        }
        Debug.Log("�A�C�e�������ւ���");
    }
}

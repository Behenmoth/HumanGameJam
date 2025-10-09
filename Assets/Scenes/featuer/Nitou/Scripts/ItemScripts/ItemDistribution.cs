using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ItemDistribution : MonoBehaviour
{
    public static ItemDistribution instance;

    [Header("�S�A�C�e���f�[�^���X�g")]
    public List<ItemData> allItems = new List<ItemData>();

    [Header("�v���C���[�̃C���x���g���[")]
    public PlayerInventry player1Inventry;
    public PlayerInventry player2Inventry;

    public void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //���E���h�J�n����2�A�C�e����z�鏈��
    public void Distribution()
    {
        //�A�C�e�����Ȃ�������return
        if (allItems == null || allItems.Count == 0)
        {
            Debug.LogWarning("�A�C�e��������܂���");
            return;
        }

        //playerInventry���ݒ肳��Ă��Ȃ�������return
        if (player1Inventry == null || player2Inventry == null) 
        {
            Debug.LogWarning("�v���C���[�C���x���g���[���ݒ肳��Ă��܂���");
            return;
        }

        //�A�C�e���������_���ɒ��I
    }

    //���I���ꂽ�A�C�e����List�ɉ����鏈��
    private void GiveRandomItems(PlayerInventry inventry, int count)
    {
        //�A�C�e����z�z���鐔�����J��Ԃ�
        for (int i = 0; i < count; i++)
        {
            //���I���ꂽ�A�C�e����ݒ肷��
            ItemData selectedItem = GetRandomItem();

            if (selectedItem != null)
            {
                inventry.AddItems(selectedItem);
                Debug.Log($"{inventry.name}��{selectedItem.itemName}����肵�܂���");
            }
        }
    }

    //�e�A�C�e���ɐݒ肳�ꂽ�m���ŃA�C�e���𒊑I���鏈��
    private ItemData GetRandomItem()
    {
        float totalRate = 0f;
        foreach(var item in allItems)
        {
            totalRate += item.dropRate;
        }

        float randomValue = Random.value * totalRate;
        float cumulatible = 0f;

        foreach(var item in allItems)
        {
            cumulatible += item.dropRate;

            //�����_���Œ��I���ꂽ�A�C�e����Ԃ�
            if (randomValue <= cumulatible)
            {
                return item;
            }
        }

        return allItems.Count > 0 ? allItems[Random.Range(0, allItems.Count)] : null;
    }

    //�A�C�e���̕\���E��\����؂�ւ����Ăяo������
    public void UpdateVisibleItems(GameManager.PlayerTurn currentTurn)
    {
        if (player1Inventry == null || player2Inventry == null)
        {
            return;
        }

        player1Inventry.SetActiveObjects(currentTurn == GameManager.PlayerTurn.Player1);
        player2Inventry.SetActiveObjects(currentTurn == GameManager.PlayerTurn.Player2);
    }
}

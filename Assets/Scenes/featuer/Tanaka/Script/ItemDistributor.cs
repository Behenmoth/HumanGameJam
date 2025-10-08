using UnityEngine;
using System.Collections.Generic;

public class ItemDistributor : MonoBehaviour
{
    [SerializeField] private ItemRate itemRateSystemScript;

    // �v���C���[���Ƃ̃A�C�e�����ʂ��i�[�i�ő�4�j
    public List<ItemList> player1Items = new List<ItemList>();
    public List<ItemList> player2Items = new List<ItemList>();

    // ItemDisplay�𕡐�����
    public List<ItemDisplay> itemDisplays = new List<ItemDisplay>();

    private const int MaxItems = 4;

    public void Start()
    {
        DistributeItems();
    }

    public void DistributeItems()
    {
        // ========================
        // �v���C���[1��2�̃A�C�e����z��
        // ========================
        var newItems1 = itemRateSystemScript.GetTwoRandomItemsAdjusted();
        AddItemsWithLimit(player1Items, newItems1);

        // ========================
        // �v���C���[2��2�̃A�C�e����z��
        // ========================
        var newItems2 = itemRateSystemScript.GetTwoRandomItemsAdjusted();
        AddItemsWithLimit(player2Items, newItems2);

        // ========================
        // ���ʂ��f�o�b�O�o��
        // ========================
        Debug.Log($"�v���C���[1 �� {string.Join(", ", player1Items.ConvertAll(i => i.ItemName))}");
        Debug.Log($"�v���C���[2 �� {string.Join(", ", player2Items.ConvertAll(i => i.ItemName))}");

        // ========================
        // �e ItemDisplay ���X�V
        // ========================

        UpdateAllDisplays();
    }

    public void AddConditionalItem(ItemList item, bool isPlayer1)
    {
        if (item == null) return;

        var targetList = isPlayer1 ? player1Items : player2Items;

        if (targetList.Count < MaxItems)
        {
            // �܂�4�����Ȃ�ǉ�
            targetList.Add(item);
            Debug.Log($"�A�C�e���u{item.ItemName}�v��ǉ����܂����B���݂̏������F{targetList.Count}");
        }
        else
        {
            // 4���łɂ���ꍇ �� �V�����A�C�e���͔j��
            Debug.Log($"�A�C�e���u{item.ItemName}�v�͒ǉ��ł��܂���B����i{MaxItems}�j�ɒB���Ă��邽�ߔj�����܂����B");
        }

        UpdateAllDisplays();
    }

    private void AddItemsWithLimit(List<ItemList> targetList, ItemList[] newItems)
    {
        foreach (var item in newItems)
        {
            if (targetList.Count < MaxItems)
            {
                targetList.Add(item); // 4�����Ȃ�ǉ�
                Debug.Log($"�A�C�e���u{item.ItemName}�v��ǉ����܂����B");
            }
            else
            {
                // ���ł�4����ꍇ�͐V�����A�C�e����j��
                Debug.Log($"5�ڂ̃A�C�e���u{item.ItemName}�v�͏���̂��ߔj������܂����B");
            }
        }
    }

    public void UpdateAllDisplays()
    {
        foreach (var display in itemDisplays)
        {
            if (display != null)
                display.UpdateItemDisplay();
        }

    }


}

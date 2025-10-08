using UnityEngine;
using System.Collections.Generic;

public class ItemDistributor : MonoBehaviour
{
    [SerializeField] private ItemRate itemRateSystemScript;

    // �v���C���[���Ƃ̃A�C�e�����ʂ��i�[
    public ItemList[] player1Items;
    public ItemList[] player2Items;

    // ItemDisplay�𕡐�����
    public List<ItemDisplay> itemDisplays = new List<ItemDisplay>();

    public void Start()
    {
        DistributeItems();
    }

    public void DistributeItems()
    {
        // ========================
        // �v���C���[1��2�̃A�C�e����z��
        // ========================
        player1Items = itemRateSystemScript.GetTwoRandomItemsAdjusted();

        // ========================
        // �v���C���[2��2�̃A�C�e����z��
        // ========================
        player2Items = itemRateSystemScript.GetTwoRandomItemsAdjusted();

        // ========================
        // ���ʂ��f�o�b�O�o��
        // ========================
        Debug.Log($"�v���C���[1 �� 1: {player1Items[0]?.ItemName}, 2: {player1Items[1]?.ItemName}");
        Debug.Log($"�v���C���[2 �� 1: {player2Items[0]?.ItemName}, 2: {player2Items[1]?.ItemName}");

        // ========================
        // �e ItemDisplay ���X�V
        // ========================
        foreach (var display in itemDisplays)
        {
            if (display != null)
            {
                display.UpdateItemDisplay();
            }
        }
    }
}

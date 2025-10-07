using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemRateSystem
{
    [UnityEngine.Range(0f,100f)] // 0%����100%�͈̔�
    public float Rate; // public�ɂ��邱�Ƃ�Inspector�ɕ\�������

    public ItemList[] item; // public�ɂ��邱�Ƃ�Inspector�ɕ\�������
    public int ItemIndex;
}
public class ItemRate : MonoBehaviour
{
    [SerializeField] private ItemList[] item;
    [SerializeField] private List<ItemRateSystem> itemRateSystem;



    private ItemList GetRandomItem()
    {
        if (item.Length == 0 || itemRateSystem.Count == 0)
        {
            Debug.LogError("���̒ނ��ɃA�C�e��������܂���I");
            return null;
        }

        float totalRate = 0f;
        for (int i = 0; i < itemRateSystem.Count; i++)
        {
            totalRate += itemRateSystem[i].Rate;
        }

        float randomValue = Random.Range(0f, totalRate);
        Debug.Log($"Total Rate: {totalRate}, Random Value: {randomValue}");

        float cumulativeRate = 0f;

        for (int i = 0; i < itemRateSystem.Count; i++)
        {
            cumulativeRate += itemRateSystem[i].Rate;
            Debug.Log($"Cumulative Rate for index {i}: {cumulativeRate}");

            if (randomValue <= cumulativeRate)
            {
                Debug.Log($"Selected fish: {item[itemRateSystem[i].ItemIndex]} at index {itemRateSystem[i].ItemIndex}");

                int ItemIndex = itemRateSystem[i].ItemIndex;

                if (ItemIndex < item.Length)
                {
                    Debug.Log($"Selected fish: {item[ItemIndex]} at index {ItemIndex}");
                    return item[ItemIndex];
                }
                else
                {
                    Debug.LogError($"fishPool�̃C���f�b�N�X���͈͊O�ł��B{ItemIndex}");
                    return null;
                }
            }
        }
        return null;
    }

    public ItemList[] GetTwoRandomitems()
    {
        ItemList first = GetRandomItem();
        ItemList second = GetRandomItem();

        Debug.Log($"���I���� �� 1��: {first?.ItemName}, 2��: {second?.ItemName}");
        return new ItemList[] { first, second };
    }

}


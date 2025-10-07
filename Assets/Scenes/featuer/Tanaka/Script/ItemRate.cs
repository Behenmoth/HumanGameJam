using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemRateSystem
{
    [UnityEngine.Range(0f,100f)] // 0%����100%�͈̔�
    public float Rate; // public�ɂ��邱�Ƃ�Inspector�ɕ\�������

    public ItemList[] item; // public�ɂ��邱�Ƃ�Inspector�ɕ\�������
}
public class ItemRate:MonoBehaviour
{
    [SerializeField] private List<ItemRateSystem> itemRateSystems;
}

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemRateSystem
{
    [UnityEngine.Range(0f,100f)] // 0%‚©‚ç100%‚Ì”ÍˆÍ
    public float Rate; // public‚É‚·‚é‚±‚Æ‚ÅInspector‚É•\Ž¦‚³‚ê‚é

    public ItemList[] item; // public‚É‚·‚é‚±‚Æ‚ÅInspector‚É•\Ž¦‚³‚ê‚é
}
public class ItemRate:MonoBehaviour
{
    [SerializeField] private List<ItemRateSystem> itemRateSystems;
}

using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "i000", menuName = "アイテム作成")]

public class ItemList : ScriptableObject
{
    //[InspectorLabel("アイテムアイコン")]
    //public Sprite ItemIcon;

    [Header("アイテムID")]
    public int ItemID;

    [Header("アイテム名")]
    public string ItemName;

    [Header("アイテムイメージ")]
    public Sprite ItemImage;

    [Header("アイテムオブジェクト")]
    public GameObject ItemObject;

}

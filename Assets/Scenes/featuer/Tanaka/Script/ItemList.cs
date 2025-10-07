using UnityEngine;

[CreateAssetMenu(fileName = "i000",menuName = "アイテム作成")]

public class ItemList:ScriptableObject
{
    //[InspectorLabel("アイテムアイコン")]
    //public Sprite ItemIcon;

    [Header("アイテム名")]
    public string ItemName;

}

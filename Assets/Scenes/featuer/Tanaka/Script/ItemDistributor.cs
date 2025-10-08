using UnityEngine;
using System.Collections.Generic;

public class ItemDistributor : MonoBehaviour
{
    [SerializeField] private ItemRate itemRateSystemScript;

    // プレイヤーごとのアイテム結果を格納
    public ItemList[] player1Items;
    public ItemList[] player2Items;

    // ItemDisplayを複数持つ
    public List<ItemDisplay> itemDisplays = new List<ItemDisplay>();

    public void Start()
    {
        DistributeItems();
    }

    public void DistributeItems()
    {
        // ========================
        // プレイヤー1に2つのアイテムを配る
        // ========================
        player1Items = itemRateSystemScript.GetTwoRandomItemsAdjusted();

        // ========================
        // プレイヤー2に2つのアイテムを配る
        // ========================
        player2Items = itemRateSystemScript.GetTwoRandomItemsAdjusted();

        // ========================
        // 結果をデバッグ出力
        // ========================
        Debug.Log($"プレイヤー1 → 1: {player1Items[0]?.ItemName}, 2: {player1Items[1]?.ItemName}");
        Debug.Log($"プレイヤー2 → 1: {player2Items[0]?.ItemName}, 2: {player2Items[1]?.ItemName}");

        // ========================
        // 各 ItemDisplay を更新
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

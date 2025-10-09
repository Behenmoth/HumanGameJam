using UnityEngine;
using System.Collections.Generic;

public class ItemDistributor : MonoBehaviour
{
    [SerializeField] private ItemRate itemRateSystemScript;

    // プレイヤーごとのアイテム結果を格納（最大4つ）
    public List<ItemList> player1Items = new List<ItemList>();
    public List<ItemList> player2Items = new List<ItemList>();
    [SerializeField] private WorldItemSpawner worldSpawner; // ← 追加！

    // ItemDisplayを複数持つ
    public List<ItemDisplay> itemDisplays = new List<ItemDisplay>();

    private const int MaxItems = 4;

    public void Start()
    {
        DistributeItems();
    }

    public void DistributeItems()
    {
        // ========================
        // プレイヤー1に2つのアイテムを配る
        // ========================
        var newItems1 = itemRateSystemScript.GetTwoRandomItemsAdjusted();
        AddItemsWithLimit(player1Items, newItems1);

        // ========================
        // プレイヤー2に2つのアイテムを配る
        // ========================
        var newItems2 = itemRateSystemScript.GetTwoRandomItemsAdjusted();
        AddItemsWithLimit(player2Items, newItems2);

        // ========================
        // 結果をデバッグ出力
        // ========================
        Debug.Log($"プレイヤー1 → {string.Join(", ", player1Items.ConvertAll(i => i.ItemName))}");
        Debug.Log($"プレイヤー2 → {string.Join(", ", player2Items.ConvertAll(i => i.ItemName))}");

        // ========================
        // 各 ItemDisplay を更新
        // ========================

        UpdateAllDisplays();
    }

    public void AddConditionalItem(ItemList item, bool isPlayer1)
    {
        if (item == null) return;

        var targetList = isPlayer1 ? player1Items : player2Items;

        if (targetList.Count < MaxItems)
        {
            targetList.Add(item);
            Debug.Log($"アイテム「{item.ItemName}」を追加しました。現在の所持数：{targetList.Count}");

            // ✅ UI + Object 同時生成
            foreach (var display in itemDisplays)
            {
                if (display == null) continue;

                // 対象プレイヤーのDisplayだけ更新
                if ((isPlayer1 && display.target == ItemDisplay.PlayerTarget.Player1) ||
                    (!isPlayer1 && display.target == ItemDisplay.PlayerTarget.Player2))
                {
                    display.GenerateSingleItemUIAndObject(item, isPlayer1);
                }
            }
        }
        else
        {
            Debug.Log($"アイテム「{item.ItemName}」は追加できません。上限({MaxItems})に達しています。");
        }
    }

    private void AddItemsWithLimit(List<ItemList> targetList, ItemList[] newItems)
    {
        foreach (var item in newItems)
        {
            if (targetList.Count < MaxItems)
            {
                targetList.Add(item); // 4未満なら追加
                Debug.Log($"アイテム「{item.ItemName}」を追加しました。");
            }
            else
            {
                // すでに4つある場合は新しいアイテムを破棄
                Debug.Log($"5個目のアイテム「{item.ItemName}」は上限のため破棄されました。");
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

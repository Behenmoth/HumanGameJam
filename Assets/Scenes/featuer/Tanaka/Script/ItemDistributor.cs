using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 🎁 アイテム配布を統括するクラス
/// 各プレイヤーにアイテムを配り、UI更新やワールド上の生成も連動させる。
/// </summary>
public class ItemDistributor : MonoBehaviour
{
    public static ItemDistributor instance;

    [Header("アイテム確率システム参照")]
    [SerializeField] private ItemRate itemRateSystemScript; // アイテム出現確率を管理するスクリプト

    [Header("プレイヤーごとの所持アイテム")]
    // プレイヤー1と2の持ち物リスト（最大4つまで）
    public List<ItemList> player1Items = new List<ItemList>();
    public List<ItemList> player2Items = new List<ItemList>();

    [Header("ワールドアイテム生成管理")]
    [SerializeField] private WorldItemSpawner worldSpawner; // ワールド上で3Dアイテムを生成するためのスクリプト

    [Header("アイテム表示UI管理")]
    // 複数の ItemDisplay（プレイヤー1用・プレイヤー2用 など）を持つ
    public List<ItemDisplay> itemDisplays = new List<ItemDisplay>();

    // 各プレイヤーの所持上限
    private const int MaxItems = 4;

    private void Awake()
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

    private void Start()
    {
        // ゲーム開始時に初期配布
        //DistributeItems();
    }

    /// <summary>
    /// 🎲 両プレイヤーに初期アイテムをランダム配布する
    /// </summary>
    public void DistributeItems()
    {
        // --- プレイヤー1に2つ配布 ---
        var newItems1 = itemRateSystemScript.GetTwoRandomItemsAdjusted();
        AddItemsWithLimit(player1Items, newItems1);

        // --- プレイヤー2に2つ配布 ---
        var newItems2 = itemRateSystemScript.GetTwoRandomItemsAdjusted();
        AddItemsWithLimit(player2Items, newItems2);

        // --- 配布結果をコンソール出力 ---
        Debug.Log($"🎮 プレイヤー1 → {string.Join(", ", player1Items.ConvertAll(i => i.ItemName))}");
        Debug.Log($"🎮 プレイヤー2 → {string.Join(", ", player2Items.ConvertAll(i => i.ItemName))}");

        // --- UIを更新（ItemDisplay 側の再描画）---
        UpdateAllDisplays();
    }

    /// <summary>
    /// 💡 条件付き（例：特定のイベント発生時）でアイテムを1つだけ追加する
    /// </summary>
    public void AddConditionalItem(ItemList item, bool isPlayer1)
    {
        if (item == null) return;

        var targetList = isPlayer1 ? player1Items : player2Items;

        if (targetList.Count < MaxItems)
        {
            targetList.Add(item);
            Debug.Log($"✅ アイテム「{item.ItemName}」を追加しました。現在の所持数：{targetList.Count}");

            // --- UIとワールド上のアイテムを即座に反映 ---
            foreach (var display in itemDisplays)
            {
                if (display == null) continue;

                // 該当プレイヤーのUIのみ更新
                if ((isPlayer1 && display.target == ItemDisplay.PlayerTarget.Player1) ||
                    (!isPlayer1 && display.target == ItemDisplay.PlayerTarget.Player2))
                {
                    display.GenerateSingleItemUIAndObject(item, isPlayer1);
                }
            }
        }
        else
        {
            Debug.Log($"⚠️ アイテム「{item.ItemName}」は追加できません。上限({MaxItems})に達しています。");
        }
    }

    /// <summary>
    /// 📦 配布時に上限を考慮してアイテムをリストに追加
    /// </summary>
    private void AddItemsWithLimit(List<ItemList> targetList, ItemList[] newItems)
    {
        foreach (var item in newItems)
        {
            if (targetList.Count < MaxItems)
            {
                targetList.Add(item);
                Debug.Log($"➕ アイテム「{item.ItemName}」を追加しました。");
            }
            else
            {
                // 上限に達している場合はスキップ
                Debug.Log($"🚫 5個目のアイテム「{item.ItemName}」は上限のため破棄されました。");
            }
        }
    }

    /// <summary>
    /// 🖼️ 登録されているすべての ItemDisplay（UI）を更新
    /// </summary>
    public void UpdateAllDisplays()
    {
        foreach (var display in itemDisplays)
        {
            if (display != null)
                display.UpdateItemDisplay();
        }
    }
}

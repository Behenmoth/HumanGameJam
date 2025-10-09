using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemRateSystem
{
    [Range(0f, 100f)] // Inspector上で0〜100%の範囲をスライダーで設定可能
    public float Rate; // このアイテムが出る確率（重み）

    public ItemList[] item; // 対応するアイテム（配列で保持可能）
    public int ItemIndex;   // 実際に使う item[] のインデックスを指定
}

public class ItemRate : MonoBehaviour
{
    public static ItemRate instance;
    
    [Header("アイテム一覧")]
    [SerializeField] private ItemList[] item; // 抽選対象のアイテム群

    [Header("アイテム出現確率設定")]
    [SerializeField] private List<ItemRateSystem> itemRateSystem; // 各アイテムの確率データリスト

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

    public void Start()
    {
        // テスト用の呼び出し（必要に応じてコメントアウトを外す）
        // GetTwoRandomItemsAdjusted();
    }

    /// <summary>
    /// 単一アイテムを確率に基づいてランダムに取得
    /// </summary>
    private ItemList GetRandomItem()
    {
        if (item.Length == 0 || itemRateSystem.Count == 0)
        {
            Debug.LogError("この釣り場にアイテムがありません！");
            return null;
        }

        // --- 全アイテムの確率を合計 ---
        float totalRate = 0f;
        for (int i = 0; i < itemRateSystem.Count; i++)
        {
            totalRate += itemRateSystem[i].Rate;
        }

        // --- 0〜合計値の範囲でランダム値を取得 ---
        float randomValue = Random.Range(0f, totalRate);
        float cumulativeRate = 0f;

        // --- 累積確率で選定 ---
        for (int i = 0; i < itemRateSystem.Count; i++)
        {
            cumulativeRate += itemRateSystem[i].Rate;
            if (randomValue <= cumulativeRate)
            {
                int ItemIndex = itemRateSystem[i].ItemIndex;

                if (ItemIndex < item.Length)
                {
                    Debug.Log($"🎯 抽選結果: {item[ItemIndex].ItemName} が選ばれました");
                    return item[ItemIndex];
                }
                else
                {
                    Debug.LogError($"⚠️ item 配列のインデックス範囲外です: {ItemIndex}");
                    return null;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 2つのアイテムをランダムに取得（同じアイテムの確率を下げて調整）
    /// </summary>
    public ItemList[] GetTwoRandomItemsAdjusted()
    {
        // --- 1回目の抽選 ---
        ItemList first = GetRandomItem();

        // --- 2回目の抽選用確率を調整 ---
        float reductionRate = 0.5f; // 同一アイテムは確率を50%に減少
        List<ItemRateSystem> adjustedRates = new List<ItemRateSystem>();

        foreach (var irs in itemRateSystem)
        {
            // 元データをコピー
            ItemRateSystem newIRS = new ItemRateSystem
            {
                Rate = irs.Rate,
                item = irs.item,
                ItemIndex = irs.ItemIndex
            };

            // もし1回目と同じアイテムなら確率を半減
            if (first != null && newIRS.ItemIndex == System.Array.IndexOf(item, first))
            {
                newIRS.Rate *= reductionRate;
            }

            adjustedRates.Add(newIRS);
        }

        // --- 調整後の確率で2回目の抽選 ---
        ItemList second = GetRandomItemWithCustomRates(adjustedRates);

        Debug.Log($"抽選結果 → 1つ目: {first?.ItemName}, 2つ目: {second?.ItemName}");
        return new ItemList[] { first, second };
    }

    /// <summary>
    /// カスタム確率リストを使ってランダムアイテムを取得
    /// </summary>
    private ItemList GetRandomItemWithCustomRates(List<ItemRateSystem> customRates)
    {
        // 合計確率を算出
        float totalRate = 0f;
        foreach (var cr in customRates)
            totalRate += cr.Rate;

        // ランダム値を生成
        float randomValue = Random.Range(0f, totalRate);
        float cumulativeRate = 0f;

        // 累積確率で抽選
        for (int i = 0; i < customRates.Count; i++)
        {
            cumulativeRate += customRates[i].Rate;
            if (randomValue <= cumulativeRate)
            {
                int index = customRates[i].ItemIndex;
                if (index < item.Length)
                    return item[index];
            }
        }

        return null;
    }

    /*
    // シンプルな2回抽選（確率調整なし）
    public ItemList[] GetTwoRandomitems() 
    { 
        ItemList first = GetRandomItem(); 
        ItemList second = GetRandomItem(); 
        Debug.Log($"抽選結果 → 1つ目: {first?.ItemName}, 2つ目: {second?.ItemName}"); 
        return new ItemList[] { first, second }; 
    }
    */

    /// <summary>
    /// 条件付きでランダムアイテムを1つ取得し、プレイヤーに配布
    /// </summary>
    public ItemList[] conditionalaGetRandomItem(ItemDistributor distributor, bool isPlayer1)
    {
        ItemList selectedItem = GetRandomItem(); // ランダム抽選

        if (selectedItem != null)
        {
            // プレイヤー1 または 2 の手札に追加
            distributor.AddConditionalItem(selectedItem, isPlayer1);

            Debug.Log($"🎁 プレイヤー{(isPlayer1 ? "1" : "2")}が「{selectedItem.ItemName}」を取得しました！");
        }
        else
        {
            Debug.LogWarning("⚠️ ランダムアイテムの取得に失敗しました。Rate設定を確認してください。");
        }

        // 抽選結果を配列で返す
        return new ItemList[] { selectedItem };
    }
}

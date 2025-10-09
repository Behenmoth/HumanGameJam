using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemRateSystem
{
    [UnityEngine.Range(0f,100f)] // 0%から100%の範囲
    public float Rate; // publicにすることでInspectorに表示される

    public ItemList[] item; // publicにすることでInspectorに表示される
    public int ItemIndex;
}
public class ItemRate : MonoBehaviour
{
    [SerializeField] private ItemList[] item;
    [SerializeField] private List<ItemRateSystem> itemRateSystem;

    public void Start()
    {
        //GetTwoRandomitems();
        //GetTwoRandomItemsAdjusted();
    }

    private ItemList GetRandomItem()
    {
        if (item.Length == 0 || itemRateSystem.Count == 0)
        {
            Debug.LogError("この釣り場にアイテムがありません！");
            return null;
        }

        float totalRate = 0f;
        for (int i = 0; i < itemRateSystem.Count; i++)
        {
            totalRate += itemRateSystem[i].Rate;
        }

        float randomValue = Random.Range(0f, totalRate);
        //Debug.Log($"Total Rate: {totalRate}, Random Value: {randomValue}");

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
                    Debug.LogError($"fishPoolのインデックスが範囲外です。{ItemIndex}");
                    return null;
                }
            }
        }
        return null;
    }

    public ItemList[] GetTwoRandomItemsAdjusted()
    {
        // ========================
        // 1回目の抽選
        // ========================
        ItemList first = GetRandomItem(); // まず1つ目のアイテムをランダムで取得

        // ========================
        // 2回目の抽選用に確率を調整
        // ========================
        float reductionRate = 0.5f; // 1回目と同じアイテムの確率を50%に下げる
        List<ItemRateSystem> adjustedRates = new List<ItemRateSystem>();

        foreach (var irs in itemRateSystem)
        {
            // 元のItemRateSystemをコピーして調整用リストを作成
            ItemRateSystem newIRS = new ItemRateSystem
            {
                Rate = irs.Rate,
                item = irs.item,
                ItemIndex = irs.ItemIndex
            };

            // もし1回目に選ばれたアイテムと同じなら確率を下げる
            if (first != null && newIRS.ItemIndex == System.Array.IndexOf(item, first))
            {
                newIRS.Rate *= reductionRate; // 確率を50%に減少
            }

            adjustedRates.Add(newIRS); // 調整済みのリストに追加
        }

        // ========================
        // 2回目の抽選
        // ========================
        ItemList second = GetRandomItemWithCustomRates(adjustedRates);
        // カスタム確率リストを使って2回目のアイテムを選ぶ
        //ItemList third = GetRandomItem();
        //ItemList fourth = GetRandomItem();

        // ========================
        // 結果をデバッグ出力
        // ========================
        Debug.Log($"抽選結果 → 1つ目: {first?.ItemName}, 2つ目: {second?.ItemName}");

        // 抽選結果を配列で返す
        return new ItemList[] { first, second };//,third,fourth};
    }

    // ========================
    // カスタム確率でアイテムを抽選する関数
    // ========================
    private ItemList GetRandomItemWithCustomRates(List<ItemRateSystem> customRates)
    {
        // 合計確率を計算
        float totalRate = 0f;
        for (int i = 0; i < customRates.Count; i++)
        {
            totalRate += customRates[i].Rate;
        }

        // 0〜totalRateの間でランダムに値を決める
        float randomValue = Random.Range(0f, totalRate);
        float cumulativeRate = 0f;

        // 累積確率でどのアイテムが選ばれるか決定
        for (int i = 0; i < customRates.Count; i++)
        {
            cumulativeRate += customRates[i].Rate;
            if (randomValue <= cumulativeRate)
            {
                int index = customRates[i].ItemIndex;

                // インデックスが範囲内ならアイテムを返す
                if (index < item.Length)
                    return item[index];
            }
        }

        // 何も選ばれなかった場合はnullを返す
        return null;
    }
    /*public ItemList[] GetTwoRandomitems() 
    { 
        ItemList first = GetRandomItem(); 
        ItemList second = GetRandomItem(); 
        Debug.Log($"抽選結果 → 1つ目: {first?.ItemName}, 2つ目: {second?.ItemName}"); 
        return new ItemList[] { first, second }; 
    }*/
    // 条件付きでランダムアイテムを1つ生成し、プレイヤーに渡す
    public ItemList[] conditionalaGetRandomItem(ItemDistributor distributor, bool isPlayer1)
    {
        // --- アイテム選択前に一時的な状態をリセット ---
        // （この関数が呼ばれるたびに新しく開始する）
        ItemList selectedItem = null;

        // --- ランダムで1つアイテムを取得（確率処理は GetRandomItem() に任せる）---
        selectedItem = GetRandomItem();

        if (selectedItem != null)
        {
            // --- 1つだけプレイヤーリストに追加 ---
            distributor.AddConditionalItem(selectedItem, isPlayer1);

            // --- デバッグ出力 ---
            Debug.Log($"プレイヤー{(isPlayer1 ? "1" : "2")}がアイテム「{selectedItem.ItemName}」を取得しました！");
        }
        else
        {
            Debug.LogWarning("ランダムアイテムの取得に失敗しました。Rate設定を確認してください。");
        }

        // --- 今回生成されたアイテムだけを返す ---
        return new ItemList[] { selectedItem };
    }
}


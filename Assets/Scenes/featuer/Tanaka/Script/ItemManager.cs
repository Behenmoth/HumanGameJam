using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [Header("UI参照")]
    public ItemSelectUI itemSelectUI; // ← Inspectorで設定

    [Header("アイテムを使ったか")]
    public bool usedItems = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public bool UseItem(int itemId)
    {
        if (usedItems)
        {
            Debug.Log("すでにアイテムを使いました");
            return false;
        }

        bool shouldDelete = false;

        switch (itemId)
        {
            case 0:
                UseBeer();
                shouldDelete = true; // 即削除
                break;

            case 1:
                UseHandcuff();
                shouldDelete = true;
                break;

            case 2:
                UseInjection();
                shouldDelete = false; // UI選択が必要 → 削除しない
                break;

            case 3:
                UseRemote();
                shouldDelete = false;
                break;

            case 4:
                UseDriver();
                shouldDelete = true;
                break;

            default:
                Debug.LogWarning("不明なアイテムIDです");
                return false;
        }

        if (shouldDelete)
            usedItems = true; // ✅ 即時使用アイテムのみここでフラグON

        return shouldDelete;
    }

    // --- 各アイテム効果 ---
    private void UseBeer()
    {
        Debug.Log("🍺 ビール使用：自分のターンを飛ばす");
        GameManager.instance.PassTurn();
    }

    private void UseHandcuff()
    {
        Debug.Log("🔗 手錠使用：相手のターンを飛ばす");
        GameManager.instance.PassTurn();
        GameManager.instance.PassTurn();
    }

    private void UseInjection()
    {
        Debug.Log("💉 注射使用：叩く回数を選択してください");
        itemSelectUI.Open("叩く回数を選んでください (1〜3回)", (value) =>
        {
            if (value < 1)
            {
                Debug.Log("💉 キャンセルされました");
                return;
            }

            BombManager.instance.SetLimitedClicks(value);
            Debug.Log($"💉 相手の叩く回数を {value} 回に制限しました");

            usedItems = true; // ✅ UI完了後にフラグON
        });
    }

    private void UseRemote()
    {
        Debug.Log("📺 リモコン使用：爆弾カウントを増やす回数を選択してください");
        itemSelectUI.Open("爆弾カウントを増やす回数 (1〜3)", (value) =>
        {
            if (value < 1)
            {
                Debug.Log("📺 キャンセルされました");
                return;
            }

            BombManager.instance.AddBombCount(value);
            Debug.Log($"📺 爆弾カウントを {value} 増やしました");

            usedItems = true;
        });
    }

    private void UseDriver()
    {
        Debug.Log("🔧 ドライバー使用：爆弾カウントを相手ターン中は非表示にする");
        BombManager.instance.HideBombCountForOpponent();
    }

    public void ResetUsedItems()
    {
        usedItems = false;
    }
}

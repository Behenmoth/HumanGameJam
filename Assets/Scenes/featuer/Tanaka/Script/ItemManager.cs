using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // --- シングルトンインスタンス（他スクリプトから ItemManager.instance で呼び出せるように） ---
    public static ItemManager instance;

    [Header("UI参照")]
    // ItemSelectUI をインスペクターで指定（注射やリモコン使用時に表示される選択UI）
    public ItemSelectUI itemSelectUI;

    [Header("アイテムを使ったか")]
    // 1ターン中にアイテムをすでに使ったかどうかのフラグ
    public bool usedItems = false;

    // ===============================
    // ✅ シングルトン初期化処理
    // ===============================
    private void Awake()
    {
        // すでに別のインスタンスが存在する場合は自分を破棄（1つだけにする）
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // ===============================
    // ✅ アイテム使用のメイン処理
    // ===============================
    // itemId：アイテムの種類を識別するID
    // 戻り値：使用後にアイテムを削除して良いかどうか（true = 削除）
    public bool UseItem(int itemId)
    {
        // すでにアイテムを使用済みなら再使用不可
        if (usedItems)
        {
            Debug.Log("すでにアイテムを使いました");
            return false;
        }

        // アイテム削除フラグ
        bool shouldDelete = false;

        // --- アイテムの種類ごとに効果を分岐 ---
        switch (itemId)
        {
            case 0:
                UseBeer();
                shouldDelete = true;  // 即効果発動 → 削除
                break;

            case 1:
                UseHandcuff();
                shouldDelete = true;  // 即効果発動 → 削除
                break;

            case 2:
                UseInjection();
                shouldDelete = false; // UIで選択が必要 → 削除は後で
                break;

            case 3:
                UseRemote();
                shouldDelete = false; // 同上（選択式）
                break;

            case 4:
                UseDriver();
                shouldDelete = true;
                break;

            default:
                Debug.LogWarning("不明なアイテムIDです");
                return false;
        }

        // --- 即使用アイテムの場合のみここで使用済みに設定 ---
        if (shouldDelete)
            usedItems = true;

        // 呼び出し元に削除の可否を返す
        return shouldDelete;
    }

    // ===============================
    // ✅ 各アイテムの効果定義
    // ===============================

    // 🍺 ビール：自分のターンを飛ばす
    private void UseBeer()
    {
        Debug.Log("🍺 ビール使用：自分のターンを飛ばす");
        GameManager.instance.PassTurn();
    }

    // 🔗 手錠：相手のターンを2回飛ばす（実質スキップ）
    private void UseHandcuff()
    {
        Debug.Log("🔗 手錠使用：相手のターンを飛ばす");
        GameManager.instance.PassTurn();
        GameManager.instance.PassTurn();
    }

    // 💉 注射：相手の叩ける回数を制限（1〜3回選択UIあり）
    private void UseInjection()
    {
        Debug.Log("💉 注射使用：叩く回数を選択してください");

        // UIを開いてプレイヤーに選ばせる
        itemSelectUI.Open("叩く回数を選んでください (1〜3回)", (value) =>
        {
            // value < 1 はキャンセル扱い
            if (value < 1)
            {
                Debug.Log("💉 キャンセルされました");
                return;
            }

            // BombManager に選択した回数を渡して制限
            BombManager.instance.SetLimitedClicks(value);
            Debug.Log($"💉 相手の叩く回数を {value} 回に制限しました");

            // UIでの選択完了後にフラグON
            usedItems = true;
        });
    }

    // 📺 リモコン：爆弾カウントを増やす（1〜3回選択UIあり）
    private void UseRemote()
    {
        Debug.Log("📺 リモコン使用：爆弾カウントを増やす回数を選択してください");

        // UIを開いてプレイヤーに選ばせる
        itemSelectUI.Open("爆弾カウントを増やす回数 (1〜3)", (value) =>
        {
            if (value < 1)
            {
                Debug.Log("📺 キャンセルされました");
                return;
            }

            // BombManager に加算回数を伝える
            BombManager.instance.AddBombCount(value);
            Debug.Log($"📺 爆弾カウントを {value} 増やしました");

            usedItems = true;
        });
    }

    // 🔧 ドライバー：相手ターン中に爆弾カウントを非表示にする
    private void UseDriver()
    {
        Debug.Log("🔧 ドライバー使用：爆弾カウントを相手ターン中は非表示にする");
        BombManager.instance.HideBombCountForOpponent();
    }

    // ===============================
    // ✅ フラグをリセット（ターン切り替えなどで使用）
    // ===============================
    public void ResetUsedItems()
    {
        usedItems = false;
    }
}

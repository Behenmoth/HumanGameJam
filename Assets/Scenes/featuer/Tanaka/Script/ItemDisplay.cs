using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemDisplay : MonoBehaviour
{
    // --- ItemDistributor（アイテム配布クラス）への参照 ---
    [HideInInspector]
    public ItemDistributor distributor;

    [Header("生成するImageプレハブ（Imageコンポーネントを持つUIプレハブ）")]
    // 各アイテムを表示するためのUIプレハブ（例：Imageボタンなど）
    public GameObject itemImagePrefab;

    [Header("生成先の親 (Canvas配下のTransform)")]
    // 手札アイテムUIを並べる親オブジェクト（GridLayoutGroup推奨）
    public Transform itemParent;

    [Header("ワールドアイテム生成管理")]
    // 3D空間上のアイテム生成を管理するクラス
    public WorldItemSpawner spawner;

    // UIオブジェクト（Image）とワールド上の3Dオブジェクトを対応付ける辞書
    private Dictionary<Transform, GameObject> spawnedObjects = new Dictionary<Transform, GameObject>();

    // --- プレイヤーの区別（どちらの手札を表示するか） ---
    public enum PlayerTarget { Player1, Player2 }

    [Header("どのプレイヤーのアイテムを表示するか")]
    public PlayerTarget target = PlayerTarget.Player1;

    // ===============================
    // ✅ Distributor（アイテム配布管理）をセット
    // ===============================
    public void SetDistributor(ItemDistributor d)
    {
        distributor = d;
    }

    // ===============================
    // ✅ 表示対象プレイヤーを切り替える
    // ===============================
    public void SetPlayerTarget(PlayerTarget newTarget)
    {
        target = newTarget;
        Debug.Log($"表示対象を {target} に設定しました");
        UpdateItemDisplay(); // UIを再生成
    }

    // ===============================
    // ✅ 現在のプレイヤーのアイテムをUIに反映
    // ===============================
    public void UpdateItemDisplay()
    {
        // --- エラーチェック ---
        if (distributor == null)
        {
            Debug.LogError($"[{name}] distributor が設定されていません。", this);
            return;
        }

        // プレイヤー1か2のアイテムリストを取得
        var items = (target == PlayerTarget.Player1)
            ? distributor.player1Items
            : distributor.player2Items;

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning($"[{name}] 表示対象の items が空または null です。target={target}", this);
            return;
        }

        if (itemParent == null)
        {
            Debug.LogError($"[{name}] itemParent が未設定です。Inspectorで設定してください。", this);
            return;
        }

        if (itemImagePrefab == null)
        {
            Debug.LogError($"[{name}] itemImagePrefab が未設定です。Inspectorで設定してください。", this);
            return;
        }

        // --- ① 既存のUIを全削除（リセット） ---
        for (int i = itemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        // --- ② 新しいアイテムUIを順に生成 ---
        for (int i = 0; i < items.Count; i++)
        {
            var itemData = items[i];
            if (itemData == null)
                continue;

            // ✅ UI（手札画像）の生成
            GameObject uiObj = Instantiate(itemImagePrefab, itemParent);
            Image img = uiObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = itemData.ItemImage;
                uiObj.name = $"Item_{i}_{itemData.ItemName}";
            }

            bool isPlayer1 = (target == PlayerTarget.Player1);

            // ✅ 対応するワールドアイテムを生成（3D空間に出現）
            GameObject spawnedObj = null;
            if (spawner != null)
            {
                spawnedObj = spawner.Spawn(itemData);
            }

            // 紐付け登録（UI ↔ 3Dオブジェクト）
            spawnedObjects[uiObj.transform] = spawnedObj;

            // ✅ ボタン押下時のイベントを登録
            AddClickHandler(uiObj, itemData, isPlayer1);
        }

        Debug.Log($"[{name}] {items.Count} 件のアイテムを生成しました (target={target})", this);
    }

    // ===============================
    // ✅ ボタンにクリックイベントを追加
    // ===============================
    private void AddClickHandler(GameObject obj, ItemList item, bool isPlayer1)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn == null)
            btn = obj.AddComponent<Button>(); // なければ追加

        btn.onClick.AddListener(() =>
        {
            if (ItemManager.instance == null)
                return;

            // 🎯 アイテム効果を実行（ItemManagerに処理を委ねる）
            bool canDelete = ItemManager.instance.UseItem(item.ItemID);

            // 🎯 即削除できるアイテムのみUI＋3Dを削除
            if (canDelete)
            {
                RemoveItem(obj, item, isPlayer1);
            }
            else
            {
                Debug.Log($"アイテム「{item.ItemName}」は選択が必要です。削除を保留します。");
                // 💉や📺のようにUI操作が必要なアイテムは、
                // ItemManagerから後でRemoveItem()が呼ばれる想定
            }
        });
    }

    // ===============================
    // ✅ 単一のアイテムだけUIと3Dを生成する
    // ===============================
    public void GenerateSingleItemUIAndObject(ItemList itemData, bool isPlayer1)
    {
        if (itemData == null) return;

        // ✅ UI生成
        GameObject uiObj = Instantiate(itemImagePrefab, itemParent);
        Image img = uiObj.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = itemData.ItemImage;
            uiObj.name = $"Item_{itemData.ItemName}";
        }

        // ✅ ワールドアイテム生成
        GameObject spawnedObj = null;
        if (spawner != null)
        {
            spawnedObj = spawner.Spawn(itemData);
        }

        // 紐付け登録（UI ↔ ワールド）
        spawnedObjects[uiObj.transform] = spawnedObj;

        // ✅ 削除イベント登録
        AddClickHandler(uiObj, itemData, isPlayer1);

        Debug.Log($"[{name}] 新アイテム {itemData.ItemName} のUIとObjectを生成しました");
    }

    // ===============================
    // ✅ アイテム削除（UI＋3D両方）
    // ===============================
    public void RemoveItem(GameObject obj, ItemList item, bool isPlayer1)
    {
        // --- 3D側削除 ---
        if (spawner != null)
            spawner.RemoveItem(item);

        // --- UI側削除 ---
        Destroy(obj);

        // --- リストから削除 ---
        var targetList = isPlayer1 ? distributor.player1Items : distributor.player2Items;
        if (targetList.Contains(item))
            targetList.Remove(item);

        Debug.Log($"アイテム「{item.ItemName}」を削除しました。");
    }

    // ===============================
    // ✅ 外部（例：ItemManager）からもUIを削除可能にする仕組み
    // ===============================
    public static ItemDisplay currentDisplay;

    private void Awake()
    {
        currentDisplay = this;
    }

    // --- 静的メソッドでUI側アイテムを削除（名前一致検索） ---
    public static void RemoveItemFromUI(ItemList item)
    {
        if (currentDisplay == null) return;

        // itemParent 内の子オブジェクトを走査して削除
        foreach (Transform child in currentDisplay.itemParent)
        {
            if (child.name.Contains(item.ItemName))
            {
                currentDisplay.RemoveItem(
                    child.gameObject,
                    item,
                    currentDisplay.target == PlayerTarget.Player1
                );
                break;
            }
        }
    }
}

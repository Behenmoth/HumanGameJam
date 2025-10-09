using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemDisplay : MonoBehaviour
{
    [HideInInspector]
    public ItemDistributor distributor;

    [Header("生成するImageプレハブ（Imageコンポーネントを持つUIプレハブ）")]
    public GameObject itemImagePrefab;

    [Header("生成先の親 (Canvas配下のTransform)")]
    public Transform itemParent;

    [Header("ワールドアイテム生成管理")]
    public WorldItemSpawner spawner;

    private Dictionary<Transform, GameObject> spawnedObjects = new Dictionary<Transform, GameObject>();

    public enum PlayerTarget { Player1, Player2 }

    [Header("どのプレイヤーのアイテムを表示するか")]
    public PlayerTarget target = PlayerTarget.Player1;

    public void SetDistributor(ItemDistributor d)
    {
        distributor = d;
    }

    public void SetPlayerTarget(PlayerTarget newTarget)
    {
        target = newTarget;
        Debug.Log($"表示対象を {target} に設定しました");
        UpdateItemDisplay();
    }

    public void UpdateItemDisplay()
    {
        if (distributor == null)
        {
            Debug.LogError($"[{name}] distributor が設定されていません。", this);
            return;
        }

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

        // --- 既存の子を削除 ---
        for (int i = itemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        // --- 新しいアイテムを生成 ---
        for (int i = 0; i < items.Count; i++)
        {
            var itemData = items[i];
            if (itemData == null)
                continue;

            // ✅ 手札UIの生成
            GameObject uiObj = Instantiate(itemImagePrefab, itemParent);
            Image img = uiObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = itemData.ItemImage;
                uiObj.name = $"Item_{i}_{itemData.ItemName}";
            }

            bool isPlayer1 = (target == PlayerTarget.Player1);

            // ✅ ワールドアイテム生成
            GameObject spawnedObj = null;
            if (spawner != null)
            {
                spawnedObj = spawner.Spawn(itemData);
            }

            spawnedObjects[uiObj.transform] = spawnedObj;

            // ✅ ボタンイベント登録
            AddClickHandler(uiObj, itemData, isPlayer1);
        }

        Debug.Log($"[{name}] {items.Count} 件のアイテムを生成しました (target={target})", this);
    }

    private void AddClickHandler(GameObject obj, ItemList item, bool isPlayer1)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn == null)
            btn = obj.AddComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            if (ItemManager.instance == null)
                return;

            // 🎯 アイテム効果を実行
            bool canDelete = ItemManager.instance.UseItem(item.ItemID);

            // 🎯 即削除可能なアイテムのみ削除
            if (canDelete)
            {
                RemoveItem(obj, item, isPlayer1);
            }
            else
            {
                Debug.Log($"アイテム「{item.ItemName}」は選択が必要です。削除を保留します。");
                // 💉や📺は UI操作完了後に ItemManager 側から RemoveItem() を呼ぶ
            }
        });
    }

    // 1つのアイテムだけUI＋Object生成する専用メソッド
    public void GenerateSingleItemUIAndObject(ItemList itemData, bool isPlayer1)
    {
        if (itemData == null) return; 
        // ✅ UI生成
        GameObject uiObj = Instantiate(itemImagePrefab, itemParent); Image img = uiObj.GetComponent<Image>(); if (img != null) { img.sprite = itemData.ItemImage; uiObj.name = $"Item_{itemData.ItemName}"; } 
        // ✅ Object生成
        GameObject spawnedObj = null; if (spawner != null) { spawnedObj = spawner.Spawn(itemData); } 
        // 紐付け登録
        spawnedObjects[uiObj.transform] = spawnedObj;
        // 削除イベント登録
        AddClickHandler(uiObj, itemData, isPlayer1);
        Debug.Log($"[{name}] 新アイテム {itemData.ItemName} のUIとObjectを生成しました"); 
    }
    // --- 共通削除処理 ---
    public void RemoveItem(GameObject obj, ItemList item, bool isPlayer1)
    {
        if (spawner != null)
            spawner.RemoveItem(item);

        Destroy(obj);

        var targetList = isPlayer1 ? distributor.player1Items : distributor.player2Items;
        if (targetList.Contains(item))
            targetList.Remove(item);

        Debug.Log($"アイテム「{item.ItemName}」を削除しました。");
    }

    // --- 外部（ItemManager）からも呼べるように ---
    public static ItemDisplay currentDisplay;

    private void Awake()
    {
        currentDisplay = this;
    }

    public static void RemoveItemFromUI(ItemList item)
    {
        if (currentDisplay == null) return;

        // itemParent 内の対象UIを探して削除
        foreach (Transform child in currentDisplay.itemParent)
        {
            if (child.name.Contains(item.ItemName))
            {
                currentDisplay.RemoveItem(child.gameObject, item, currentDisplay.target == PlayerTarget.Player1);
                break;
            }
        }
    }
}

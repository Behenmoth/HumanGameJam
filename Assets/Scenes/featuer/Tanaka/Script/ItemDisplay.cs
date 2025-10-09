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

        // List<ItemList> に対応
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

        //// --- 新しいアイテムを生成 ---
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
                spawnedObj = spawner.Spawn(itemData); // 戻り値を受け取る
            }

            // ✅ 紐付けを登録
            spawnedObjects[uiObj.transform] = spawnedObj;

            // ✅ 削除機能を追加（Objectも削除）
            AddClickToDestroy(uiObj, itemData, isPlayer1);
        }

        Debug.Log($"[{name}] {items.Count} 件のアイテムを生成しました (target={target})", this);
    }

    private void AddClickToDestroy(GameObject obj, ItemList item, bool isPlayer1)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn == null)
            btn = obj.AddComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            // 🎯 対応する3Dオブジェクトを削除
            if (spawnedObjects.ContainsKey(obj.transform))
            {
                var linkedObj = spawnedObjects[obj.transform];
                if (linkedObj != null)
                    Destroy(linkedObj);

                spawnedObjects.Remove(obj.transform);
            }

            // UIを削除
            Destroy(obj);

            // リストから削除
            var targetList = isPlayer1 ? distributor.player1Items : distributor.player2Items;
            if (targetList.Contains(item))
                targetList.Remove(item);

            // 表示更新
            distributor.UpdateAllDisplays();
        });
    }

}

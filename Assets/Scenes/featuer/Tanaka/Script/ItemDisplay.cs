using UnityEngine;
using UnityEngine.EventSystems;
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

    public enum PlayerTarget { Player1, Player2 }

    [Header("どのプレイヤーのアイテムを表示するか")]
    public PlayerTarget target = PlayerTarget.Player1;

    public void SetDistributor(ItemDistributor d)
    {
        distributor = d;
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

        // --- 新しいアイテムを生成 ---
        // --- 新しいアイテムを生成 ---
        for (int i = 0; i < items.Count; i++)
        {
            var itemData = items[i];
            if (itemData == null)
            {
                Debug.LogWarning($"[{name}] items[{i}] が null です。", this);
                continue;
            }

            GameObject go = Instantiate(itemImagePrefab, itemParent);
            Image img = go.GetComponent<Image>();

            if (img != null)
            {
                img.sprite = itemData.ItemImage;
                go.name = $"Item_{i}_{itemData.ItemName}";
            }
            else
            {
                Debug.LogError($"[{name}] プレハブに Image コンポーネントがありません: {itemImagePrefab.name}", this);
            }

            // 🔽 targetがPlayer1かPlayer2かでisPlayer1を判定して渡す
            bool isPlayer1 = (target == PlayerTarget.Player1);

            // 🔽 アイテム削除時にリストも更新できるように引数を渡す
            AddClickToDestroy(go, itemData, isPlayer1);
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
            Destroy(obj);

            // ✅ データ側リストにアクセスするには distributor 経由で！
            var targetList = isPlayer1 ? distributor.player1Items : distributor.player2Items;

            if (targetList.Contains(item))
            {
                targetList.Remove(item);
                Debug.Log($"アイテム「{item.ItemName}」を削除しました。");
            }

            // ✅ distributorの関数を通じてUIを更新
            distributor.UpdateAllDisplays();
        });
    }


}

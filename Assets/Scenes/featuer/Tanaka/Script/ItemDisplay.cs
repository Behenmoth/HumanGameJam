using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    // ================================
    // 【ItemDistributorとの連携部分】
    // ================================

    // ItemDistributor（アイテム配布を管理するスクリプト）を参照
    // Distributor側のAwake()で自動的に設定される
    [HideInInspector]
    public ItemDistributor distributor;


    // ================================
    // 【Inspectorで設定する項目】
    // ================================

    [Header("生成するImageプレハブ（Imageコンポーネントを持つUIプレハブ）")]
    // → アイテム画像を表示するためのプレハブを指定
    // 　例：ImageオブジェクトのPrefabを設定
    public GameObject itemImagePrefab;

    [Header("生成先の親 (Canvas配下のTransform)")]
    // → 生成したImageを配置する親Transform
    // 　例：PanelやEmptyオブジェクトを指定
    public Transform itemParent;


    // ================================
    // 【表示対象プレイヤー設定】
    // ================================

    // このItemDisplayが「プレイヤー1用」なのか「プレイヤー2用」なのかを指定
    public enum PlayerTarget { Player1, Player2 }

    [Header("どのプレイヤーのアイテムを表示するか")]
    public PlayerTarget target = PlayerTarget.Player1;


    // ================================
    // 【Distributorからのセット用関数】
    // ================================

    /// <summary>
    /// ItemDistributor側から呼ばれて、このDisplayに関連付けを行う
    /// </summary>
    public void SetDistributor(ItemDistributor d)
    {
        distributor = d;
    }


    // ================================
    // 【アイテム表示更新処理】
    // ================================
    /// <summary>
    /// ItemDistributorのDistributeItems()内から呼ばれる
    /// プレイヤーに配布されたアイテムをUIに表示する
    /// </summary>
    public void UpdateItemDisplay()
    {
        // --- Distributorが設定されていない場合はエラーを出して中断 ---
        if (distributor == null)
        {
            Debug.LogError($"[{name}] distributor が設定されていません。", this);
            return;
        }

        // --- 表示対象のプレイヤーに応じて、配布されたアイテム配列を取得 ---
        ItemList[] items = (target == PlayerTarget.Player1)
            ? distributor.player1Items
            : distributor.player2Items;

        // --- アイテム配列がnullの場合 ---
        if (items == null)
        {
            Debug.LogWarning($"[{name}] 表示対象の items が null です。target={target}", this);
            return;
        }

        // --- itemParentが未設定（生成先が指定されていない）場合 ---
        if (itemParent == null)
        {
            Debug.LogError($"[{name}] itemParent が未設定です。Inspectorで設定してください。", this);
            return;
        }

        // --- 画像プレハブが未設定の場合 ---
        if (itemImagePrefab == null)
        {
            Debug.LogError($"[{name}] itemImagePrefab が未設定です。Inspectorで設定してください。", this);
            return;
        }


        // ================================
        // 【既存の子オブジェクトを削除】
        // ================================
        // すでに表示中のアイテム画像を一度消してから新しく作る
        for (int i = itemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }


        // ================================
        // 【アイテムごとにImageを生成して表示】
        // ================================
        for (int i = 0; i < items.Length; i++)
        {
            var itemData = items[i];

            // --- アイテムデータがnull（配られていない）場合はスキップ ---
            if (itemData == null)
            {
                Debug.LogWarning($"[{name}] items[{i}] が null です。", this);
                continue;
            }

            // --- プレハブを生成して親に配置 ---
            GameObject go = Instantiate(itemImagePrefab, itemParent);

            // --- Imageコンポーネントを取得してスプライトを設定 ---
            Image img = go.GetComponent<Image>();
            if (img != null)
            {
                // ItemList内の画像（ItemImage）をセット
                img.sprite = itemData.ItemImage;

                // デバッグ用にオブジェクト名を変更（例：Item_0_りんご）
                go.name = $"Item_{i}_{itemData.ItemName}";
            }
            else
            {
                // プレハブにImageが付いていない場合のエラー
                Debug.LogError($"[{name}] プレハブに Image コンポーネントがありません: {itemImagePrefab.name}", this);
            }
            AddClickToDestroy(go);
        }

        // --- 生成完了ログ（デバッグ用） ---
        Debug.Log($"[{name}] {items.Length} 件のアイテムを生成しました (target={target})", this);
    }
    // -------------------------------
    // 生成したオブジェクトにクリックで削除する処理を追加
    // -------------------------------
    private void AddClickToDestroy(GameObject obj)
    {
        // Button コンポーネントを付ける（なければ追加）
        Button btn = obj.GetComponent<Button>();
        if (btn == null)
            btn = obj.AddComponent<Button>();

        // クリック時にオブジェクトを削除
        btn.onClick.AddListener(() => Destroy(obj));
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ItemDistribution : MonoBehaviour
{
    public static ItemDistribution instance;

    [Header("全アイテムデータリスト")]
    public List<ItemData> allItems = new List<ItemData>();

    [Header("プレイヤーのインベントリー")]
    public PlayerInventry player1Inventry;
    public PlayerInventry player2Inventry;

    public void Awake()
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ラウンド開始時に2つアイテムを配る処理
    public void Distribution()
    {
        //アイテムがなかったらreturn
        if (allItems == null || allItems.Count == 0)
        {
            Debug.LogWarning("アイテムがありません");
            return;
        }

        //playerInventryが設定されていなかったらreturn
        if (player1Inventry == null || player2Inventry == null) 
        {
            Debug.LogWarning("プレイヤーインベントリーが設定されていません");
            return;
        }

        //アイテムをランダムに抽選
    }

    //抽選されたアイテムをListに加える処理
    private void GiveRandomItems(PlayerInventry inventry, int count)
    {
        //アイテムを配布する数だけ繰り返す
        for (int i = 0; i < count; i++)
        {
            //抽選されたアイテムを設定する
            ItemData selectedItem = GetRandomItem();

            if (selectedItem != null)
            {
                inventry.AddItems(selectedItem);
                Debug.Log($"{inventry.name}が{selectedItem.itemName}を入手しました");
            }
        }
    }

    //各アイテムに設定された確率でアイテムを抽選する処理
    private ItemData GetRandomItem()
    {
        float totalRate = 0f;
        foreach(var item in allItems)
        {
            totalRate += item.dropRate;
        }

        float randomValue = Random.value * totalRate;
        float cumulatible = 0f;

        foreach(var item in allItems)
        {
            cumulatible += item.dropRate;

            //ランダムで抽選されたアイテムを返す
            if (randomValue <= cumulatible)
            {
                return item;
            }
        }

        return allItems.Count > 0 ? allItems[Random.Range(0, allItems.Count)] : null;
    }

    //アイテムの表示・非表示を切り替えを呼び出す処理
    public void UpdateVisibleItems(GameManager.PlayerTurn currentTurn)
    {
        if (player1Inventry == null || player2Inventry == null)
        {
            return;
        }

        player1Inventry.SetActiveObjects(currentTurn == GameManager.PlayerTurn.Player1);
        player2Inventry.SetActiveObjects(currentTurn == GameManager.PlayerTurn.Player2);
    }
}

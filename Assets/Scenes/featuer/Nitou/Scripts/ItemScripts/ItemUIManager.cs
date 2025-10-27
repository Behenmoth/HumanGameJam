using UnityEngine;
using UnityEngine.UI;

public class ItemUIManager : MonoBehaviour
{
    public static ItemUIManager instance;

    [Header("UI全体")]
    public GameObject itemUI;

    [Header("UI制御")]
    public bool isUiOpen = false;

    [Header("アイテムカード")]
    public Image itemCard;

    private ItemData currentItem;
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

        itemUI.SetActive(false);
    }
    
    //アイテムカードを表示
    public void OnShowItemCards(ItemData item)
    {
        isUiOpen = true;
        currentItem = item;

        itemCard.sprite = item.itemCards;

        itemUI.SetActive(true);
    }

    //アイテムを使用
    public void UseItem()
    {
        GameManager.instance.canUseItems = false;

        if (currentItem == null) 
        {
            Debug.LogWarning("アイテムが選択されていません");
        }

        //効果発動前に現在のターンと、アイテムIDを保存
        GameManager.PlayerTurn currentTurn = GameManager.instance.currentPlayerTurn;

        int usedItemID = currentItem.itemID;

        //アイテムのIDに応じてswitch文で効果を発動する
        switch (currentItem.itemID)
        {
            //ビール使用時の効果
            case 0:
                Debug.Log("ビールを使用した");
                GameManager.instance.PassTurn();
                break;
            //手錠
            case 1:
                Debug.Log("手錠を使用した");
                GameManager.instance.SkipOpponentTurn();
                break;
            //注射
            case 2:
                Debug.Log("注射を使用した");
                injection.instance.OpenUI(currentTurn);
                break;
            //リモコン
            case 3:
                Debug.Log("リモコンを使用した");
                RemoteControl.instance.OpenUI();
            break;
            //ドライバー
            case 4:
                Debug.Log("ドライバーを使用した");
                BombManager.instance.HideBombCountForOpponent(currentTurn);
            break;
        }

        //使用したアイテムを削除
        RemoveUseItem(currentItem, currentTurn);

        //使用後にUIを閉じる
        OnClose();

        currentItem = null;

    }

    //使用したアイテムをプレイヤーのインベントリーから削除
    private void RemoveUseItem(ItemData usedItem, GameManager.PlayerTurn turn)
    {
        //プレイヤー1のターンだったら
        if (turn == GameManager.PlayerTurn.Player1)
        {
            GameManager.instance.player1Inventory.RemoveItem(usedItem);
        }
        //プレイヤー２のターンだったら
        else if(turn == GameManager.PlayerTurn.Player2)
        {
            GameManager.instance.player2Inventory.RemoveItem(usedItem);
        }
    }

    //UIを閉じる
    public void OnClose()
    {
        isUiOpen = false;
        currentItem = null;
        itemUI.SetActive(false);
    }
}

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
        Debug.Log($"{currentItem.itemName}を使用した");
    }

    //UIを閉じる
    public void OnClose()
    {
        isUiOpen = false;
        itemUI.SetActive(false);
    }
}

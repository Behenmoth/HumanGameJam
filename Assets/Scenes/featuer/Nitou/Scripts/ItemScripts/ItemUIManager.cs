using UnityEngine;
using UnityEngine.UI;

public class ItemUIManager : MonoBehaviour
{
    public static ItemUIManager instance;

    [Header("UI�S��")]
    public GameObject itemUI;

    [Header("UI����")]
    public bool isUiOpen = false;

    [Header("�A�C�e���J�[�h")]
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
    
    //�A�C�e���J�[�h��\��
    public void OnShowItemCards(ItemData item)
    {
        isUiOpen = true;
        currentItem = item;

        itemCard.sprite = item.itemCards;

        itemUI.SetActive(true);
    }

    //�A�C�e�����g�p
    public void UseItem()
    {
        Debug.Log($"{currentItem.itemName}���g�p����");
    }

    //UI�����
    public void OnClose()
    {
        isUiOpen = false;
        itemUI.SetActive(false);
    }
}

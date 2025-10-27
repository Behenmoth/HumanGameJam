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
        GameManager.instance.canUseItems = false;

        if (currentItem == null) 
        {
            Debug.LogWarning("�A�C�e�����I������Ă��܂���");
        }

        //���ʔ����O�Ɍ��݂̃^�[���ƁA�A�C�e��ID��ۑ�
        GameManager.PlayerTurn currentTurn = GameManager.instance.currentPlayerTurn;

        int usedItemID = currentItem.itemID;

        //�A�C�e����ID�ɉ�����switch���Ō��ʂ𔭓�����
        switch (currentItem.itemID)
        {
            //�r�[���g�p���̌���
            case 0:
                Debug.Log("�r�[�����g�p����");
                GameManager.instance.PassTurn();
                break;
            //���
            case 1:
                Debug.Log("������g�p����");
                GameManager.instance.SkipOpponentTurn();
                break;
            //����
            case 2:
                Debug.Log("���˂��g�p����");
                injection.instance.OpenUI(currentTurn);
                break;
            //�����R��
            case 3:
                Debug.Log("�����R�����g�p����");
                RemoteControl.instance.OpenUI();
            break;
            //�h���C�o�[
            case 4:
                Debug.Log("�h���C�o�[���g�p����");
                BombManager.instance.HideBombCountForOpponent(currentTurn);
            break;
        }

        //�g�p�����A�C�e�����폜
        RemoveUseItem(currentItem, currentTurn);

        //�g�p���UI�����
        OnClose();

        currentItem = null;

    }

    //�g�p�����A�C�e�����v���C���[�̃C���x���g���[����폜
    private void RemoveUseItem(ItemData usedItem, GameManager.PlayerTurn turn)
    {
        //�v���C���[1�̃^�[����������
        if (turn == GameManager.PlayerTurn.Player1)
        {
            GameManager.instance.player1Inventory.RemoveItem(usedItem);
        }
        //�v���C���[�Q�̃^�[����������
        else if(turn == GameManager.PlayerTurn.Player2)
        {
            GameManager.instance.player2Inventory.RemoveItem(usedItem);
        }
    }

    //UI�����
    public void OnClose()
    {
        isUiOpen = false;
        currentItem = null;
        itemUI.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class ItemClick : MonoBehaviour
{
    [Header("�N���b�N����p�J����")]
    public Camera mainCamera;

    public void Awake()
    {
        mainCamera = Camera.main;
    }

    //�N���b�N�����Ƃ��̏���
    public void OnClickItem(InputAction.CallbackContext callbackContext)
    {
        //�����ꂽ���̂ݎ��s����
        if (!callbackContext.performed)
        {
            return;
        }

        //�A�C�e��UI���o�Ă���Ƃ��͉������Ȃ�
        if (ItemUIManager.instance != null && ItemUIManager.instance.itemUI.activeSelf)
        {
            return;
        }

        //�N���b�N�����ꏊ��Ray���΂�
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Ray��������������s
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Item�^�O�����Ă���A�C�e����Ray��������������s
            if (hit.collider.CompareTag("Item"))
            {
               //�N���b�N�����A�C�e�����擾
               ItemObj itemObj=hit.collider.GetComponent<ItemObj>();
                if ((itemObj != null)) 
                {
                    ItemUIManager.instance.OnShowItemCards(itemObj.itemData);
                }
            }
        }
    }
}

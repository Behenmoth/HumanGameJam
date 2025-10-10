using UnityEngine;
using UnityEngine.InputSystem;

public class ItemClick : MonoBehaviour
{
    [Header("クリック判定用カメラ")]
    public Camera mainCamera;

    public void Awake()
    {
        mainCamera = Camera.main;
    }

    //クリックしたときの処理
    public void OnClickItem(InputAction.CallbackContext callbackContext)
    {
        //押された時のみ実行する
        if (!callbackContext.performed)
        {
            return;
        }

        //アイテムUIが出ているときは何もしない
        if (ItemUIManager.instance != null && ItemUIManager.instance.itemUI.activeSelf)
        {
            return;
        }

        //クリックした場所にRayを飛ばす
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Rayが当たったら実行
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //ItemタグがついているアイテムにRayが当たったら実行
            if (hit.collider.CompareTag("Item"))
            {
               //クリックしたアイテムを取得
               ItemObj itemObj=hit.collider.GetComponent<ItemObj>();
                if ((itemObj != null)) 
                {
                    ItemUIManager.instance.OnShowItemCards(itemObj.itemData);
                }
            }
        }
    }
}

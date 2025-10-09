using UnityEngine;
using UnityEngine.InputSystem;

public class ItemEntryKey : MonoBehaviour
{
    [SerializeField] private ItemRate itemRateSystemScript;
    [SerializeField] private ItemDistributor itemDistributor;
    [SerializeField] private bool isPlayer1 = true;
    [SerializeField] private ItemDisplay display;


    void Update()
    {
        // 新Input Systemの場合は Keyboard.current を使う
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log("Enterキー（Input System）が押されました！");
            itemRateSystemScript.conditionalaGetRandomItem(itemDistributor, isPlayer1);


            //display.SetPlayerTarget(ItemDisplay.PlayerTarget.Player2);
        }
    }
}

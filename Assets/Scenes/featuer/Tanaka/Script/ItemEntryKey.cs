using UnityEngine;
using UnityEngine.InputSystem;

public class ItemEntryKey : MonoBehaviour
{
    [SerializeField] private ItemRate itemRateSystemScript;
    [SerializeField] private ItemDistributor itemDistributor;
    [SerializeField] private bool isPlayer1 = true;

    void Update()
    {
        // �VInput System�̏ꍇ�� Keyboard.current ���g��
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log("Enter�L�[�iInput System�j��������܂����I");
            itemRateSystemScript.conditionalaGetRandomItem(itemDistributor, isPlayer1);
        }
    }
}

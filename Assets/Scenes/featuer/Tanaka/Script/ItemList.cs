using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "i000", menuName = "�A�C�e���쐬")]

public class ItemList : ScriptableObject
{
    //[InspectorLabel("�A�C�e���A�C�R��")]
    //public Sprite ItemIcon;

    [Header("�A�C�e��ID")]
    public int ItemID;

    [Header("�A�C�e����")]
    public string ItemName;

    [Header("�A�C�e���C���[�W")]
    public Sprite ItemImage;

    [Header("�A�C�e���I�u�W�F�N�g")]
    public GameObject ItemObject;

}

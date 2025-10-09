using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("�A�C�e����")]
    public string itemName;

    [Header("�A�C�e��ID")]
    public int itemID;

    [Header("���ʐ���")]
    [TextArea]
    public string description;

    [Header("�r�o��")]
    [Range(0f, 1f)]
    public float dropRate;

    [Header("��������I�u�W�F�N�g")]
    public GameObject spawnObj;

    [Header("�I�u�W�F�N�g�̊p�x")]
    public Vector3 spawnRotation;
}

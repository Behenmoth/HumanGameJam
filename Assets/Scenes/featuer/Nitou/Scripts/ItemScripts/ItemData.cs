using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("アイテム名")]
    public string itemName;

    [Header("アイテムID")]
    public int itemID;

    [Header("効果説明")]
    [TextArea]
    public string description;

    [Header("排出率")]
    [Range(0f, 1f)]
    public float dropRate;

    [Header("生成するオブジェクト")]
    public GameObject spawnObj;

    [Header("オブジェクトの角度")]
    public Vector3 spawnRotation;
}

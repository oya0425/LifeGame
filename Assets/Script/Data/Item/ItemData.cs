using UnityEngine;

[CreateAssetMenu(
    fileName ="ItemData",
    menuName ="LifeGame/ItemData"
)]

public class ItemData : ScriptableObject
{
    [Tooltip("アイテムの名前")] public string itemName;
    [Tooltip("アイテムの画像")] public Sprite itemImage;
    [TextArea][Tooltip("説明文")]public string itemDescription;
    [Tooltip("増加減少の倍率")]public float itemMultiplier;

}

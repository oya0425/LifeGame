using UnityEngine;

[CreateAssetMenu(
    fileName = "ItemData",
    menuName = "LifeGame/ItemData"
)]
public class ItemData : ScriptableObject
{
    [Tooltip("アイテムの名前")]
    public string itemName;

    [Tooltip("アイテムの画像")]
    public Sprite itemImage;

    [TextArea]
    [Tooltip("説明文")]
    public string itemDescription;

    [Tooltip("増加減少の倍率")]
    public Vector2 multiplierRange;


    public enum ItemEffectTiming
    {
        NowTurn,   // 使用した瞬間（そのターン中）
        NextTurn     // 次の自分のターン開始時
    }

    public float GetRandomMultiplier()
    {
        return Random.Range(multiplierRange.x, multiplierRange.y);
    }

    [Tooltip("効果の発動タイミング")]
    public ItemEffectTiming effectTiming;
}

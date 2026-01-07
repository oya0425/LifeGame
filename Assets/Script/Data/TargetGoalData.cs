using UnityEngine;

[CreateAssetMenu(
    fileName ="TargetGoalData",
    menuName ="LifeGame/Target Goal"
 )]

public class TargetGoalData : ScriptableObject
{
    [Header("•\¦—p")]
    public string title;

    [TextArea(2, 4)]
    public string description;

    [Header("‹àŠzƒŒƒ“ƒW")]
    public int minMoney;
    public int maxMoney;
}

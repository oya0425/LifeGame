using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// ゲーム内で使用するプレイヤー1人分のデータを保持する ScriptableObject。
/// 名前、所持金、現在位置、使用モデル、順番、色など、
/// プレイヤーに必要な情報をまとめて管理する。
/// </summary>
[CreateAssetMenu(fileName = "Player", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    /// <summary>プレイヤーの表示名。</summary>
    [Header("--- プレイヤーの名前 ---")]
    public  string  playerName;


    /// <summary>現在の所持金。</summary>
    [Header("--- 所持金 ---")]
    public int money;

    /// <summary>現在いるマスのインデックス番号。</summary>
    [Header("--- 現在のマス ---")]
    public int positionIndex;

    /// <summary>ゲーム内で使用するプレイヤーの見た目（Prefab）。</summary>
    [Header("--- プレイヤーのモデル ---")]
    public GameObject playerPrefab;

    /// <summary>行動順番号。ルーレットなどで決定される。</summary>
    [Header("--- 順番 ---")]
    public int number;

    /// <summary>プレイヤーの順番が確定したかどうか。</summary>
    public bool isOrderDecided;

    /// <summary>プレイヤーを判別するための色。</summary>
    [Header("--- 色 ---")]
    public Color playerColor;

    [Tooltip("ゴールしたかどうか？")]
    public bool isGoal;

    [Tooltip("目標金額")]
    public int targetMoney;

    [Tooltip("選ばれた目標データ")]
    public TargetGoalData targetGoalData;

    [Header("所持アイテム一覧"),Tooltip("所持アイテム一覧")]
    public List<ItemData> itemList = new List<ItemData>();
}

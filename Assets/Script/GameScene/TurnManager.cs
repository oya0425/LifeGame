using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// プレイヤーの行動順を管理するクラス。
/// ・プレイヤー一覧の管理
/// ・ターンの開始／終了
/// ・強制的なプレイヤー切替
/// </summary>

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    /// <summary>実際に動くプレイヤー（PlayerMover）のリスト</summary>
    public List<PlayerMover> players = new List<PlayerMover>();

    /// <summary>順番決めで確定したプレイヤー（PlayerData）のリスト</summary>
    public List<PlayerData> turnManager_players = new List<PlayerData>();

    /// <summary>現在行動中のプレイヤー番号（players の index）</summary>
    [SerializeField]private int currentPlayerIndex = -1;

    [Tooltip("全体のターン数")]
    public int allTurn = 30;
    [Tooltip("現在のターン数")]
    public int currentTurn = 1;

    public TurnTextUI turnTextUI;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // ※プレイヤーが揃ってから StartTurn を呼ぶ想定なので未使用
        // if (players.Count > 0) StartTurn();
    }


    /// <summary>
    /// 現在のプレイヤーのターンを開始する。
    /// </summary>
    public void StartTurn()
    {
        turnTextUI.UpdateTurnText(currentTurn, allTurn);
        Debug.Log("currentPlayerIndex = " + TurnManager.instance.currentPlayerIndex);
        players[currentPlayerIndex].isMyTurn = true;
        Debug.Log("数" + players.Count);
    }


    /// <summary>
    /// 現在のプレイヤーのターンを終了し、次のプレイヤーへ進める。
    /// </summary>
    public void EndTurn()
    {
        players[currentPlayerIndex].isMyTurn = false;

        // 次のプレイヤーへ移動
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;

            //１ターン終了　次のターンへ
            currentTurn++;
        }

        CheckTurnEnd();
    }

    private void CheckTurnEnd()
    {
        if (currentTurn > allTurn)
        {
            OnGameEnd();
            return;
        }

        StartTurn();
    }

    private void OnGameEnd()
    {
        Debug.Log("ゲーム終了");

        // リザルト表示、入力停止などをここに書く
    }

    /// <summary>
    /// PlayerMover をリストに登録する。
    /// 戻り値：登録された index（失敗時は -1）
    /// </summary>
    public int AddPlayer_PlayerMover(PlayerMover p)
    {
        if (p == null) return -1;
        players.Add(p);
        return players.Count - 1;
    }

    /// <summary>
    /// PlayerData を順番確定リストに登録する。
    /// 戻り値：登録された index（失敗時は -1）
    /// </summary>
    public int AddPlayer_PlayerData(PlayerData data)
    {
        if (data == null) return -1;
        turnManager_players.Add(data);
        return turnManager_players.Count - 1;
    }


    /// <summary>
    /// 強制で現在の行動プレイヤーを指定した index に変更する。
    /// </summary>
    public void SetCurrentPlayer(int index)
    {
        if (index < 0 || index >= players.Count) return;

        // 現在のプレイヤーをターン終了状態にする
        if (players.Count > 0) players[currentPlayerIndex].isMyTurn = false;

        // 新しいプレイヤーに切り替え
        currentPlayerIndex = index;
        players[currentPlayerIndex].isMyTurn = true;
    }


    /// <summary>
    /// 現在行動中の PlayerMover を返す。
    /// </summary>
    public PlayerMover GetCurrentPlayer()
    {
        if (players.Count == 0) return null;
        return players[currentPlayerIndex];
    }
    /// <summary>
    /// 今のターンの PlayerData を返す
    /// </summary>
    public PlayerData GetCurrentPlayerData()
    {
        if (turnManager_players.Count == 0) return null;
        return turnManager_players[currentPlayerIndex];
    }

    /// <summary>
    /// プレイヤーを全削除し、currentPlayerIndex をリセットする。
    /// </summary>
    public void ClearPlayers()
    {
        players.Clear();
        currentPlayerIndex = 0;
    }
}

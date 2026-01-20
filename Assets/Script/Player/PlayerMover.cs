using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのマス移動を管理するクラス
/// ・ルーレットの出目を受け取り移動
/// ・タイル（マス）を順に移動
/// ・ターン終了を TurnManager に通知
/// </summary>
public class PlayerMover : MonoBehaviour
{
    // --- 外部参照（シングルトン風） ---
    public static PlayerMover instance;

    [Header("移動中かどうか")]
    [SerializeField] private bool isMoving = false;
    [Tooltip("移動が終わったかどうか？")]
    public event Action OnMoveFinished;

    [Header("移動にかける時間（１マス）")]
    public float moveTime = 0.8f;

    // --- 現在止まっているマス番号 ---
    private int currentTileIndex = 0;

    [Header("マスの親オブジェクト")]
    public Transform tileParent;

    [Header("マス一覧（自動で取得）")]
    public List<Transform> tiles = new List<Transform>();

    [Header("出目を受け取るルーレット")]
    public DiceSpinner spinner;

    // --- 行動順 ---
    public bool isMyTurn = false;
    public int myIndex = -1;

    [Tooltip("ゴールしたかどうか？")]
    [SerializeField,Header("ゴールしたか？")] bool isReachedGoal = false;


    TileData tile;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeTiles();

        // 自分の行動順を登録
        myIndex = TurnManager.instance.AddPlayer_PlayerMover(this);

        // 出目が出たら MoveSteps を呼ぶ
        //spinner.OnSpinEnd += MoveSteps;
    }

    /// <summary>
    /// タイル（マス）を tileParent から取得してリストに登録
    /// </summary>
    private void InitializeTiles()
    {
        tiles.Clear();
        foreach (Transform child in tileParent)
        {
            tiles.Add(child);
        }
    }

    /// <summary>
    /// 出目を受け取り、ターン中なら移動開始
    /// </summary>
    public void MoveSteps(int steps)
    {
        //// ゲームがプレイモードでなければ無視
        //if (GameManager.instance.CurrentMode != GameManager.MODE.Move) return;

        //// 自分のターン & 動いていない → 移動開始
        //if (!isMoving && isMyTurn)
        //{
        //    StartCoroutine(MoveSmoothly(steps));
        //}
        if (isMoving) return;
        StartCoroutine(MoveSmoothly(steps));
    }

    /// <summary>
    /// 指定歩数だけタイルを順に移動するコルーチン
    /// </summary>
    private IEnumerator MoveSmoothly(int steps)
    {

        isMoving = true;

        // ===== マス移動 =====
        for (int i = 0; i < steps; i++)
        {
            int nextTileIndex = currentTileIndex+1;
            if (nextTileIndex >= tiles.Count)
            {
                //isReachedGoal = true;
                //PlayerData nowPlayerData = TurnManager.instance.GetCurrentPlayerData();
                //nowPlayerData.isGoal = isReachedGoal;
                nextTileIndex = 1;
                //break;
            }

            Vector3 start = transform.position;
            Vector3 target = tiles[nextTileIndex].position;
            float elapsed = 0f;

            while (elapsed < moveTime)
            {
                float t = elapsed / moveTime;
                transform.position = Vector3.Lerp(start, target, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            currentTileIndex = nextTileIndex;

        }

        // ===== ここから「同じマスの配置処理」 =====

        // 4分割オフセット
        Vector3[] offsets =
        {
             new Vector3(-10f, 0,  10f),
             new Vector3( 10f, 0,  10f),
             new Vector3(-10f, 0, -10f),
             new Vector3( 10f, 0, -10f),
        };

        // 使用中フラグ
        bool[] used = new bool[offsets.Length];

        // 他プレイヤーが使ってる位置を調べる
        foreach (var otherPlayer in TurnManager.instance.players)
        {
            if (otherPlayer == this) continue;
            if (otherPlayer.currentTileIndex != this.currentTileIndex) continue;

            // basePos は後で使うので一旦ここでは使わない
            Vector3 otherPos = otherPlayer.transform.position;

            for (int i = 0; i < offsets.Length; i++)
            {
                Vector3 expectedPos = tiles[currentTileIndex].position + offsets[i];

                // ほぼ同じ位置なら「使用中」
                if (Vector3.Distance(otherPos, expectedPos) < 0.1f)
                {
                    used[i] = true;
                    break;
                }
            }
        }

        // 空いてる席を探す
        int myIndex = 0;
        for (int i = 0; i < used.Length; i++)
        {
            if (!used[i])
            {
                myIndex = i;
                break;
            }
        }

        // 配置
        Vector3 basePos = tiles[currentTileIndex].position;
        transform.position = basePos + offsets[myIndex];

        tile = tiles[currentTileIndex].GetComponent<TileData>();
        //tile.DebugLog();

        isMoving = false;
        //TurnManager.instance.EndTurn();
        OnMoveFinished?.Invoke(); //?はNULLチェック
    }
    
    /// <summary>
    /// 現在地のマス
    /// </summary>
    /// <returns></returns>
    public TileData GetCurrentTile()
    {
        if(tile == null) return null;
        return tile;
    }

    /// <summary>
    /// 現在移動中かどうかを返す
    /// </summary>
    public bool GetIsMove()
    {
        return isMoving;
    }
}

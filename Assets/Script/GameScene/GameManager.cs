using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum eActionType { Roulette, Item, Map }

/// <summary>
/// ゲーム全体を管理するメインクラス
/// ・順番決め
/// ・ゲーム開始制御
/// ・DiceSpinner 結果受信
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    /// <summary>
    /// ゲーム進行モード
    /// </summary>
    public enum MODE
    {
        NONE,
        [Tooltip("順番決めフェーズ（ゲーム開始時のみ）")]
        SelectOrder,

        [Tooltip("行動選択ハブ（サイコロ・アイテム・マップ等を選ぶ）")]
        SelectAction,

        [Tooltip("アイテム画面（使用する・やめるを選択）")]
        Item,

        [Tooltip("サイコロ／ルーレットの実行フェーズ")]
        Dice,

        [Tooltip("プレイヤーの移動フェーズ")]
        Move,

        [Tooltip("停止マスのイベント処理フェーズ")]
        Event,

        [Tooltip("ターン終了処理（次のプレイヤーへ進む）")]
        EndTurn,

        [Tooltip("全員ゴール後の結果表示フェーズ")]
        Result,
    }

    [SerializeField] private MODE eMode=MODE.NONE;
    public MODE CurrentMode => eMode;

    PlayerData playerData;


    // --- 順番決めの変数 ---

    // 順番決定後に割り振られる内部番号の開始値
    private int outnumber = 7;

    // 出目が被って再抽選が必要なプレイヤー一覧
    public List<PlayerData> playersToReRoll = new List<PlayerData>();

    // 通常順番決め時の進行カウント
    [SerializeField] public int selectCount = 0;

    // 再抽選時の進行カウント
    public int rerollCount = 0;
    // ------------------------


    // --- 選択画面の変数 ---
    [SerializeField] private PlayerStatusView playerStatusUI;

    [Header("選択画面の親")]
    public GameObject selectActionView;
    [Tooltip("選択画面のモード")] public eActionType actionType;

    [Header("キャンセルボタン")]
    public GameObject backButton;

    private bool isRegistered = false;

    // ----------------------


    // --- ダイスの処理の変数 ---
    [Tooltip("ルーレットのオブジェクト")]
    [SerializeField] private GameObject diceView;
   
    [SerializeField,Header("turnManagerスクリプト")] TurnManager turnM;
    PlayerMover currentMover;

    float hideDiceTimer = 0f;
    bool waitingHideDice = false;

    // --------------------------

    // --- イベント処理の変数 ---
    [Tooltip("イベントが終わったかどうか?")]
    bool isEndEvent = false;

    // --------------------------


    // --- リザルト処理の変数 ---
    [Tooltip("リザルト画面UI")]
    [SerializeField] private ResultUI resultUI;
    bool isResult = false;
    // --------------------------


    #region Unity Lifecycle

    private void Awake()
    {
        // シングルトン初期化
        instance = this;
    }

    private void Start()
    {
        // --- 初期化 ---
        diceView.SetActive(false);
        HideBackButton();
        HideSelectActionView();

        playerStatusUI.Hide();
        resultUI.Hide();
        // --------------


        //eMode = MODE.SelectOrder;

        // DiceSpinner のイベント登録
        RegisterDiceEvent();

        // 最初は順番決めモード
        ChangeMode(MODE.SelectOrder);



    }

    [Tooltip("状態遷移")]
    void ChangeMode(MODE next)
    {
        if (eMode == next) return;
        switch (eMode)
        {
            case MODE.SelectOrder:
                break;
            case MODE.SelectAction:
                break;
            case MODE.Item:
                break;
            case MODE.Dice:
                break;
            case MODE.Move:
                break;
            case MODE.Event:
                break;
            case MODE.EndTurn:
                break;
            case MODE.Result:
                break;
        }


        switch (next)
        {
            case MODE.SelectOrder:
                OnSelectOrderStart();


                break;
            case MODE.SelectAction:
                ShowSelectActionView();
                ShowBackButton();
                OnSelectActionStart();
                break;
            case MODE.Dice:
                HideSelectActionView();
                OnDiceStart();
                break;

            case MODE.Move:
                HideBackButton();
                OnMoveStart();
                break;
            case MODE.Item:
                break;
            case MODE.Event:
                OnEventStart();
                break;
            case MODE.EndTurn:
                StartCoroutine(OnEndTurnStart());
                break;
            case MODE.Result:
                isResult = true;
                Debug.Log("Result通った");
                OnResultStart();
                break;
        }
        eMode = next;


    }


    void OnSelectOrderStart()
    {
        //ルーレットを表示
        diceView.SetActive(true);

        // 順番決め開始
        // PlayerManager の準備完了を待って順番決め開始
        StartCoroutine(InitPlayersAndSelectOrder());

        // → 完了したら ChangeMode(MODE.Dice);
    }
    #region 順番決め関数



    #region 初期化・イベント登録

    /// <summary>
    /// ダイス回転終了イベント登録
    /// ※ ここで一度だけ登録する（Updateでは絶対にやらない）
    /// </summary>
    private void RegisterDiceEvent()
    {
        DiceSpinner.instance.OnSpinEnd += ProcessOrderDecision;
        //PlayerMover.instance.isOnMoveFinished +=;
    }

    #endregion

    #region プレイヤー初期化待ち

    /// <summary>
    /// PlayerManager がプレイヤー生成を完了するまで待機
    /// </summary>
    private IEnumerator InitPlayersAndSelectOrder()
    {
        yield return new WaitUntil(IsPlayerManagerReady);
        StartCoroutine(SelectOrderCoroutine());
    }

    /// <summary>
    /// PlayerManager の準備完了判定
    /// </summary>
    private bool IsPlayerManagerReady()
    {
        return PlayerManager.instance != null &&
               PlayerManager.instance.playerObjects.Count > 0;
    }

    #endregion

    #region 順番決めメイン処理

    /// <summary>
    /// 順番決め全体の流れを管理するコルーチン
    /// </summary>
    private IEnumerator SelectOrderCoroutine()
    {
        var dataList = PlayerManager.instance.playerDataList;
        var moverList = GetAllPlayerMovers();

        // TurnManager の登録情報を初期化
        TurnManager.instance.ClearPlayers();

        // 全員の出目が揃い、かつ衝突解決が完了するまで待つ
        yield return new WaitUntil(() =>
        {
            if (!AreAllNumbersFilled(dataList)) return false;

            ResolveNumberConflicts(dataList);
            RegisterDecidedPlayers(dataList, moverList);
            PrepareReRoll(dataList);

            return AreAllOrdersDecided(dataList);
        });

        // 全員の順番が確定したらゲーム開始
        FinishOrderSelection();
    }

    /// <summary>
    /// 全プレイヤーの PlayerMover を取得
    /// </summary>
    private List<PlayerMover> GetAllPlayerMovers()
    {
        return PlayerManager.instance.playerObjects
            .ConvertAll(obj => obj.GetComponent<PlayerMover>());
    }

    #endregion

    #region 順番決め補助処理

    /// <summary>
    /// 全員が出目を振り終わったか確認
    /// </summary>
    private bool AreAllNumbersFilled(List<PlayerData> dataList)
    {
        foreach (var data in dataList)
            if (data.number == 0) return false;
        return true;
    }

    /// <summary>
    /// 出目が被っているプレイヤーを検出し、再抽選対象にする
    /// </summary>
    private void ResolveNumberConflicts(List<PlayerData> dataList)
    {
        bool[] hasConflict = new bool[dataList.Count];

        for (int i = 0; i < dataList.Count; i++)
        {
            for (int j = i + 1; j < dataList.Count; j++)
            {
                if (dataList[i].number == dataList[j].number)
                {
                    hasConflict[i] = true;
                    hasConflict[j] = true;
                }
            }
        }

        for (int i = 0; i < dataList.Count; i++)
        {
            if (hasConflict[i])
            {
                // 被ったプレイヤーは出目リセット
                dataList[i].number = 0;
                dataList[i].isOrderDecided = false;
            }
            else
            {
                // 問題なければ順番確定
                dataList[i].isOrderDecided = true;
            }
        }
    }

    /// <summary>
    /// 順番が確定したプレイヤーを TurnManager に登録
    /// </summary>
    private void RegisterDecidedPlayers(
        List<PlayerData> dataList,
        List<PlayerMover> moverList)
    {
        var decidedPlayers = dataList
            .Select((data, index) => (mover: moverList[index], data))
            .Where(p => p.data.isOrderDecided)
            .OrderByDescending(p => p.data.number)
            .ToArray();

        foreach (var pair in decidedPlayers)
        {
            if (TurnManager.instance.players.Contains(pair.mover)) continue;

            // 内部順番番号を割り当て
            pair.data.number = outnumber++;

            // TurnManager に登録
            TurnManager.instance.AddPlayer_PlayerMover(pair.mover);
            TurnManager.instance.AddPlayer_PlayerData(pair.data);

            // UI更新
            TurnUI.instance.UpdateAllResults();
        }
    }

    /// <summary>
    /// 再抽選対象プレイヤーを準備
    /// </summary>
    private void PrepareReRoll(List<PlayerData> dataList)
    {
        playersToReRoll.Clear();
        DiceSpinner.instance.rerollCount = 0;

        foreach (var data in dataList)
        {
            if (!data.isOrderDecided)
                playersToReRoll.Add(data);
        }

        if (playersToReRoll.Count > 0)
        {
            TurnUI.instance.ShowCurrentRoulettePlayer(
                playersToReRoll,
                DiceSpinner.instance.rerollCount);
        }
    }

    /// <summary>
    /// 全員の順番が確定したか確認
    /// </summary>
    private bool AreAllOrdersDecided(List<PlayerData> dataList)
    {
        foreach (var data in dataList)
            if (!data.isOrderDecided) return false;
        return true;
    }

    #endregion

    #region 順番決定後処理

    /// <summary>
    /// 順番決定完了後の後処理
    /// </summary>
    private void FinishOrderSelection()
    {
        TurnUI.instance.HideAllResultText();
        playersToReRoll.Clear();
        DiceSpinner.instance.rerollCount = -1;
        DiceSpinner.instance.OnSpinEnd -= ProcessOrderDecision;
        Debug.Log("全員登録完了。ゲーム開始");

        // ルーレットを回す処理へ移行
        ChangeMode(MODE.SelectAction);

        // ターン開始
        TurnManager.instance.StartTurn();
        playerData = TurnManager.instance.GetCurrentPlayerData();
        playerStatusUI.SetPlayer(playerData);

        playerStatusUI.Show();

    }

    #endregion

    #region Dice 結果処理

    /// <summary>
    /// ダイス回転終了時の処理振り分け
    /// </summary>
    public void ProcessOrderDecision(int number)
    {
        if (selectCount < TitleManager.playerCount)
        {
            ProcessNormalOrder(number);
            return;
        }

        ProcessReRoll(number);
    }

    /// <summary>
    /// 通常の順番決め処理
    /// </summary>
    private void ProcessNormalOrder(int number)
    {
        var player = PlayerManager.instance.playerDataList[selectCount];

        player.number = number;
        player.isOrderDecided = true;

        TurnUI.instance.ShowCurrentRoulettePlayer(selectCount + 1);
        TurnUI.instance.ShowResult(selectCount, number);

        selectCount++;
        DiceSpinner.instance.ResetNeedle();
    }

    /// <summary>
    /// 再抽選時の処理
    /// </summary>
    private void ProcessReRoll(int number)
    {
        if (playersToReRoll == null ||
            rerollCount >= playersToReRoll.Count) return;

        var player = playersToReRoll[rerollCount];

        player.number = number;
        player.isOrderDecided = true;

        TurnUI.instance.ShowCurrentRoulettePlayer(
            playersToReRoll,
            rerollCount + 1);

        TurnUI.instance.ShowResult(rerollCount, number);

        rerollCount++;
        DiceSpinner.instance.ResetNeedle();

    }

    #endregion

    #endregion

    #region 選択画面処理
    public eActionType GetActionType() { return actionType; }

    /// <summary>
    /// 選択画面の選択
    /// </summary>
    public void OnActionSelected(eActionType action)
    {
        actionType= action;
        switch (actionType)
        {
            case eActionType.Roulette:
                ChangeMode(MODE.Dice);
                ShowBackButtonUI();
                break;
            case eActionType.Item:
                break;
            case eActionType.Map:
                break;
        }
    }
    ///<sammary>戻るボタン（キャンセル）処理
    ///</sammary>>
    public void OnBackButton(eActionType action)
    {
        switch (action)
        {
            case eActionType.Roulette:
                ChangeMode(MODE.SelectAction);
                HideBackButtonUI();
                break;
            case eActionType.Item:
                ChangeMode(MODE.SelectAction);
                break;
            case eActionType.Map:
                ChangeMode(MODE.SelectAction);
                break;
        }
    }
    /// <summary>
    /// 選択画面の表示
    /// </summary> 
    public void ShowSelectActionView()
    {
        selectActionView.SetActive(true);
    }
    /// <summary>
    /// 選択画面の非表示
    /// </summary> 
    public void HideSelectActionView()
    {
        selectActionView.SetActive(false);
    }

    /// <summary>
    /// キャンセルボタンの表示
    /// </summary> 
    public void ShowBackButton()
    {
        backButton.SetActive(true);
    }
    /// <summary>
    /// キャンセルボタンの非表示
    /// </summary> 
    public void HideBackButton()
    {
        backButton.SetActive(false);

    }
    /// <summary>
    /// このUIが表示されている間に反応するイベントの登録／解除
    /// </summary>
    private void ShowBackButtonUI()
    {
        if (isRegistered) return;
        if (DiceSpinner.instance == null) return;

        DiceSpinner.instance.OnSpinStart += HideBackButton;
        isRegistered = true;
    }

    private void HideBackButtonUI()
    {
        if (!isRegistered) return;
        if (DiceSpinner.instance == null) return;

        DiceSpinner.instance.OnSpinStart -= HideBackButton;
        isRegistered = false;
    }
    private void OnSelectActionStart()
    {
        diceView.SetActive(false);
    }

    #endregion


    #region  ダイス操作処理
    void OnDiceStart()
    {
        // タイマー強制停止
        waitingHideDice = false;
        hideDiceTimer = 0f;

        //ルーレットを表示
        diceView.SetActive(true);

        // ルーレット開始
        DiceSpinner.instance.OnSpinEnd += OnDiceResult;
    }

    [Tooltip("ルーレットの結果を受け取って手番のプレイヤーを動かす")]
    void OnDiceResult(int step)
    {
        DiceSpinner.instance.OnSpinEnd -= OnDiceResult;
        currentMover = turnM.GetCurrentPlayer();
        currentMover.OnMoveFinished += OnMoveFinished;
        ChangeMode(MODE.Move);
        currentMover.MoveSteps(step);
    }
    
    void OnMoveFinished()
    {
        DiceSpinner.instance.ResetNeedle();
        currentMover.OnMoveFinished-= OnMoveFinished;
        ChangeMode(MODE.Event);
    }

    void OnHideDiceTimer()
    {
        // すぐ消さず、待ち状態に入る
        hideDiceTimer = 0f;
        waitingHideDice = true;

    }
    #endregion

    #region　イベント処理
    private void OnEventStart()
    {
        isEndEvent = false;
        OnHideDiceTimer();
        PlayerMover mover = currentMover;
        TileData tile=　mover.GetCurrentTile();
        PlayerData playerData = TurnManager.instance.GetCurrentPlayerData();
        diceView.SetActive(false);

        ProcessTileEvent(tile, playerData);
    }

    void ProcessTileEvent(TileData tile, PlayerData playerData)
    {
        TileMoneyCalculator calculator = new TileMoneyCalculator();
        int delta = 0;
        switch (tile.tileType)
        {
            case TileData.eTileType.NORMAL:
                delta = calculator.CalcMoneyDelta(tile);
                playerData.money += delta;

                tile.DebugLog();
                OnTileEventFinished();

                break;
            case TileData.eTileType.START:
                tile.DebugLog();
                break;
            case TileData.eTileType.EVENT:
                TileEvent tileEvent = new TileEvent();
                tileEvent.Execute(tile, OnTileEventFinished);
                tile.DebugLog();

                break;
            case TileData.eTileType.LUCKY:
                TileLucky tileLucky=new TileLucky();
                tileLucky.Execute(tile, OnTileEventFinished);
                tile.DebugLog();

                break;
            case TileData.eTileType.MINUS:
                delta = calculator.CalcMoneyDelta(tile);
                playerData.money -= delta;

                tile.DebugLog();
                OnTileEventFinished();
                break;
            case TileData.eTileType.BRANCH:
                tile.DebugLog();
                break;
            case TileData.eTileType.GOAL:
                //プレイヤーがゴールした
                playerData.isGoal = true;
                tile.DebugLog();
                OnTileEventFinished();

                break;
        }


    }
    /// <summary>
    /// EndTurnに遷移 </summary>
    void OnTileEventFinished()
    {
        //プレイヤーのステータスUIを更新
        playerStatusUI.SetPlayer(playerData);
        isEndEvent = true;

    }

    #endregion

    void OnMoveStart()
    {
        // 移動開始
        // → 移動完了イベント待ち
    }

    #region 次のターンの人に渡す

    private IEnumerator OnEndTurnStart()
    {
        bool allGoal = true;
        foreach (var player in TurnManager.instance.turnManager_players)
        {
            if (!player.isGoal)
            {
                allGoal = false;
                break;
            }
        }
        Debug.Log($"allGoal{allGoal}");
        if (allGoal)
        {
            ChangeMode(MODE.Result);
            yield break; // ← ここ超重要
        }

        turnM.EndTurn();
        playerData = TurnManager.instance.GetCurrentPlayerData();

        // UI 更新
        playerStatusUI.SetPlayer(playerData);
        playerStatusUI.Show();
        yield return new WaitForSeconds(1.5f);
        
        ChangeMode(MODE.SelectAction);
    }

    #endregion
    #region リザルト処理

    private void OnResultStart()
    {
        playerStatusUI.Hide();
        HideSelectActionView();
        isEndEvent = false;

        // プレイヤー一覧を取得
        List<PlayerData> players =
            new List<PlayerData>(TurnManager.instance.turnManager_players);

        // お金の量で降順ソート
        players.Sort((a, b) => b.money.CompareTo(a.money));

        resultUI.Show(players);
    }

    #endregion

    #endregion

    private void Update()
    {
        if (waitingHideDice)
        {
            hideDiceTimer += Time.deltaTime;

            if (hideDiceTimer >= 1.0f) // 1秒待つ
            {
                diceView.SetActive(false);
                waitingHideDice = false;
            }
        }

        // --- イベントが終わったらマウスクリックするのを待つ ---
        if (isEndEvent && eMode != MODE.Result)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isEndEvent = false;
                ChangeMode(MODE.EndTurn);
                
            }
        }

        if (isResult)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SceneManager.LoadScene("TitleScene");
            }

        }
    }


}

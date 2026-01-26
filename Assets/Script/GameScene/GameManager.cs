using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        [Tooltip("目標を決める")]
        TargetGoalSetting,

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

    [SerializeField,Header("フレームコントローラー")]
    public FrameColorController frameColorController;


    // --- 目標決定の変数 ---
    [SerializeField, Tooltip("目標のデータ所持")]
    TargetGoalManager targetGoalManager;




    // ----------------------


    // --- 順番決めの変数 ---
    [SerializeField, Header("再抽選などのテキスト")]
    Text setumei;
    [SerializeField, Header("順番決めの全体UI")]
    OrderSelectUI orderSelectUI;

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


    // --- アイテム処理の変数 ---
    [Tooltip("アイテムを使用できるかどうか")]
    public bool isItem;
    [SerializeField]private ItemUIController itemUIController;

    [SerializeField]private SelectActionViewUI selectActionViewUI;

    // --------------------------


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
    [SerializeField] 
    EventUIController eventUIController;
    TileEvent tileEvent;

    int currentMoney;

    [Tooltip("イベント終了時に出るテキスト")]
    [SerializeField]EventTextManager eventTextManager;

    // --------------------------

    // --- ラッキーマス処理の変数 ---
    [SerializeField]
    LuckyUIController luckyUIController;
    TileLucky tileLucky;

    // ------------------------------



    // --- リザルト処理の変数 ---
    [SerializeField] ResultManager resultManager;
    [Tooltip("リザルト画面UI")]
    [SerializeField] private ResultUI resultUI;
    bool isResult = false;
    [SerializeField] Button detailButton;

    [SerializeField] ResultDetailManager resultDetailManager;
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
        orderSelectUI.Hide();
        OnHideRankingFinished();
        resultDetailManager.Hide();
        eventTextManager.Hide();
        frameColorController.SetColor(Color.white);

        tileEvent = new TileEvent(eventUIController);
        tileLucky = new TileLucky(luckyUIController);
        currentMoney = 0;
        isItem = true;

        // --------------


        //eMode = MODE.SelectOrder;

        // DiceSpinner のイベント登録
        RegisterDiceEvent();

        // 最初は順番決めモード
        ChangeMode(MODE.NONE);
        //StartCoroutine(StartGameFlow());

        StartCoroutine(StartGame());
        StartCoroutine(StartGameFlow());

    }
    private IEnumerator StartGame()
    {
        yield return ScreenTransition.instance.PlayShrink();

    }

    IEnumerator StartGameFlow()
    {
        // 初期化がすべて終わるのを待つ
        yield return null;

        ChangeMode(MODE.TargetGoalSetting);
        
    }

    [Tooltip("状態遷移")]
    void ChangeMode(MODE next)
    {
        if (eMode == next) return;
        switch (eMode)
        {
            case MODE.TargetGoalSetting:
                break;
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
            case MODE.TargetGoalSetting:
                OnTargetGoalStart();
                break;

            case MODE.SelectOrder:
                OnSelectOrderStart();


                break;
            case MODE.SelectAction:
                selectActionViewUI.SetItemButtonInteractable(isItem);
                ShowSelectActionView();
                ShowBackButton();
                HideDiceView();
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
                HideSelectActionView();
                OnItemStart();
                break;
            case MODE.Event:
                OnEventStart();
                break;
            case MODE.EndTurn:
                TurnManager.instance.OnGameFinish -= OnEndTurnToResult;
                TurnManager.instance.OnGameFinish += OnEndTurnToResult;
                StartCoroutine(OnEndTurnStart());
                break;
            case MODE.Result:
                isResult = true;
                Debug.Log("Result通った");
                StartCoroutine(OnResultStart());
                break;
        }
        eMode = next;


    }

    #region 目標決定処理
    private void OnTargetGoalStart()
    {
        targetGoalManager.OnFinished -= OnTargetGoalFinish;
        targetGoalManager.OnFinished += OnTargetGoalFinish;
        targetGoalManager.StartSetting();
    }

    private void OnTargetGoalFinish()
    {
        targetGoalManager.OnFinished-= OnTargetGoalFinish;
        ChangeMode(MODE.SelectOrder);
    }


    #endregion


    void OnSelectOrderStart()
    {
        //ルーレットを表示
        diceView.SetActive(true);
        DiceSpinner.instance.SetOrderSelectPosition();
        //説明を表示
        orderSelectUI.Show();
        string colorCode = ColorUtility.ToHtmlStringRGB(PlayerManager.instance.playerDataList[0].playerColor);
        frameColorController.SetColor(PlayerManager.instance.playerDataList[0].playerColor);
        orderSelectUI.SetMessage(
            $"ルーレットを回そう！:" +
            $"<color=#{colorCode}>{PlayerManager.instance.playerDataList[0].playerName}</color>");


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
        while (true)
        {
            // 全員の出目が揃うのを待つ
            yield return new WaitUntil(() =>
                AreAllNumbersFilled(dataList)
            );
            // 出目かぶりをなくす
            ResolveNumberConflicts(dataList);
            RegisterDecidedPlayers(dataList, moverList);
            PrepareReRoll(dataList);

            // 再抽選が必要ならここで終了（次は StartReRoll から再開）
            if (playersToReRoll.Count > 0)
            {
                // UI更新
                TurnUI.instance.UpdateAllResults();
                string colorCode = ColorUtility.ToHtmlStringRGB(playersToReRoll[rerollCount].playerColor);
                frameColorController.SetColor(playersToReRoll[rerollCount].playerColor);

                orderSelectUI.SetMessage($"再抽選ルーレットを回そう！:<color=#{colorCode}>{playersToReRoll[rerollCount].playerName}</color>");

                StartReRoll();
                //再抽選時に全員がルーレットを回すまで待つ
                yield return new WaitUntil(() =>
                   AreReRollNumbersFilled(playersToReRoll)
                );

                continue;
            }
            //全員終了したら
            break;
        }
        frameColorController.SetColor(Color.white);
        orderSelectUI.SetMessage("順番決定！");

        yield return new WaitForSeconds(2f);
        orderSelectUI.Hide();

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
    /// 再抽選時に全員が出目を振り終わったか確認
    /// </summary>

    private bool AreReRollNumbersFilled(List<PlayerData> reRollPlayers)
    {
        foreach (var data in reRollPlayers)
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
        rerollCount = 0;

        foreach (var data in dataList)
        {
            if (!data.isOrderDecided)
                playersToReRoll.Add(data);
        }

        if (playersToReRoll.Count > 0)
        {
            TurnUI.instance.ShowCurrentRoulettePlayer(
                playersToReRoll,
                rerollCount);
        }
    }
    void StartReRoll()
    {
        // rerollCount をここでリセット
        rerollCount = 0;

        foreach (var data in playersToReRoll)
        {
            data.number = 0;
            data.isOrderDecided = false;
        }
        

        TurnUI.instance.ShowCurrentRoulettePlayer(
            playersToReRoll,
            rerollCount);
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
    /// 通常の順番決め処理(ルーレットを回して結果を入れる)
    /// </summary>
    private void ProcessNormalOrder(int number)
    {
        var player = PlayerManager.instance.playerDataList[selectCount];

        player.number = number;
        player.isOrderDecided = true;

        TurnUI.instance.ShowCurrentRoulettePlayer(selectCount + 1);
        TurnUI.instance.ShowResult(player, number);
        selectCount++;
        // 次のプレイヤー案内（まだ残っている場合のみ）
        if (selectCount < PlayerManager.instance.playerDataList.Count)
        {
            var nextPlayer =
                PlayerManager.instance.playerDataList[selectCount];
            string colorCode = ColorUtility.ToHtmlStringRGB(nextPlayer.playerColor);
            frameColorController.SetColor(nextPlayer.playerColor);

            orderSelectUI.SetMessage(
                $"ルーレットを回そう！:<color=#{colorCode}>{nextPlayer.playerName}</color>"
            );
        }

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
        //player.isOrderDecided = true;

        TurnUI.instance.ShowResult(player, number);
        

        
        TurnUI.instance.ShowCurrentRoulettePlayer(
            playersToReRoll,
            rerollCount + 1);

        rerollCount++;

        if (rerollCount < playersToReRoll.Count)
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(playersToReRoll[rerollCount].playerColor);
            frameColorController.SetColor(playersToReRoll[rerollCount].playerColor);

            orderSelectUI.SetMessage(
                $"再抽選ルーレットを回そう！:<color=#{colorCode}>{playersToReRoll[rerollCount].playerName}</color>"
            );
        }

        DiceSpinner.instance.ResetNeedle();

    }

    #endregion


    #region 順番決定後処理

    /// <summary>
    /// 順番決定完了後の後処理
    /// </summary>
    private void FinishOrderSelection()
    {
        TurnUI.instance.HideAllResultText();
        DiceSpinner.instance.SetDefaultPosition();

        playersToReRoll.Clear();
        rerollCount = -1;
        DiceSpinner.instance.OnSpinEnd -= ProcessOrderDecision;

        Debug.Log("全員登録完了。ゲーム開始");

        // ルーレットを回す処理へ移行
        ChangeMode(MODE.SelectAction);

        // ターン開始
        TurnManager.instance.StartTurn();
        playerData = TurnManager.instance.GetCurrentPlayerData();
        playerStatusUI.SetPlayer(playerData);
        frameColorController.SetColor(playerData.playerColor);
        playerStatusUI.Show();

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
                DiceSpinner.instance.ResetNeedle();
                ShowBackButtonUI();
                break;
            case eActionType.Item:
                ChangeMode(MODE.Item);

                ShowBackButtonUI();

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
                DiceSpinner.instance.SetDefaultPosition();
                HideBackButtonUI();
                break;
            case eActionType.Item:
                HideItemUI();
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
    /// <summary>
    /// ルーレット非表示 
    /// </summary> 
    private void HideDiceView()
    {
        diceView.SetActive(false);
    }

    #endregion

    #region アイテム画面処理
    private void OnItemStart()
    {
        itemUIController.OnChoiceSelected -= OnItemSelected;
        itemUIController.OnChoiceSelected += OnItemSelected;

        itemUIController.Show();
        string[] descriptions = new string[playerData.itemList.Count];

        for (int i = 0; i < playerData.itemList.Count; i++)
        {
            descriptions[i] = playerData.itemList[i].itemDescription;
        }

        itemUIController.SetChoices(playerData, descriptions);
    }

    /// <summary>
    /// どのボタンを押したか（どのアイテムを使用したか） 
    /// </summary>
    private void OnItemSelected(int index)
    {
        // 仮：使用ログ
        Debug.Log("Item Used : " + index);
        isItem = false;
        selectActionViewUI.SetItemButtonInteractable(isItem);

        playerData.itemList.RemoveAt(index);
        itemUIController.ClearDescription();
        HideItemUI();
        playerStatusUI.SetPlayer(playerData);

        ChangeMode(MODE.SelectAction);
    }

    private void HideItemUI()
    {
        itemUIController.Hide();
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

        currentMoney = playerData.money;

        switch (tile.tileType)
        {
            case TileData.eTileType.NORMAL:
                delta = calculator.CalcMoneyDelta(tile);

                playerData.money += delta;

                tile.DebugLog();
                OnEventText(currentMoney, playerData.money, delta);


                break;
            case TileData.eTileType.START:
                tile.DebugLog();
                break;
            case TileData.eTileType.EVENT:

                //tileEvent.Execute(tile, OnTileEventFinished);
                tileEvent.Execute(tile, OnEventFinished);

                tile.DebugLog();

                break;
            case TileData.eTileType.LUCKY:
                tileLucky.Execute(tile, OnLuckyEnd);

                tile.DebugLog();

                break;
            case TileData.eTileType.MINUS:
                delta = calculator.CalcMoneyDelta(tile);
                playerData.money -= delta;
                OnEventText(currentMoney,playerData.money,delta);

                tile.DebugLog();
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
    void OnEventFinished()
    {
        int delta = tileEvent.GetMoneyDelta();
        playerData.money += delta;

        OnEventText(currentMoney, playerData.money, delta);
    }

    /// <summary>
    /// お金の増減のテキスト表示 </summary>
    void OnEventText(int currentMoney,int newMoney,int delta)
    {
        eventTextManager.Show();
        eventTextManager.OnClicked -= OnEndEventText;
        eventTextManager.OnClicked += OnEndEventText;
        if (delta == 0)
        {
            eventTextManager.SetMessageText($"何も起きなかった\n"
                                    + "<align=right>クリックで次へ</align>");
            playerStatusUI.ChangeSetMoney(playerData.money);

        }
        else if (currentMoney < newMoney)
        {
            eventTextManager.SetMessageText($"{MyUtility.FormatEventMoneyManEn(delta)}もらった!\n"
                                                + "<align=right>クリックで次へ</align>");
            playerStatusUI.ChangeSetMoney(playerData.money);

        }
        else
        {
            eventTextManager.SetMessageText($"{MyUtility.FormatEventMoneyManEn(delta)}失った...\n"
                                                + "<align=right>クリックで次へ</align>");
            playerStatusUI.ChangeSetMoney(playerData.money);

        }

    }

    void OnLuckyEnd()
    {
        ItemData getItem = tileLucky.GetItem();
        bool canGetItem = playerData.itemList.Count < 3;

        if (canGetItem)
        {
            playerData.itemList.Add(getItem);
        }

        OnLuckyText(getItem.itemName, canGetItem);
    }
    void OnLuckyText(string itemName,bool canGetItem)
    {
        eventTextManager.Show();
        eventTextManager.OnClicked -= OnEndEventText;
        eventTextManager.OnClicked += OnEndEventText;
        if (!canGetItem)
        {
            eventTextManager.SetMessageText($"アイテムを獲得できなかった（所持品がいっぱいだ）\n"
                                            + "<align=right>クリックで次へ</align>");
        }
        else
        {
            eventTextManager.SetMessageText($"{playerData.playerName}は{itemName}を獲得した\n"
                                            + "<align=right>クリックで次へ</align>");
            playerStatusUI.SetPlayer(playerData);

        }

    }

    void OnEndEventText()
    {
        eventTextManager.OnClicked -= OnEndEventText;
        eventTextManager.Hide();
        OnTileEventFinished();
    }

    /// <summary>
    /// EndTurnに遷移 </summary>
    void OnTileEventFinished()
    {

        //プレイヤーのステータスUIを更新
        //playerStatusUI.SetPlayer(playerData);

        ChangeMode(MODE.EndTurn);

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
        if (allGoal||TurnManager.instance.bOnfinish)
        {

            ChangeMode(MODE.Result);
            yield break; // ← ここ超重要
        }
        yield return new WaitForSeconds(1.0f);

        turnM.EndTurn();
        if(TurnManager.instance.bOnfinish) yield break;
        isItem = true;

        playerData = TurnManager.instance.GetCurrentPlayerData();
        // UI 更新
        playerStatusUI.SetPlayer(playerData);
        playerStatusUI.Show();
        frameColorController.SetColor(playerData.playerColor);

        ChangeMode(MODE.SelectAction);
    }
    void OnEndTurnToResult()
    {
        ChangeMode(MODE.Result);
        HideSelectActionView();

    }

    #endregion
    #region リザルト処理

    private IEnumerator OnResultStart()
    {

        // ① 画面を黒で覆う
        yield return ScreenTransition.instance.PlayerExpandShrink();
        

        playerStatusUI.Hide();
        HideSelectActionView();
        isEndEvent = false;

        resultUI.OnRankingAnimationFinished += OnShowRankingFinished;

        // プレイヤー一覧を取得
        List<PlayerData> players =
            new List<PlayerData>(TurnManager.instance.turnManager_players);
        resultManager.CreateResultEntryList(players);
        resultUI.ShowRanking(
        resultManager.GetResultEntryList()
          );

        resultUI.Show();


        yield return ScreenTransition.instance.PlayShrink();

        //// お金の量で降順ソート
        //players.Sort((a, b) => b.money.CompareTo(a.money));

        //resultUI.Show(players);
    }
    /// <summary>
    /// 詳細ボタン 
    /// </summary>
    void OnShowRankingFinished()
    {
        detailButton.gameObject.SetActive(true);
    }
    void OnHideRankingFinished()
    {
        detailButton.gameObject.SetActive(false);
    }

    public void OnClickDetailButton()
    {
        detailButton.gameObject.SetActive(false);

        resultUI.Hide();

        // 詳細表示へ
        resultDetailManager.Show(
            PlayerManager.instance.playerDataList
        );
    }
    /// <summary>
    /// タイトルに戻るボタン  
    /// </summary>
    public void OnClickBackToTitle()
    {
        // リザルト系UIを全部閉じる
        //resultDetailManager.Hide();
        //resultUI.Hide();

        // タイトルへ
        SceneManager.LoadScene("TitleScene");
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
        //if (isEndEvent && eMode != MODE.Result)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        isEndEvent = false;
        //        ChangeMode(MODE.EndTurn);
                
        //    }
        //}

        if (isResult)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SceneManager.LoadScene("TitleScene");
            }

        }
    }


}

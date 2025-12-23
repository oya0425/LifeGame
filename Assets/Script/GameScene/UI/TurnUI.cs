using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnUI : MonoBehaviour
{
    public static TurnUI instance;

    /// <summary>プレイヤーの枠UI（Waku）のPrefab。</summary>
    [SerializeField] private GameObject playerWakuPrefab;

    /// <summary>生成したプレイヤー枠UIのリスト。</summary>
    [SerializeField] private List<GameObject> playerObjects = new List<GameObject>();

    /// <summary>TurnUI を生成する親オブジェクト。</summary>
    [SerializeField] private Transform TurnUIParent;

    /// <summary>説明文を表示するテキスト。</summary>
    public Text setumei;


    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// プレイヤーの生成完了 → UI生成 → 行動順決定 → UI並び替え  
    /// までの流れをコルーチンで自動処理する。
    /// </summary>
    void Start()
    {
        StartCoroutine(InitPlayersAndSelectOrder());
        StartCoroutine(TurnUIRoutine());
    }

    // --- このscript内で使用 ---

    // --- 順番決め ---

    /// <summary>
    /// プレイヤー生成完了を待ち、UI を作成し、  
    /// 行動順が決定されるまで待ってから UI を並び替える。
    /// </summary>
    private IEnumerator InitPlayersAndSelectOrder()
    {
        yield return new WaitUntil(() =>
            PlayerManager.instance != null &&
            PlayerManager.instance.playerObjects.Count > 0);

        SetupTurnUI();

        yield return new WaitUntil(() =>
            TurnManager.instance != null &&
            TurnManager.instance.players != null &&
            TurnManager.instance.players.Count ==
            PlayerManager.instance.playerObjects.Count);

        SortUIByTurnOrder();
    }


    /// <summary>
    /// プレイヤーUI枠を生成し、色・名前をセットする。
    /// 最初のルーレット可能者の名前も表示する。
    /// </summary>
    private void SetupTurnUI()
    {
        var dataList = PlayerManager.instance.playerDataList;

        for (int i = 0; i < dataList.Count; i++)
        {
            GameObject obj = Instantiate(playerWakuPrefab, TurnUIParent);

            Text text = obj.GetComponentInChildren<Text>();
            text.text = "Player" + (i + 1);

            obj.GetComponent<Image>().color = dataList[i].playerColor;

            var turnImage = obj.transform.Find("Turn").GetComponent<Image>();
            turnImage.gameObject.SetActive(PlayerMover.instance.isMyTurn);

            playerObjects.Add(obj);
        }

        ShowCurrentRoulettePlayer(DiceSpinner.instance.selectCount);
    }

    /// <summary>
    /// TurnManager が決めた行動順に合わせて  
    /// UI の並び順（Hierarchy の並び）を変更する。
    /// </summary>
    private void SortUIByTurnOrder()
    {
        var turnList = TurnManager.instance.players;

        for (int UIPos = 0; UIPos < turnList.Count; UIPos++)
        {
            PlayerMover mover = turnList[UIPos];
            int pIndex = mover.myIndex;

            GameObject uiObj = playerObjects[pIndex];
            uiObj.transform.SetSiblingIndex(UIPos);
        }
    }

    // --------------順番決め

    // --- ゲーム中 ---

    /// <summary>
    /// 現在行動中のプレイヤーだけ “YourTurn” を表示する。  
    /// 毎フレーム更新し続ける UI 管理コルーチン。
    /// </summary>
    IEnumerator TurnUIRoutine()
    {
        yield return new WaitUntil(() =>
            PlayerManager.instance.playerDataList.Count > 0 &&
            playerObjects.Count > 0);

        while (true)
        {
            var dataList = PlayerManager.instance.playerDataList;
            int count = Mathf.Min(dataList.Count, playerObjects.Count);

            for (int i = 0; i < count; i++)
            {
                GameObject obj = playerObjects[i];
                var turnImage = obj.transform.Find("Turn").GetComponent<Image>();

                bool isTurn =
                    PlayerManager.instance.playerObjects[i]
                        .GetComponent<PlayerMover>()
                        .isMyTurn;

                turnImage.gameObject.SetActive(isTurn);
            }

            yield return null;
        }
    }


    // ------------------ゲーム中 




    // ------------------このスクリプト内で使用




    // --- 外部で使用 ---

    // --- 順番決め ---

    /// <summary>
    /// ルーレットを回す順番のプレイヤー名を表示する（通常抽選）。
    /// </summary>
    public void ShowCurrentRoulettePlayer(int selectCount)
    {
        var playerList = PlayerManager.instance.playerDataList;

        if (selectCount >= playerList.Count)
        {
            setumei.text = "ルーレットを回せる人はいません";
            return;
        }

        var player = playerList[selectCount];
        setumei.text = $"ルーレットを回せるのは: {player.name}";
    }

    /// <summary>
    /// 再抽選時の “ルーレットを回すプレイヤー” を表示する。
    /// playersToReRoll から playerData を直接取得する。
    /// </summary>
    public void ShowCurrentRoulettePlayer(List<PlayerData> playersToReRoll, int rerollIndex)
    {
        if (rerollIndex >= playersToReRoll.Count)
        {
            setumei.text = "ルーレットを回せる人はいません";
            return;
        }

        var player = playersToReRoll[rerollIndex];
        setumei.text = $"ルーレットを回せるのは: {player.name}";
    }

    /// <summary>
    /// プレイヤーのルーレット結果（数字 or 完了）を表示する。
    /// </summary>
    public void ShowResult(int playerIndex, int num)
    {
        var obj = playerObjects[playerIndex];
        Text txt = obj.transform.Find("number").GetComponent<Text>();
        txt.gameObject.SetActive(true);

        var player = PlayerManager.instance.playerDataList[playerIndex];

        bool isAlreadyAdded = TurnManager.instance.turnManager_players.Contains(player);
        txt.text = isAlreadyAdded ? "[完了]" : num.ToString();
    }



    /// <summary>
    /// すでに決定済みのプレイヤーには「完了」を、  
    /// まだのプレイヤーには現在の number を表示する。
    /// </summary>
    public void UpdateAllResults()
    {
        var dataList = PlayerManager.instance.playerDataList;

        for (int i = 0; i < dataList.Count; i++)
        {
            var obj = playerObjects[i];
            Text txt = obj.transform.Find("number").GetComponent<Text>();

            var data = dataList[i];
            bool done = TurnManager.instance.turnManager_players.Contains(data);

            txt.text = done ? "完了" : data.number.ToString();
        }
    }


    /// <summary>
    /// 全プレイヤーの「数字／完了」表示を一度すべて非表示にする。
    /// </summary>
    public void HideAllResultText()
    {
        foreach (var obj in playerObjects)
        {
            var txt = obj.transform.Find("number").gameObject;
            txt.SetActive(false);
        }
    }



}

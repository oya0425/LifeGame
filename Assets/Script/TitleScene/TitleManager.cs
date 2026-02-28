using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// タイトル画面の管理クラス。
/// ・タイトル画面／人数選択画面の切り替え
/// ・プレイヤー枠の生成／削除
/// ・色の割り当て
/// ・GameScene への遷移
/// </summary>

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;

    // ------------------------------
    //    タイトルの状態
    // ------------------------------
    enum MODE
    {
        TITLE,        // タイトル画面
        SELECTPLAYER, // プレイヤー人数選択画面
    }

    [SerializeField] MODE titleMode;


    // ------------------------------
    //    背景
    // ------------------------------
    [SerializeField, Header("背景")]
    Sprite[] BackImg = new Sprite[2];  // 0=タイトル、1=プレイヤー選択
    SpriteRenderer myBG;
    Image myImgBackGround;

    [SerializeField] GameObject frameObj;


    // ------------------------------
    //    プレイヤー選択画面
    // ------------------------------
    public GameObject playerWakuPrefab;       // Player1～の枠Prefab
    public List<GameObject> playerObjects = new List<GameObject>();
    public Transform playerListParent;

    public static int playerCount = 0;        // 現在のプレイヤー数
    public const int PLAYERMAX = 4;
    public const int PLAYERMIN = 1;

    TextMeshProUGUI txtplayerName; // Player1 などのテキスト

    public static int allTurn;
    [SerializeField] TextMeshProUGUI allTurnText;

    private const int MIN_TURN = 5;
    private const int MAX_TURN = 30;
    private const int TURN_STEP = 5;

    //ゲームスタートの時に出る拡大されるやつ
    [SerializeField] private RectTransform gameStartText;

    // ------------------------------
    //    ボタン
    // ------------------------------
    public Button playerPlusButton;
    public Button playerMinusButton;
    public Button StartButton;

    //スタートボタンの位置
    public RectTransform titlePos_Button;
    public RectTransform selectPos_Button;

    //ターン設定ボタン
    public Button turnPlusButton;
    public Button turnMinusButton;


    // ------------------------------
    //    プレイヤーカラー
    // ------------------------------
    Color[] colors = new Color[8]
    {
        /*Color.red*/new Color(1.0f, 0.55f, 0.25f),/* Color.blue*/new Color(0.25f, 0.55f, 1.0f), Color.green, Color.yellow,
        Color.magenta, Color.cyan,
        new Color(1f,0.5f,0f),       // オレンジ
        new Color(0.5f,0f,1f)        // パープル
    };

    /// <summary>選択したプレイヤーの色一覧（GameSceneでも参照する）</summary>
    public static List<Color> playerColor = new List<Color>();

    [SerializeField] private AudioManager audioManager;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        myBG = GetComponent<SpriteRenderer>();
        myImgBackGround=GetComponent<Image>();
        playerCount = 0;
        playerColor.Clear();
        playerObjects.Clear();
        SetTitle();   // タイトル状態で初期化
        frameObj.SetActive(false);
        StartButton.transform.position=titlePos_Button.transform.position;
        gameStartText.gameObject.SetActive(false);

        allTurn = MIN_TURN;
        allTurnText.text = $"ターン数:{allTurn}";

        if (audioManager!= null)
        {
            audioManager.PlayBGM("TitleBGM");
        }

    }


    // ------------------------------
    //    ボタン ON/OFF
    // ------------------------------
    /// <summary>
    /// 現在のモードに応じてプラス/マイナスボタン
    /// の表示を切り替える。
    /// </summary>
    private void SetButton()
    {
        switch (titleMode)
        {
            case MODE.TITLE:
                playerPlusButton.gameObject.SetActive(false);
                playerMinusButton.gameObject.SetActive(false);
                turnPlusButton.gameObject.SetActive(false);
                turnMinusButton.gameObject.SetActive(false);
                
                allTurnText.gameObject.SetActive(false);
                break;

            case MODE.SELECTPLAYER:
                playerPlusButton.gameObject.SetActive(true);
                playerMinusButton.gameObject.SetActive(true);
                turnPlusButton.gameObject.SetActive(true);
                turnMinusButton.gameObject.SetActive(true);
                UpdateTurnButtonState();
                UpdatePlayerButtonState();
                
                allTurnText.gameObject.SetActive(true);
                
                break;
        }
    }


    // ------------------------------
    //    タイトル画面の「スタート」ボタン
    // ------------------------------
    /// <summary>
    /// タイトルの決定ボタン。
    /// ・タイトル：枠生成して人数選択へ
    /// ・選択画面：GameSceneへ遷移
    /// </summary>
    public void PushButton()
    {
        switch (titleMode)
        {
            // タイトル → 人数選択
            case MODE.TITLE:
                CreatePlayerFrame();
                SetSelect();
                StartButton.transform.position = selectPos_Button.transform.position;
                frameObj.SetActive(true);
                break;

            // 人数選択 → ゲーム開始
            case MODE.SELECTPLAYER:
                audioManager.PlaySE("StartSE");

                StartCoroutine(GameStartSeqience());
                //SceneManager.LoadScene("DebugScene");
                break;
        }
    }
    
    private IEnumerator GameStartSeqience()
    {
        //ボタン入力を止める
        StartButton.interactable = false;
        playerPlusButton.interactable = false;
        playerMinusButton.interactable=false;

        turnPlusButton.interactable = false;
        turnMinusButton.interactable = false;

        //GameStartを表示
        gameStartText.gameObject.SetActive(true);
        gameStartText.localScale = Vector3.zero;

        float time = 0;
        float duration = 0.5f;
        while (time < duration)
        {
            time += Time.deltaTime/3;
            float t=time/duration;
            gameStartText.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene("GameScene");
        audioManager.StopBGM();

    }

    // ------------------------------
    //    ＋ボタン：プレイヤー追加
    // ------------------------------
    public void PlayerPlusButton()
    {
        if (playerCount == PLAYERMAX) return;
        
        audioManager.PlaySE("DecisionSE");

        CreatePlayerFrame();
        UpdatePlayerButtonState();
    }


    // ------------------------------
    //    －ボタン：プレイヤー削除
    // ------------------------------
    public void PlayerMinusButton()
    {
        if (playerCount == PLAYERMIN) return;
        audioManager.PlaySE("DecisionSE");

        Destroy(playerObjects[playerObjects.Count - 1]);
        playerObjects.RemoveAt(playerObjects.Count - 1);

        playerColor.RemoveAt(playerCount - 1);

        playerCount--;
        UpdatePlayerButtonState();
    }
    private void UpdatePlayerButtonState()
    {
        playerPlusButton.interactable = (playerCount < PLAYERMAX);
        playerMinusButton.interactable = (playerCount > PLAYERMIN);
    }

    // ------------------------------
    //    共通：プレイヤー枠生成
    // ------------------------------
    /// <summary>
    /// Player1～の枠を生成し、色と名前をセット。
    /// PlusButton と PushButton から呼ばれる共通処理。
    /// </summary>
    private void CreatePlayerFrame()
    {
        GameObject obj = Instantiate(playerWakuPrefab, playerListParent);
        playerCount++;

        // Player1/Player2 などの表示
        TextMeshProUGUI txt = obj.GetComponentInChildren<TextMeshProUGUI>();
        txt.text = "Player" + playerCount;

        // プレイヤーカラーを枠に適用
        Color newColor = colors[playerCount - 1];
        obj.GetComponent<Image>().color = newColor;

        // 色リストに追加
        playerColor.Add(newColor);

        // プレイヤーオブジェクト一覧にも追加
        playerObjects.Add(obj);
    }

    // ------------------------------
    //    ターン追加
    // ------------------------------
    public void TurnPlusButton()
    {
        allTurn += TURN_STEP;

        if (allTurn > MAX_TURN)
        {
            allTurn = MAX_TURN;
        }

        allTurnText.text = $"ターン数:{allTurn}";
        UpdateTurnButtonState();
    }

    // ------------------------------
    //    ターン減少
    // ------------------------------
    public void TurnMinusButton()
    {
        allTurn -= TURN_STEP;
        if (allTurn < MIN_TURN)
        {
            allTurn = MIN_TURN;
        }
        allTurnText.text = $"ターン数:{allTurn}";

        UpdateTurnButtonState();
    }
    //ボタンを押せないようにする
    private void UpdateTurnButtonState()
    {
        turnPlusButton.interactable = (allTurn < MAX_TURN);
        turnMinusButton.interactable = (allTurn > MIN_TURN);
        audioManager.PlaySE("DecisionSE");

    }


    // ------------------------------
    //    画面状態変更
    // ------------------------------
    /// <summary>
    /// タイトル状態に移行
    /// </summary>
    private void SetTitle()
    {
        titleMode = MODE.TITLE;
        //myBG.sprite = BackImg[0];
        myImgBackGround.sprite = BackImg[0];
        SetButton();
    }

    /// <summary>
    /// 人数選択状態に移行<
    /// </summary>
    private void SetSelect()
    {
        titleMode = MODE.SELECTPLAYER;
        //myBG.sprite = BackImg[1];
        myImgBackGround.sprite = BackImg[1];
        SetButton();
    }
}

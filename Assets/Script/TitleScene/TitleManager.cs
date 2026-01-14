using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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


    // ------------------------------
    //    ボタン
    // ------------------------------
    public Button PlusButton;
    public Button MinusButton;


    // ------------------------------
    //    プレイヤーカラー
    // ------------------------------
    Color[] colors = new Color[8]
    {
        Color.red, Color.blue, Color.green, Color.yellow,
        Color.magenta, Color.cyan,
        new Color(1f,0.5f,0f),       // オレンジ
        new Color(0.5f,0f,1f)        // パープル
    };

    /// <summary>選択したプレイヤーの色一覧（GameSceneでも参照する）</summary>
    public static List<Color> playerColor = new List<Color>();


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        myBG = GetComponent<SpriteRenderer>();
        playerCount = 0;
        playerColor.Clear();
        playerObjects.Clear();
        SetTitle();   // タイトル状態で初期化
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
                PlusButton.gameObject.SetActive(false);
                MinusButton.gameObject.SetActive(false);
                break;

            case MODE.SELECTPLAYER:
                PlusButton.gameObject.SetActive(true);
                MinusButton.gameObject.SetActive(true);
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
                break;

            // 人数選択 → ゲーム開始
            case MODE.SELECTPLAYER:
                SceneManager.LoadScene("GameScene");
                //SceneManager.LoadScene("DebugScene");
                break;
        }
    }


    // ------------------------------
    //    ＋ボタン：プレイヤー追加
    // ------------------------------
    public void PlayerPlusButton()
    {
        if (playerCount == PLAYERMAX) return;

        CreatePlayerFrame();
    }


    // ------------------------------
    //    －ボタン：プレイヤー削除
    // ------------------------------
    public void PlayerMinusButton()
    {
        if (playerCount == PLAYERMIN) return;

        Destroy(playerObjects[playerObjects.Count - 1]);
        playerObjects.RemoveAt(playerObjects.Count - 1);

        playerColor.RemoveAt(playerCount - 1);

        playerCount--;
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
    //    画面状態変更
    // ------------------------------
    /// <summary>
    /// タイトル状態に移行
    /// </summary>
    private void SetTitle()
    {
        titleMode = MODE.TITLE;
        myBG.sprite = BackImg[0];
        SetButton();
    }

    /// <summary>
    /// 人数選択状態に移行<
    /// </summary>
    private void SetSelect()
    {
        titleMode = MODE.SELECTPLAYER;
        myBG.sprite = BackImg[1];
        SetButton();
    }
}

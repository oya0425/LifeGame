using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー生成と管理を行うマネージャー
/// ・ScriptableObject をコピーして個々のプレイヤーデータを作成
/// ・プレイヤーの実体（Prefab）を生成
/// ・PlayerMover や Camera に登録
/// </summary>

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("プレイヤーの元テンプレート（ScriptableObject）")]
    public List<PlayerData> allPlayerTemplates;

    [Header("生成されたプレイヤーデータ")]
    public List<PlayerData> playerDataList = new List<PlayerData>();

    [Header("スタート位置")]
    public Transform startTitle;



    [Header("タイル（マス）親")]
    public Transform tileParent;

    [Header("サイコロ（ルーレット）")]
    public DiceSpinner diceSpinner;

    [Header("実体として生成されたプレイヤー")]
    public List<GameObject> playerObjects = new List<GameObject>();


    [Header("オーディオマネージャー")]
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetPlayerCount(TitleManager.playerCount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetPlayerCount(TitleManager.playerCount);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetPlayerCount(3);
    }

    /// <summary>
    /// プレイヤー人数を変更して、新規生成し直す
    /// </summary>
    public void SetPlayerCount(int count)
    {
        ClearExistingPlayers();

        float offsetX = 10f;

        MoveCamera cam = Camera.main.GetComponent<MoveCamera>();
        if (cam != null) cam.players.Clear();

        for (int i = 0; i < count; i++)
        {
            PlayerData newData = CreatePlayerData(i);
            playerDataList.Add(newData);

            GameObject obj = CreatePlayerObject(newData, i * offsetX);
            playerObjects.Add(obj);

            RegisterPlayer(obj, newData, cam);
        }
    }

    /// <summary>
    /// 既存プレイヤーをすべて破棄し、リストを初期化
    /// </summary>
    private void ClearExistingPlayers()
    {
        foreach (var obj in playerObjects)
        {
            Destroy(obj);
        }
        playerObjects.Clear();
        playerDataList.Clear();
    }

    /// <summary>
    /// ScriptableObject テンプレートから新しい PlayerData を生成
    /// </summary>
    private PlayerData CreatePlayerData(int index)
    {
        PlayerData template = allPlayerTemplates[0];
        PlayerData newData = ScriptableObject.CreateInstance<PlayerData>();

        newData.playerName = "Player_" + (index + 1);
        newData.money = template.money;
        newData.positionIndex = template.positionIndex;
        newData.playerPrefab = template.playerPrefab;
        newData.number = template.number;
        newData.playerColor = TitleManager.playerColor[index];

        return newData;
    }

    /// <summary>
    /// プレイヤーの見た目（Prefab）を生成して返す
    /// </summary>
    private GameObject CreatePlayerObject(PlayerData data, float offsetX)
    {
        GameObject obj = Instantiate(
            data.playerPrefab,
            startTitle.position + new Vector3(offsetX, 5.0f, 0),
            Quaternion.identity
        );

        obj.name = data.playerName;

        // 色変更
        obj.transform.Find("Sphere").GetComponent<MeshRenderer>().material.color = data.playerColor;
        obj.transform.Find("Cone").GetComponent<MeshRenderer>().material.color = data.playerColor;

        return obj;
    }

    /// <summary>
    /// PlayerDataHolder, PlayerMover, Camera などに登録
    /// </summary>
    private void RegisterPlayer(GameObject obj, PlayerData data, MoveCamera cam)
    {
        PlayerDataHolder holder = obj.GetComponent<PlayerDataHolder>();
        holder.data = data;

        PlayerMover mover = obj.GetComponent<PlayerMover>();
        if (mover != null)
        {
            mover.tileParent = tileParent;
            mover.spinner = diceSpinner;
            mover.Setup(audioManager);

            // カメラ登録
            if (cam != null) cam.players.Add(mover);
        }
    }
}


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectActionViewUI : MonoBehaviour
{
    [Header("ルーレットボタン")]
    public Button buttonRouletteUI;
    [Header("アイテムボタン")]
    public Button buttonItemUI;
    [Header("マップボタン")]
    public Button buttonMapUI;
    [Header("キャンセルボタン")]
    public Button buttonBackUI;

    public TextMeshProUGUI rouletteText;
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI mapText;
    public TextMeshProUGUI backText;

    public GameManager gameManager;

    [SerializeField] private AudioManager audioManager;


    void Start()
    {
        //buttonRouletteUI.onClick.AddListener(() => gameManager.OnActionSelected(eActionType.Roulette));
        //buttonItemUI.onClick.AddListener(() => gameManager.OnActionSelected(eActionType.Item));
        //buttonMapUI.onClick.AddListener(() => gameManager.OnActionSelected(eActionType.Map));
        //buttonBackUI.onClick.AddListener(() => gameManager.OnBackButton(gameManager.GetActionType()));
        // ルーレットボタン
        buttonRouletteUI.onClick.AddListener(() => {
            audioManager.PlaySE("DecisionSE"); // 1. 音を鳴らす
            gameManager.OnActionSelected(eActionType.Roulette); // 2. ゲーム処理
        });

        // アイテムボタン
        buttonItemUI.onClick.AddListener(() => {
            audioManager.PlaySE("DecisionSE");
            gameManager.OnActionSelected(eActionType.Item);
        });

        // マップボタン
        buttonMapUI.onClick.AddListener(() => {
            audioManager.PlaySE("DecisionSE");
            gameManager.OnActionSelected(eActionType.Map);
        });

        // 戻るボタン（これだけ違う音にするのもアリ！）
        buttonBackUI.onClick.AddListener(() => {
            audioManager.PlaySE("CancelSE"); 
            gameManager.OnBackButton(gameManager.GetActionType());
        });
    }
    /// <summary>
    /// アイテムボタンの有効無効 
    /// </summary>
    public void SetItemButtonInteractable(bool value)
    {
        buttonItemUI.interactable = value;
    }


}

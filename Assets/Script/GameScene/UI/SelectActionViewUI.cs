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


    void Start()
    {
        buttonRouletteUI.onClick.AddListener(() => gameManager.OnActionSelected(eActionType.Roulette));
        buttonItemUI.onClick.AddListener(() => gameManager.OnActionSelected(eActionType.Item));
        buttonMapUI.onClick.AddListener(() => gameManager.OnActionSelected(eActionType.Map));
        buttonBackUI.onClick.AddListener(() => gameManager.OnBackButton(gameManager.GetActionType()));
    }
    private void Update()
    {
        
    }


}

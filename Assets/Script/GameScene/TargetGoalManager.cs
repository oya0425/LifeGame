using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;

public class TargetGoalManager : MonoBehaviour
{

    [SerializeField, Tooltip("目標う決定UI")]
    TargetSettingUIController targetUIController;


    [Header("目標データ一覧")]
    [SerializeField]
    TargetGoalData[] targetGoalsDatas;

    [Tooltip("選択待ちか？")]
    bool isWaitingForChoice;
    [Tooltip("目標の選んだ番号１～４")]
    int selectedIndex;


    bool isRunning = false;

    [Tooltip("結果表示後、クリック待ち")]
    bool isWaitingForConfirm;

    // --- 音 ---
    [SerializeField] private AudioManager audioManager;



    private void Start()
    {
        targetUIController.Hide();
    }


    /// <summary>
    /// 目標決定フェーズ開始(GameManagerで呼ぶ)
    /// </summary>
    public void StartSetting()
    {
        if (isRunning)
        {
            Debug.LogWarning("TargetGoalFlow はすでに実行中です");
            return;
        }
        targetUIController.Show();

        targetUIController.OnChoiceSelected -= OnChoiceSelected;
        targetUIController.OnChoiceSelected += OnChoiceSelected;
        StartCoroutine(TargetGoalFlow());
    }

    /// <summary>
    /// 全プレイヤー分の目標決定フロー</summary>
    IEnumerator TargetGoalFlow()
    {
        List<PlayerData> players = PlayerManager.instance.playerDataList;

        for (int i = 0; i < players.Count; i++)
        {
            PlayerData playerData = players[i];
            //4つランダムに選ぶ（目標の中から）
            TargetGoalData[] choices = GetRandomGoals(4);
            if (choices == null)
            {
                yield break; // フロー中断（クラッシュ回避）
            }
            string colorCode = ColorUtility.ToHtmlStringRGB(playerData.playerColor);
            GameManager.instance.frameColorController.SetColor(playerData.playerColor);

            //UI表示
            targetUIController.SetMessage(
                $"{playerData.playerName}は、目標を選択してください。");
            targetUIController.SetMessage(
            $"<color=#{colorCode}>{playerData.playerName}</color>は、目標を選択してください。");

            targetUIController.SetChoices(
            new string[]
            {
                choices[0].title,
                choices[1].title,
                choices[2].title,
                choices[3].title,
            },
            new string[] 
            {
                choices[0].description,
                choices[1].description,
                choices[2].description,   
                choices[3].description,
            }
            );
            

            //選択待ち
            isWaitingForChoice = true;
            selectedIndex = -1;

            //選択が決まるまで待つ
            while(isWaitingForChoice)
            {
                yield return null;
            }

            //結果反映
            TargetGoalData selected = choices[selectedIndex];
            int targetMoney = Random.Range(
                selected.minMoney,
                selected.maxMoney + 1
            );
            // 下三桁を切り捨て（10000円単位にする）
            string moneyText = MyUtility.FormatMoneyManEn(targetMoney);


            //選択肢は消す
            targetUIController.OnHoverExit();
            targetUIController.HideChoices();

            // --- 結果メッセージ表示 ---
            targetUIController.SetMessageClick(
                $"({selected.title})\n<color=#{colorCode}>{playerData.playerName}</color> の目標金額は {moneyText} です。\n" +
                "<align=right>クリックで次へ</align>"
            );
            

            playerData.targetGoalData = selected;
            playerData.targetMoney = targetMoney;

            // クリック待ち
            isWaitingForConfirm = true;
            // クリックされるまで待つ
            while (isWaitingForConfirm)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (audioManager != null)
                    {
                        audioManager.PlaySE("DecisionSE"); // 決定音の名前
                    }

                    isWaitingForConfirm = false;
                }
                yield return null;
            }
        }

        //全員が終わったら
        targetUIController.Hide();
        targetUIController.OnChoiceSelected -= OnChoiceSelected;

        //GameManagerに通知
        OnFinished?.Invoke();
    }

    /// <summary>
    /// UIから呼ばれる　 </summary>
    void OnChoiceSelected(int index)
    {
        if (!isWaitingForChoice) return;

        if (index < 0 || index >= 4)
        {
            Debug.LogWarning($"不正な選択 index:{index}");
            return;
        }
        selectedIndex = index;
        isWaitingForChoice = false;
    }

    /// <summary>
    /// ランダムで目標データを1つ取得
    /// </summary>
    public TargetGoalData GetRandomGoal()
    {

        if (targetGoalsDatas == null || targetGoalsDatas.Length == 0)
        {
            Debug.LogError("TargetGoalData が設定されていません");
            return null;
        }

        int index = Random.Range(0, targetGoalsDatas.Length);
        return targetGoalsDatas[index];
    }
    TargetGoalData[]GetRandomGoals(int count)
    {

        if (targetGoalsDatas == null || targetGoalsDatas.Length <count)
        {
            Debug.LogError("TargetGoalData が設定されていません");
            return null;
        }


        List<TargetGoalData>list=new List<TargetGoalData>(targetGoalsDatas);
        TargetGoalData[] result=new TargetGoalData[count];

        for(int i = 0; i < count; i++)
        {
            int r = Random.Range(0, list.Count);
            result[i] = list[r];
            list.RemoveAt(r);
        }

        return result;
    }
    public System.Action OnFinished;

    /// <summary>
    /// 指定した目標データから目標金額を生成
    /// </summary>
    public int GenerateTargetMoney(TargetGoalData goalData)
    {
        if (goalData == null)
        {
            Debug.LogError("TargetGoalData が null です");
            return 0;
        }

        return Random.Range(goalData.minMoney, goalData.maxMoney + 1);
    }

}

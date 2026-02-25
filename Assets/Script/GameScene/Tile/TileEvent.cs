using System;
using UnityEngine;

public class TileEvent
{
    System.Action onFinished;

    System.Action<int> onFinishedWithDelta;
    [Tooltip("マウスがクリックされたかどうか?")]
    bool isWaiting;

    int textIndex;
     EventData currentEvent;

    bool isChoosing;

    bool isShowingResult;

    EventUIController eventUI;

    int moneyDelta;

    public TileEvent(EventUIController ui)
    {
        this.eventUI = ui;

        //UIからの通知を登録
        // UI → TileEvent への通知
        Debug.Log($"TileEvent NEW : {GetHashCode()}");
        eventUI.OnTextClicked -= OnTextClicked;
        eventUI.OnChoiceASelected -= OnChoiceASelected;
        eventUI.OnChoiceBSelected -= OnChoiceBSelected;
        eventUI.OnTextClicked += OnTextClicked;
        eventUI.OnChoiceASelected += OnChoiceASelected;
        eventUI.OnChoiceBSelected += OnChoiceBSelected;

    }

    public void Execute(TileData tile, System.Action<int> onFinished)
    {
        this.onFinishedWithDelta = onFinished;
        
        currentEvent = EventDatabase.instance.GetRandomEvent();

        textIndex = 0;

        isChoosing = false;
        isShowingResult = false;
        isWaiting = false;

        Debug.Log($"EVENTイベントマス発生 index{tile.tileIndex}");
        eventUI.SetBackGround(currentEvent.backGround);
        eventUI.SetMainImage(currentEvent.mainImg);
        eventUI.SetEventNameText(currentEvent.eventName);
        eventUI.ShowWindow();
        eventUI.HideChoices();
        eventUI.SetEventText(currentEvent.texts[textIndex]);
    }

    void ShowText()
    {
        if (textIndex < currentEvent.texts.Length)
        {
            Debug.Log(currentEvent.texts[textIndex]);
            isWaiting = true;
        }
        else
        {
            ShowChoices();
        }
    }


    /// <summary>
    /// 選択肢表示
    /// </summary>
    void ShowChoices()
    {
        isChoosing = true;
        eventUI.ShowChoices(
            currentEvent.choiceAText,
            currentEvent.choiceBText
        );
    }
    /// <summary>
    /// テキスト枠クリック時
    /// </summary>
    void OnTextClicked()
    {
        if (isChoosing) return;

        if (isShowingResult)
        {
            EndEvent();
            return;
        }

        textIndex++;

        if (textIndex < currentEvent.texts.Length)
        {
            eventUI.SetEventText(currentEvent.texts[textIndex]);
        }
        else
        {
            ShowChoices();
            eventUI.SetresultAImage(currentEvent.resultAImage);
            eventUI.SetresultBImage(currentEvent.resultBImage);
        }
    }


    public void Update()
    {
        if (isWaiting && Input.GetMouseButtonDown(0))
        {
            isWaiting = false;
            textIndex++;
            ShowText();
        }

        if (isChoosing)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnChoiceASelected();
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                OnChoiceBSelected();
            }
        }
    }
    void OnChoiceASelected()
    {
        if (!isChoosing) return;
        moneyDelta = currentEvent.GetChoiceAMoney();

        eventUI.HideChoices();
        eventUI.SetEventText(currentEvent.resultAText);
        eventUI.SetMainImage(currentEvent.resultAImage);
        isChoosing = false;
        isShowingResult = true;
    }

    void OnChoiceBSelected()
    {
        if (!isChoosing) return;
        moneyDelta = currentEvent.GetChoiceBMoney();

        eventUI.HideChoices();
        eventUI.SetEventText(currentEvent.resultBText);
        eventUI.SetMainImage(currentEvent.resultBImage);

        isChoosing = false;
        isShowingResult = true;
    }

    /// <summary>
    /// イベント終了
    /// </summary>
    void EndEvent()
    {
        Debug.Log($"TileEvent END : {GetHashCode()}");
        eventUI.OnTextClicked -= OnTextClicked;
        eventUI.OnChoiceASelected -= OnChoiceASelected;
        eventUI.OnChoiceBSelected -= OnChoiceBSelected;
        //eventUI.HideAll();
        eventUI.FadeOut(() => {
            onFinished?.Invoke();
        });
        //onFinished?.Invoke();
        onFinishedWithDelta?.Invoke(moneyDelta);
    }

    public int GetMoneyDelta()
    {
        return moneyDelta;
    }
}

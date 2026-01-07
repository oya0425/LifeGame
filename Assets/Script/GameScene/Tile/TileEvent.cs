using UnityEngine;

public class TileEvent
{
    System.Action onFinished;
    [Tooltip("マウスがクリックされたかどうか?")]
    bool isWaiting;

    int textIndex;
    Event currentEvent;

    bool isChoosing;

    bool isShowingResult;

    EventUIController eventUI;

    public TileEvent(EventUIController ui)
    {
        this.eventUI = ui;

        //UIからの通知を登録
        // UI → TileEvent への通知
        eventUI.OnTextClicked += OnTextClicked;
        eventUI.OnChoiceASelected += OnChoiceASelected;
        eventUI.OnChoiceBSelected += OnChoiceBSelected;

    }

    public void Execute(TileData tile, System.Action onFinished)
    {
        this.onFinished = onFinished;
        currentEvent = EventDatabase.GetRandomEvent();

        textIndex = 0;

        isChoosing = false;
        isShowingResult = false;
        isWaiting = false;

        Debug.Log($"EVENTイベントマス発生 index{tile.tileIndex}");

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

        eventUI.HideChoices();
        eventUI.SetEventText(currentEvent.resultAText);

        isChoosing = false;
        isShowingResult = true;
    }

    void OnChoiceBSelected()
    {
        if (!isChoosing) return;

        eventUI.HideChoices();
        eventUI.SetEventText(currentEvent.resultBText);

        isChoosing = false;
        isShowingResult = true;
    }

    /// <summary>
    /// イベント終了
    /// </summary>
    void EndEvent()
    {
        eventUI.HideAll();
        onFinished?.Invoke();
    }
}

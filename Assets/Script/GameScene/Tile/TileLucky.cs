using UnityEngine;

public class TileLucky
{
    System.Action OnFinished;

    LuckyUIController luckyUI;
    ItemData currentItem;

    bool isShowing;

    public TileLucky(LuckyUIController ui)
    {
        this.luckyUI= ui;
        luckyUI.OnTextClicked += OnTextClicked;
    }



    public void Execute(TileData tile,System.Action onFinished)
    {
        this.OnFinished = onFinished;

        Debug.Log($"Luckyラッキーマス発生 index{tile.tileIndex}");

        currentItem = ItemDatabase.instance.GetRandomItem();

        //UIセット
        luckyUI.SetItemDiscriptionText(currentItem.itemDescription);
        luckyUI.SetResultText(currentItem.itemName);
        luckyUI.SetItemImage(currentItem.itemImage);
        luckyUI.Show();

        isShowing = true;
    }

    void OnTextClicked()
    {
        if (!isShowing) return;
        EndEvent();
    }

    void EndEvent()
    {
        isShowing = false;
        luckyUI.Hide();
        OnFinished?.Invoke();
    }
    public ItemData GetItem()
    {
        return currentItem;
    }
}

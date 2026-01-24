using UnityEngine;

public class TileLucky
{
    public ItemData Execute(TileData tile,System.Action onFinished)
    {
        Debug.Log($"Luckyラッキーマス発生 index{tile.tileIndex}");

        ItemData item = ItemDatabase.instance.GetRandomItem();

        //イベントの終了を通知
        onFinished?.Invoke();
        return item;
    }

}

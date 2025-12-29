using UnityEngine;

public class TileLucky
{
    public void Execute(TileData tile,System.Action onFinished)
    {
        Debug.Log($"Luckyラッキーマス発生 index{tile.tileIndex}");


        //イベントの終了を通知
        onFinished?.Invoke();
    }

}

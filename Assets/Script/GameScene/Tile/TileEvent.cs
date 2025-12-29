using UnityEngine;

public class TileEvent
{
    System.Action onFinished;
    [Tooltip("マウスがクリックされたかどうか?")]
    bool isWaiting;

    public void Execute(TileData tile, System.Action onFinished)
    {
        Debug.Log($"EVENTイベントマス発生 index{tile.tileIndex}");
        //イベントの終了を通知
        onFinished?.Invoke();

    }
    void Update()
    {
        if (!isWaiting) return;

        if (Input.GetMouseButtonDown(0))
        {
            isWaiting = false;
            onFinished?.Invoke();
        }
    }

}

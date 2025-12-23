using UnityEngine;

public class TileData : MonoBehaviour
{
    [Tooltip("マスの種類")]
    public enum eTileType
    {
        [Tooltip("スタートマス")]START,
        [Tooltip("通常マス")]NORMAL,
        [Tooltip("イベントマス")]EVENT,
        [Tooltip("ラッキーマス")]LUCKY,
        [Tooltip("マイナスマス")]MINUS,
        [Tooltip("分岐マス")]BRANCH,
        [Tooltip("ゴールマス")]GOAL,

    }

    [SerializeField, Header("マスの番号"), Tooltip("マスの番号")]
    public int tileIndex;

    [SerializeField, Header("マスの種類"), Tooltip("マスの種類")]
    public eTileType tileType;

    public void DebugLog()
    {
        switch (tileType)
        {
            case eTileType.START:
                Debug.Log($"[Tile]{tileIndex}:スタートマス");
                break;
            case eTileType.NORMAL:
                Debug.Log($"[Tile]{tileIndex}:通常マス");

                break;
            case eTileType.EVENT:
                Debug.Log($"[Tile] {tileIndex} : イベントマス");
                break;
            case eTileType.LUCKY:
                Debug.Log($"[Tile] {tileIndex} : ラッキーマス");
                break;
            case eTileType.MINUS:
                Debug.Log($"[Tile] {tileIndex} : マイナスマス");
                break;

            case eTileType.BRANCH:
                Debug.Log($"[Tile] {tileIndex} : 分岐マス");
                break;

            case eTileType.GOAL:
                Debug.Log($"[Tile] {tileIndex} : ゴール！");
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

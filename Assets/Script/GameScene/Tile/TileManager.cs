using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    
    public List<TileData> tiles = new List<TileData>();
    
    private void Awake()
    {
        instance = this;

        tiles.Clear();

        for(int i = 0; i < transform.childCount;i++)
        {
            TileData tile = transform.GetChild(i).GetComponent<TileData>();
            if(tile != null )
            {
                tile.tileIndex = i;
                tiles.Add(tile);
            }
        }
    }
}

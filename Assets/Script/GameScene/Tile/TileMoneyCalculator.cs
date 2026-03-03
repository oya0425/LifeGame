using UnityEngine;

public class TileMoneyCalculator
{
    const int minMoney = 100;

    public int CalcMoneyDelta(TileData tile)
    {
        //ƒ}ƒX‚ÌÅ‘å”‚ğæ‚é
        //int maxTileCount = TileManager.instance.tiles.Count;
        int maxTileCount = TurnManager.instance.GetAllTurn();

        //float progress = (float)tile.tileIndex / (maxTileCount - 1);
        float progress = ((float)TurnManager.instance.GetCurrentTurn()*2) / (maxTileCount - 1);

        //delta‘Œ¸—Ê
        int delta = Mathf.RoundToInt(minMoney * (1f + progress));
        return delta;
    }

}

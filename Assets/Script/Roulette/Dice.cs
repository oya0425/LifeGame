using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] public PlayerMover playerMover;
    [SerializeField] public Transform tileParent;
    [SerializeField] public List<Transform> tiles = new List<Transform>();

    [SerializeField] DiceSpinner spinner; // ← これをInspectorで設定！

    public int currentTileIndex = 0;

    void Start()
    {
        tiles.Clear();
        foreach (Transform child in tileParent)
        {
            tiles.Add(child);
        }

        // 出目を受け取るイベント登録（重要！）
        spinner.OnSpinEnd += OnDiceResult;
    }

    void OnDiceResult(int result)
    {
        Debug.Log("Diceで受け取った目: " + result);

        int steps = result;
        int targetIndex = currentTileIndex + steps;

        if (targetIndex >= tiles.Count)
        {
            targetIndex = tiles.Count - 1;
            Debug.LogWarning("移動先のマスが存在しません");
        }

        if (targetIndex >= 0 && targetIndex < tiles.Count)
        {
            playerMover.MoveSteps(steps);
            currentTileIndex = targetIndex;
        }
    }
}

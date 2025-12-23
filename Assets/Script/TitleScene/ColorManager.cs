using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{

    // --- 色 ---
    Color[] colors = new Color[8]
    { Color.red, Color.blue, Color.green, Color.yellow,
       Color.magenta, Color.cyan, new Color(1f,0.5f,0f), new Color(0.5f,0f,1f) };

    int[] usedColorIndex;   //使っている色の保存先

    public Image[] imgPlayerFrames; //枠のImage(プレイヤー分)
    //------------------
    int playerCount=TitleManager.playerCount;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        usedColorIndex = new int[playerCount];
        // --- 初期化 ---
        for(int i = 0; i < playerCount; ++i)
        {
            usedColorIndex[i] = i;               //最初は順番に入れる
            imgPlayerFrames[i].color = colors[i];//色反映
        }
    }
    
    // --- 枠を押したときの色変更(TitleManagerで呼ぶ) ---
    public void ChangeColor(int playerIndex)
    {
        // --- 今使ってる色を返却 ---
        int current=usedColorIndex[playerIndex];
        usedColorIndex[playerIndex] = -1;

        // --- 次に使う色を探す ---
        int next=FindNextColor(current);
        imgPlayerFrames[playerIndex].color=colors[next];
    }


    // --- 空いてる色を探す ---
    int FindNextColor(int start)
    {
        int index = start;
        while (true)
        {
            // --- ループで0 1 2...7 0で回す
            index =(index+1)%colors.Length;

            // --- 使用可能であれば ---
            if (!IsColorUsed(index))
            {
                return index;
            }
        }
    }

    // --- その色が使われているかどうか ---
    bool IsColorUsed(int colorIndex)
    {
        for(int i = 0; i < playerCount; ++i)
        {
            if(usedColorIndex[i] == colorIndex)
            {
                return true;
            }
        }
        return false;
    }
}

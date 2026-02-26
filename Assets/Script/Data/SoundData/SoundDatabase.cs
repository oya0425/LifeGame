using System.Collections.Generic;
using UnityEngine;

public class SoundDatabase : MonoBehaviour
{
    public List<Sounddata> seList;
    public List<Sounddata> bgmList;
    // 名前で検索してClipを返すメソッド
    public Sounddata GetSound(string name)
    {
        return seList.Find(s => s.soundName == name);
    }
    public Sounddata GetBGM(string name)
    {
        return bgmList.Find(s => s.soundName == name);
    }

}

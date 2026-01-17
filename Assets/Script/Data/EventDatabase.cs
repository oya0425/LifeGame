using UnityEngine;

public class EventDatabase : MonoBehaviour
{
    [SerializeField] private EventData[] events;
    public static EventDatabase instance;
    private void Awake()
    {
        instance = this;
    }

    //    = new EventData[]
    //{
    //    new EventData
    //    {
    //        texts = new string[]
    //        {
    //            "今日はなんだか眠い。",
    //            "授業中、強烈な睡魔が襲ってきた。"
    //        },
    //        choiceAText = "寝る",
    //        choiceBText = "耐える",
    //        resultAText = "爆睡した。先生に注意された。",
    //        resultBText = "眠気に勝った。ちょっと成長した気がする。",

    //        choiceAMinMoney = -30,
    //        choiceAMaxMoney = -10,

    //         choiceBMinMoney = 10,
    //         choiceBMaxMoney = 30
    //    },

    //    new EventData
    //    {
    //        texts = new string[]
    //        {
    //            "コンビニに立ち寄った。",
    //            "新作スイーツが目に入る。"
    //        },
    //        choiceAText = "買う",
    //        choiceBText = "我慢する",
    //        resultAText = "美味しかったが出費が痛い。",
    //        resultBText = "財布は守られた。",
    //                    choiceAMinMoney = -30,
    //        choiceAMaxMoney = -10,

    //         choiceBMinMoney = 10,
    //         choiceBMaxMoney = 30

    //    },

    //    new EventData
    //    {
    //        texts = new string[]
    //        {
    //            "帰り道、雨が降ってきた。",
    //            "傘を持っていない。"
    //        },
    //        choiceAText = "走る",
    //        choiceBText = "濡れて帰る",
    //        resultAText = "転んだ。最悪だ。",
    //        resultBText = "びしょ濡れだが無事帰宅。",
    //                    choiceAMinMoney = -30,
    //        choiceAMaxMoney = -10,

    //         choiceBMinMoney = 10,
    //         choiceBMaxMoney = 30

    //    },

    //    new EventData
    //    {
    //        texts = new string[]
    //        {
    //            "スマホの通知が鳴った。",
    //            "昔の友人からだ。"
    //        },
    //        choiceAText = "返信する",
    //        choiceBText = "無視する",
    //        resultAText = "少し懐かしい気持ちになった。",
    //        resultBText = "特に何も起きなかった。",
    //                    choiceAMinMoney = -30,
    //        choiceAMaxMoney = -10,

    //         choiceBMinMoney = 10,
    //         choiceBMaxMoney = 30

    //    },

    //    new EventData
    //    {
    //        texts = new string[]
    //        {
    //            "自販機の前で立ち止まった。",
    //            "喉が渇いている。"
    //        },
    //        choiceAText = "ジュースを買う",
    //        choiceBText = "水道水で我慢",
    //        resultAText = "甘くて満足した。",
    //        resultBText = "節約できた。",
    //                    choiceAMinMoney = -30,
    //        choiceAMaxMoney = -10,

    //         choiceBMinMoney = 10,
    //         choiceBMaxMoney = 30

    //    },

    // 以下同じ形式で5個追加可能
    //};

    public EventData GetRandomEvent()
    {
        return events[Random.Range(0, events.Length)];
    }
}

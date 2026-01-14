using UnityEngine;
using System.Collections;
public static class MyUtility
{
    /// <summary>
    /// 数値の切り捨て（下三桁捨てなど）
    /// </summary>
    public static int FloorByUnit(int value)
    {
        return (value / 10000) * 10000;
    }

    /// <summary>
    /// 表示を単位に合わせて変更 </summary>
    public static string FormatMoneyManEn(int man)
    {
        bool isMinus = man < 0;
        int absMan = Mathf.Abs(man);

        int oku = absMan / 10000;
        int restMan = absMan % 10000;

        string result;

        if (oku > 0 && restMan > 0)
            result = $"{oku} 億 {restMan} 万円";
        else if (oku > 0)
            result = $"{oku} 億円";
        else
            result = $"{absMan} 万円";

        // マイナスなら先頭に − を付ける
        return isMinus ? $"−{result}" : result;
    }


    public static IEnumerator AnimateMoney(
    int fromMoney,
    int toMoney,
    float duration,
    System.Action<int> onValueChanged
)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            int current = Mathf.RoundToInt(Mathf.Lerp(fromMoney, toMoney, t));
            onValueChanged?.Invoke(current);

            yield return null;
        }

        // 最終値を保証
        onValueChanged?.Invoke(toMoney);
    }

}

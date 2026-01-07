using UnityEngine;

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
        int oku = man / 10000;
        int restMan = man % 10000;

        if (oku > 0 && restMan > 0)
            return $"{oku} 億 {restMan} 万円";
        else if (oku > 0)
            return $"{oku} 億円";
        else
            return $"{man} 万円";
    }


}

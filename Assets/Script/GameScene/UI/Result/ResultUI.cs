using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField, Header("リザルトUIの親")]
    GameObject window;

    [SerializeField, Header("ランキングの枠の親")]
    GameObject window_ins;

    [SerializeField, Header("ランキングの枠1位")]
    GameObject Rank1stPrefab;
    [SerializeField, Header("ランキングの枠2位")]
    GameObject Rank2ndPrefab;
    [SerializeField, Header("ランキングの枠3位以下")]
    GameObject RankOtherPrefab;

    [SerializeField, Header("生成位置（順位順）")]
    List<RectTransform> rankStartPosList;

    [SerializeField, Header("移動位置（順位順）")]
    List<RectTransform> rankTargetPosList;

    [Tooltip("生成したランキング枠の位置の配列")]
    List<RectTransform> rankingItemRects = new List<RectTransform>();

    public System.Action OnRankingAnimationFinished;

    public void Hide()
    {
        window.SetActive(false);
    }
    public void Show()
    {
        window.SetActive(true);
    }


    public void ShowRanking(List<ResultEntryData> resultList)
    {
        window_ins.SetActive(true);
        foreach (Transform child in window_ins.transform)
        {
            Destroy(child.gameObject);
        }
        rankingItemRects.Clear();

        foreach (var entry in resultList)
        {
            int index = entry.rank - 1;
            GameObject prefab = GetPrefabByRank(entry.rank);
            RectTransform StartPos = rankStartPosList[index];

            GameObject item = Instantiate(prefab, window_ins.transform);
            RectTransform itemRect = item.GetComponent<RectTransform>();

            itemRect.anchoredPosition = StartPos.anchoredPosition;

            RankingItemUI rankingItemUI = item.GetComponent<RankingItemUI>();
            rankingItemUI.Setup(entry.rank, entry.playerName, entry.money);

            //位置を保存
            rankingItemRects.Add(itemRect);
        }
        StartCoroutine(MoveRankingItems());
    }

    /// <summary>
    /// 順位に合わせた枠を設定 </summary>
    GameObject GetPrefabByRank(int rank)
    {
        if (rank == 1) return Rank1stPrefab;
        if (rank == 2) return Rank2ndPrefab;
        return RankOtherPrefab;
    }
    
    IEnumerator MoveRankingItems()
    {
        for(int i=rankingItemRects.Count-1; i >= 0; i--)
        {
            RectTransform item = rankingItemRects[i];
            RectTransform targetPos = rankTargetPosList[i];

            yield return StartCoroutine(
                MoveToPosition(item, targetPos.anchoredPosition, 1.4f)
                );
            yield return new WaitForSeconds(0.2f);
        }

        //演出完了通知
        OnRankingAnimationFinished?.Invoke();
    }

    IEnumerator MoveToPosition(
        RectTransform item,
        Vector2 targetPos,
        float duration)
    {
        Vector2 startPos = item.anchoredPosition;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime*2;
            float t = time / duration;
            item.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        item.anchoredPosition = targetPos;

    }

}

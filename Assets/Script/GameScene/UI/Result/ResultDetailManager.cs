using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class ResultDetailManager : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] RectTransform contentParent;
    [SerializeField] GameObject detailEntryPrefab;

    public void Show(List<PlayerData> playerDatas)
    {
        window.SetActive(true);

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in playerDatas)
        {
            GameObject obj = Instantiate(detailEntryPrefab, contentParent);
            ResultSummaryUI ui = obj.GetComponent<ResultSummaryUI>();
            ui.Setup(player);
        }
    }
    public void Hide()
    {
        window?.SetActive(false);
    }
    
}

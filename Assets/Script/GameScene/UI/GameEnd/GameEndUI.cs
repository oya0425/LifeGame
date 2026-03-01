using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameEndUI : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] TextMeshProUGUI messageText;

    private void Start()
    {
        Hide(); 
    }

    public void Show()
    {
        window.SetActive(true);
        // 表示された瞬間に演出を開始
        StartCoroutine(GameEndRoutine());
    }

    private IEnumerator GameEndRoutine()
    {
        // 1. 最初のメッセージ
        messageText.text = "全ターン終了！";

        // 16秒待機
        yield return new WaitForSeconds(14.0f);

        // 2. 次のメッセージ
        messageText.text = "結果発表！！";

    }

    public void Hide()
    {
        window.SetActive(false);
        StopAllCoroutines(); // 閉じる時は演出も止める
    }

}

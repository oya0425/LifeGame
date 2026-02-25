using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class MapEventEffect : MonoBehaviour
{
    [SerializeField] GameObject window;

    [Header("表示用(実体)")]
    [SerializeField] Image tileImageMain; // 中央に表示されるマスの画像
    [SerializeField] Image bgImageMain;   // 背後に表示される帯などの画像

    [Header("リソース用（0:イベント, 1:ラッキー）")]
    [SerializeField] Sprite[] tileSprites; // マスの絵
    [SerializeField] Sprite[] bgSprites;   // 背景の絵

    [SerializeField,Header("発生したイベントの名前")] TextMeshProUGUI infoText;

    private void Start()
    {
        window.SetActive(false);
        infoText.text = "";

    }

    public IEnumerator PlayCutinRoutine(TileData tile, System.Action onFadeOutStart)
    {
        // 1. TileTypeに応じて画像をセット
        switch (tile.tileType)
        {
            case TileData.eTileType.EVENT:
                tileImageMain.sprite = tileSprites[0];
                bgImageMain.sprite = bgSprites[0];
                infoText.text = "イベントマス";
                break;
            case TileData.eTileType.LUCKY:
                tileImageMain.sprite = tileSprites[1];
                bgImageMain.sprite = bgSprites[1];
                infoText.text = "ラッキーマス";

                break;
            default:
                yield break; // イベント/ラッキー以外は何もしない
        }
        // 2. 初期化
        window.SetActive(true);
        Vector3 startPos = tileImageMain.transform.localPosition;
        ResetVisuals(); // ここで背景のAlphaは0になる

        // 3. マスの画像が拡大して現れる (0.3s)
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / 0.5f;
            tileImageMain.transform.localScale = Vector3.one * Mathf.Lerp(0, 1.2f, ratio);
            yield return null;
        }

        // --- 理想のポイント1: 少しして背景が出る ---
        yield return new WaitForSeconds(0.2f); // 少しだけ「マスの絵」を見せる「タメ」
        elapsed = 0;
        float bgFadeInDuration = 0.3f; // 0.3秒かけてフェードイン
        while (elapsed < bgFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / bgFadeInDuration;
            SetAlpha(bgImageMain, ratio); // 0から1へ
            yield return null;
        }
        SetAlpha(bgImageMain, 1); // 念のため最後は完全に1にする

        // 4. 溜め
        yield return new WaitForSeconds(1.0f);

        // 5. 退場演出：下にフェードアウト
        // STEP 5-1: マスの画像だけ先に消える (背景はまだ残す)
        elapsed = 0;
        bool hasTriggered = false;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / 0.5f;

            if (!hasTriggered)
            {
                onFadeOutStart?.Invoke();
                hasTriggered = true;
            }

            // マスを移動させて、透明にする
            tileImageMain.transform.localPosition = startPos + new Vector3(0, -100f * ratio, 0);
            SetAlpha(tileImageMain, 1f - ratio);

            // 【ポイント】ここでは bgImageMain はまだいじらない（表示したまま）
            yield return null;
        }

        // STEP 5-2: マスが完全に消えた後、背景だけをスッと消す
        elapsed = 0;
        float bgFadeDuration = 0.3f; // 少しゆっくり消すと綺麗です
        while (elapsed < bgFadeDuration)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / bgFadeDuration;

            // 背景のAlphaを 1 から 0 へ
            SetAlpha(bgImageMain, 1f - ratio);

            yield return null;
        }
        tileImageMain.transform.localPosition = startPos;
        window.SetActive(false);
        infoText.text = "";

    }

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    void ResetVisuals()
    {
        tileImageMain.transform.localScale = Vector3.zero;
        SetAlpha(tileImageMain, 1);
        SetAlpha(bgImageMain, 0);
    }
}
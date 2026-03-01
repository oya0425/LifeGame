using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceSpinner : MonoBehaviour
{
    public static DiceSpinner instance;
    /// <summary>インスペクター上で針の位置を取ってくる</summary>
    [Header("ルーレット針")]
    [SerializeField] public RectTransform needle;

    /// <summary>ルーレット結果（1〜6）</summary>
    [Header("結果保存用")]
    [SerializeField] public int result = 1;

    [SerializeField,Header("初期位置")]
    RectTransform selfRect;

    [SerializeField,Header("デフォルト位置")]
    RectTransform defaultPos;
    [SerializeField,Header("順番決め中の位置")]
    RectTransform orderSelectPos;

    /// <summary>針の初期角度</summary>
    private Quaternion initialRotation;

    /// <summary>回転中かどうか</summary>
    private bool isSpinning = false;

    // --- 結果通知イベント（必要なら外部で使用） ---
    public event System.Action<int> OnSpinEnd;

    public event System.Action OnSpinStart;

    /// <summary>
    /// 順番決めのとき、何番目のプレイヤーが今抽選中か（0〜）
    /// </summary>
    [SerializeField] public int selectCount = 0;


    [Header("演出設定")]
    [SerializeField] private AnimationCurve spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Vector2 backBounceRange = new Vector2(3f, 8f); // 停止時の跳ね返り角度
    private float lastTickAngle = 0f;

    // --- 画像の色を変える（数字）---
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color winColor = new Color(1f, 0.85f, 0f); // 金色

    [SerializeField] private RectTransform[] numberObjects; // 1~6の数字UIを順番に入れる
    // 文字のImageコンポーネントをキャッシュ（Startで取得）
    private Image[] numberImages;
    // --- Startの少し上で変数を追加 ---
    private Outline[] numberOutlines;

    // --- 音 ---
    [SerializeField] AudioManager audioManager;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        initialRotation = needle.rotation;
        numberImages = new Image[numberObjects.Length];
        numberOutlines = new Outline[numberObjects.Length];
        for (int i = 0; i < numberObjects.Length; i++)
        {
            numberImages[i] = numberObjects[i].GetComponent<Image>();
            numberOutlines[i] = numberObjects[i].GetComponent<Outline>();
            if (numberImages[i] != null) numberImages[i].color = normalColor;
            if (numberOutlines[i] != null) numberOutlines[i].enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpin();
        }
    }

    /// <summary>
    /// ルーレット開始（いろいろ条件を満たしている必要あり）
    /// </summary>
    public void StartSpin()
    {
        if (isSpinning) return;                                 // すでに回転中
        if (PlayerMover.instance.GetIsMove()) return;           // プレイヤー移動中は回せない
        //if (!MoveCamera.instance.GetIsInitPosition()) return;   // カメラ位置が戻っていない
        if (audioManager != null) audioManager.PlaySE("DecisionSE");

        isSpinning = true;
        OnSpinStart?.Invoke();
        //GameManager.instance.HideBackButton();
        //StartCoroutine(SpinNeedle());
        StartCoroutine(SpinNeedleRoutine());
    }

    IEnumerator SpinNeedleRoutine()
    {
        float totalTime = 4.0f; // 少し長めにするとハラハラします
        float t = 0;

        // 最終的な回転量（最低5回転 + ランダムな角度）
        float startAngle = needle.eulerAngles.z;
        float totalRotation = 360f * 15f + Random.Range(0f, 360f);

        lastTickAngle = startAngle;

        while (t < totalTime)
        {
            t += Time.deltaTime;
            float progress = t / totalTime;

            // AnimationCurveを使って「粘り」のある回転を計算
            float evaluatedProgress = spinCurve.Evaluate(progress);
            float currentRotation = startAngle - (totalRotation * evaluatedProgress);

            needle.rotation = Quaternion.Euler(0, 0, currentRotation);

            int currentHover = CalculateResult();
            for (int i = 0; i < numberObjects.Length; i++)
            {
                // 指している数字だけ少し大きく、他は標準サイズに
                float targetScale = (i + 1 == currentHover) ? 1.3f : 1.0f;
                numberObjects[i].localScale = Vector3.Lerp(numberObjects[i].localScale, Vector3.one * targetScale, 0.2f);
                // 回転中は全員白
                if (numberImages[i] != null) numberImages[i].color = normalColor;
            }

            // --- カチカチ演出 (60度ごとに処理) ---
            if (Mathf.Abs(currentRotation - lastTickAngle) >= 60f)
            {
                // ここで音を鳴らす
                // if(tickAudio) tickAudio.PlayOneShot(tickAudio.clip);
                if (audioManager != null) audioManager.PlaySE("RouletteQSE");
                // 針を少しだけ振動させる（しなり表現）
                // needle.localScale = new Vector3(1.1f, 0.9f, 1f); // ほんの一瞬太らせるなど

                lastTickAngle -= 60f;
                // 針の見た目を一瞬だけ「ビクッ」とさせる演出（DOTweenなしでも簡単）
                StartCoroutine(TickVisualEffect());
            }

            yield return null;
        }

        // ---- 停止時の「揺り戻し」演出 ----
        float finalAngle = needle.eulerAngles.z;

        // ランダムな跳ね返り角度を決定
        float randomBounce = Random.Range(backBounceRange.x, backBounceRange.y);

        // 1. ちょっと行き過ぎる（一気に）
        yield return RotateToAngle(finalAngle - randomBounce, 0.05f);

        // 2. 戻る（少しゆっくり）
        yield return RotateToAngle(finalAngle + (randomBounce * 0.3f), 0.1f);

        // 3. 最終位置にピタッと止まる
        yield return RotateToAngle(finalAngle, 0.1f);

        
        // ---- 出目計算 ----
        result = CalculateResult();
        Debug.Log("出た目:" + result);

        // 【ここがポイント：金色に光らせて大きくする】
        int winIndex = result - 1;
        for (int i = 0; i < numberObjects.Length; i++)
        {
            if (i == winIndex)
            {
                numberObjects[i].localScale = Vector3.one * 1.8f; // ボヨヨンと大きく
                if (numberImages[i] != null) numberImages[i].color = winColor; // 金色に


                // 【追加】アウトラインを有効にして、白く光らせる
                if (numberOutlines[i] != null)
                {
                    numberOutlines[i].enabled = true;
                    numberOutlines[i].effectColor = Color.white; // 光ってる感を出すために白
                    numberOutlines[i].effectDistance = new Vector2(4f, -4f); // 少し太めに
                }
            }
            else
            {
                numberObjects[i].localScale = Vector3.one;
                if (numberImages[i] != null) numberImages[i].color = normalColor;
                if (numberImages[i] != null) numberImages[i].color = normalColor;
                if (numberOutlines[i] != null) numberOutlines[i].enabled = false;
            }
        }
        if (audioManager != null) audioManager.PlaySE("RouletteDecisionSE");

        yield return new WaitForSeconds(0.2f); // 少し待つ

        // 2. 少し縮ませる (ギュッ)
        numberObjects[winIndex].localScale = Vector3.one * 1.4f;
        yield return new WaitForSeconds(0.2f);
        if (audioManager != null) audioManager.PlaySE("RouletteDecisionSE");

        // 3. もう一度だけ少し膨らんでから落ち着く (ポヨン)
        numberObjects[winIndex].localScale = Vector3.one * 1.6f;
        yield return new WaitForSeconds(0.2f);

        // 最終的な大きさをキープ（または1.5fくらいで少し強調したままにする）
        numberObjects[winIndex].localScale = Vector3.one * 1.5f;

        yield return new WaitForSeconds(0.5f);
        OnSpinEnd?.Invoke(result);
    }

    // 指定の角度まで滑らかに回転させるサブコルーチン
    IEnumerator RotateToAngle(float targetAngle, float duration)
    {
        float time = 0;
        Quaternion startRot = needle.rotation;
        Quaternion endRot = Quaternion.Euler(0, 0, targetAngle);
        while (time < duration)
        {
            needle.rotation = Quaternion.Slerp(startRot, endRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        needle.rotation = endRot;
    }

    // 針を一瞬膨らませるなどの簡易演出用
    IEnumerator TickVisualEffect()
    {
        needle.localScale = new Vector3(1.1f, 1.1f, 1f);
        yield return new WaitForSeconds(0.05f);
        needle.localScale = Vector3.one;
    }
    /// <summary>
    /// 針を 3秒かけて回し → 止まった角度から出目を決定
    /// </summary>
    IEnumerator SpinNeedle()
    {
        float totalTime = 3.0f;
        float speed = Random.Range(2000f, 5000f);
        float t = 0;

        while (t < totalTime)
        {
            float deltaSpeed = Mathf.Lerp(speed, 0, t / totalTime);
            needle.Rotate(0, 0, -deltaSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        // ---- 出目計算 ----
        result = CalculateResult();
        Debug.Log("出た目:" + result);

        yield return new WaitForSeconds(1.0f);

        OnSpinEnd?.Invoke(result);
    }

    /// <summary>
    /// 針の角度から 1〜6 の出目を取得
    /// </summary>
    private int CalculateResult()
    {
        float z = needle.eulerAngles.z;
        float angle = (z + 360) % 360;
        int sector = Mathf.FloorToInt(angle / 60f);

        int[] resultMap = { 6, 5, 4, 3, 2, 1 };
        return resultMap[sector];
    }


    /// <summary>
    /// 針を初期位置に戻し、次のスピンが可能な状態にする
    /// </summary>
    public void ResetNeedle()
    {
        needle.rotation = initialRotation;
        isSpinning = false;
        result = 1;
        //サイズと色をリセット
        for (int i = 0; i < numberObjects.Length; i++)
        {
            numberObjects[i].localScale = Vector3.one;
            if (numberImages[i] != null) numberImages[i].color = normalColor;
            // 【追加】アウトラインも消す
            if (numberOutlines[i] != null) numberOutlines[i].enabled = false;
        }
    }

    /// <summary>
    /// 元の位置に戻す 
    /// </summary>
    public void SetDefaultPosition()
    {
        selfRect.anchoredPosition = defaultPos.anchoredPosition;
    }

    public void SetOrderSelectPosition()
    {
        selfRect.anchoredPosition = orderSelectPos.anchoredPosition;
    }

    public bool GetIsSpinning()
    {
        return isSpinning;
    }
}




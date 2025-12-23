using System.Collections;
using UnityEngine;

public class DiceSpinner : MonoBehaviour
{
    public static DiceSpinner instance;
    /// <summary>インスペクター上で針の位置を取ってくる</summary>
    [Header("ルーレット針")]
    [SerializeField] public RectTransform needle;

    /// <summary>ルーレット結果（1〜6）</summary>
    [Header("結果保存用")]
    [SerializeField] public int result = 1;

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

    /// <summary>
    /// 再抽選時に何番目のプレイヤーを再抽選しているか
    /// </summary>    
    public int rerollCount = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        initialRotation = needle.rotation;
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

        isSpinning = true;
        OnSpinStart?.Invoke();
        //GameManager.instance.HideBackButton();
        StartCoroutine(SpinNeedle());
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
    }
}

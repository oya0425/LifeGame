 using UnityEngine;
using System.Collections.Generic;

public class MoveCamera : MonoBehaviour
{
    public static MoveCamera instance;

    public List<PlayerMover> players = new List<PlayerMover>(); // 全プレイヤー
    public float height = 10f;
    public float distance = 5f;
    public float followSpeed = 5f;
    public float returnSpeed = 2f;

    private Vector3 initialPos;
    private Quaternion initialRot;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
    }

    void Update()
    {
        PlayerMover activePlayer = null;

        // 動いているプレイヤーを優先で探す
        foreach (var p in players)
        {
            if (p != null && p.GetIsMove())
            {
                activePlayer = p;
                break;
            }
        }
        // 1. 移動中のプレイヤー（最優先）
        if (activePlayer != null && activePlayer.GetIsMove())
        {
            FollowPlayer(activePlayer);
            return;
        }

        // 2. 次に動くプレイヤー（上から見下ろす）
        PlayerMover next = GetNextPlayer();
        if (next != null)
        {
            LookDownPlayer(next);
            return;
        }

        // 3. 誰も動いていなければ初期位置へ戻る
        ResetCamera();

        //PlayerMover nextPlayer = GetNextPlayer();
        //if (nextPlayer != null)
        //{
        //    // 次のプレイヤーが既に移動中ならスキップする（必要なら）
        //    if (nextPlayer.GetIsMove())
        //    {
        //        Debug.Log("nextPlayer is already moving - skip camera focus");
        //    }
        //    else
        //    {
        //        Debug.Log("フォーカス: " + nextPlayer.name);
        //        Vector3 targetPos = nextPlayer.transform.position
        //                            - nextPlayer.transform.forward * distance
        //                            + Vector3.up * height*2;

        //        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        //        transform.LookAt(nextPlayer.transform);
        //    }
        //}
        //else if (activePlayer != null)
        //{
        //    // 移動中プレイヤーを追う
        //    Vector3 targetPos = activePlayer.transform.position - activePlayer.transform.forward * distance + Vector3.up * height;
        //    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        //    transform.LookAt(activePlayer.transform);
        //}
        //else
        //{
        //    // 動いているプレイヤーがいなければ初期位置に戻る
        //    transform.position = Vector3.Lerp(transform.position, initialPos, Time.deltaTime * returnSpeed);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, initialRot, Time.deltaTime * returnSpeed);

        //}
    }

    // --- 最初の位置に戻っているか ---
    public bool GetIsInitPosition()
    {
        return 
        Vector3.Distance(transform.position, initialPos) < 0.1f &&
        Quaternion.Angle(transform.rotation, initialRot) < 1f;

    }

    /// <summary>どのプレイヤーの番かを取る
    /// </summary>
    PlayerMover GetNextPlayer()
    {
        foreach (var p in players)
        {
            if (p.isMyTurn)
                return p;
        }
        return null;
    }
    void FollowPlayer(PlayerMover p)
    {
        Vector3 targetPos =
            p.transform.position
            - p.transform.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        transform.LookAt(p.transform);
    }
    void LookDownPlayer(PlayerMover p)
    {

        // 見下ろすように斜めに向く
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(p.transform.position - transform.position),
            Time.deltaTime * returnSpeed * 1000
        );

        Vector3 topPos =
            p.transform.position
            + Vector3.up * (height * 5f)    // 高めに配置
            - p.transform.forward * (distance); // 近めに寄せる

        transform.position = Vector3.Lerp(transform.position, topPos, Time.deltaTime * returnSpeed);
        //// 位置だけをなめらかに移動
        //transform.position = Vector3.Lerp(
        //    transform.position,
        //    topPos,
        //    Time.deltaTime * returnSpeed
        //);

    }
    void ResetCamera()
    {
        transform.position = Vector3.Lerp(transform.position, initialPos, Time.deltaTime * returnSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, initialRot, Time.deltaTime * returnSpeed);
    }
}

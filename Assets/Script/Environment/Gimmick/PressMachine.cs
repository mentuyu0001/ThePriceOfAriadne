using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;


/// <summary>
/// プレス機を動かすスクリプト。プレス機の台座にアタッチすること。
/// </summary>

public class PressMachine : StoppableGimick
{
    // Plateオブジェクト
    [SerializeField] private GameObject plate;
    // Plateの落下位置の判定用オブジェクト
    [SerializeField] private GameObject pressArea;
    //GameOverManagerを参照
    [SerializeField] private GameOverManager gameOverManager;
    //Playerオブジェクトの参照
    [SerializeField] private GameObject player;
    //Playerオブジェクトのオブジェクト名
    private string playerName;
    // Plateの開始位置（ローカル座標）
    [SerializeField] private Vector2 posStart;
    // Plateのスタンバイ位置（ローカル座標）
    [SerializeField] private Vector2 posReady;
    // Plateの落下位置（ローカル座標）
    [SerializeField] private Vector2 posPressed;
    //プレス機のクールタイム
    [SerializeField] private float coolTime;
    // PlateオブジェクトのRigidBody2D
    private Rigidbody2D plateRigidBody;
    // 動作するかを判定
    private bool isMoving;
    // キャンセレーショントークン
    private CancellationTokenSource cancellationTokenSource;
    // Playerの死亡判定用、PressAreaに入ったらtrueになる
    private bool isInsidePressArea;
    //

    void Start()
    {
        // オブジェクトがアタッチされているかチェック
        if (plate == null)
        {
            Debug.LogError("PressMachine: Plate object is undefined.");
        }
        if (pressArea == null)
        {
            Debug.LogError("PressMachine: PressArea is undefined");
        }
        plateRigidBody = plate.GetComponent<Rigidbody2D>();
        if (plateRigidBody == null)
        {
            Debug.LogError("PressMachine: RigidBody2D component cannot found in Plate object.");
        }
        if (gameOverManager == null)
        {
            Debug.LogError("PressMachine: GameOverManager cannnot found.");
        }
        if (player != null)
        {
            playerName = player.name;
        }
        // トークンを生成
            cancellationTokenSource = new CancellationTokenSource();
        isMoving = true;
        isInsidePressArea = false;
        // プレス機のPlateを初期位置へ
        plate.transform.localPosition = posStart;
        // 動作開始
        Debug.Log("PressMachine started");
        MoveLoop(cancellationTokenSource.Token).Forget();
    }

    async UniTask MoveLoop(CancellationToken MyToken = default)
    {
        // Start関数が呼び出されるまで待つ
        await this.GetAsyncStartTrigger().StartAsync();
        // 動作のループ
        while (isMoving)
        {
            Debug.Log("---  Move begin ---");
            // Plateをスタンバイ位置へ移動
            await plateRigidBody.DOLocalPath(
                path: new Vector2[] { posStart, posReady },
                duration: 0.2f
            ).WithCancellation(MyToken);
            // スタンバイ位置へ移動したらちょっと待つ
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: MyToken);
            // Plateを落下位置へ移動
            await plateRigidBody.DOLocalPath(
                path: new Vector2[] { posReady, posPressed },
                duration: 1.0f
            ).WithCancellation(MyToken);
            // 落下位置へ移動したらちょっと待つ
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: MyToken);
            // Plateを再びスタート位置へ移動
            await plateRigidBody.DOLocalPath(
                path: new Vector2[] { posPressed, posStart },
                duration: 4.0f
            ).WithCancellation(MyToken);
            // 指定されたクールタイム分だけ待つ
            await UniTask.Delay(TimeSpan.FromSeconds(coolTime), cancellationToken: MyToken);
            Debug.Log("--- Move end ---");
        }
    }

    // プレス機の動作を止める関数
    public override void StopGimick()
    {
        // 非同期処理をキャンセル
        isMoving = false;
        cancellationTokenSource.Cancel();
        // Plateをスタート位置へ移動
        Vector2 posNow = plate.transform.localPosition;
        plateRigidBody.DOLocalPath(
            path: new Vector2[] { posNow, posStart },
            duration: 1.0f
        );
        Debug.Log("Stopped PressMachine!");
    }

    // StopGimick関数のデバッグ用--------------
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         StopGimick();
    //     }
    // }
    // -----------------------------------------

    // オブジェクトが破棄される時に非同期処理をキャンセルする
    private void OnDestroy()
    {
        // 非同期処理をキャンセル
        cancellationTokenSource.Cancel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (pressArea != null && collision.gameObject.name == playerName)
        {
            isInsidePressArea = true;
            Debug.Log("Player entered PressArea");
        }
    }
}

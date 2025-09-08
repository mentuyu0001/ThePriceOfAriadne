using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using NUnit.Framework.Constraints;
using VContainer;


/// <summary>
/// プレス機を動かすスクリプト。プレス機の台座にアタッチすること。
/// </summary>

public class PressMachineBase : StoppableGimick
{
    [Header("ゲームオブジェクト")]
    // Plateオブジェクト
    [SerializeField] private GameObject plate;
    // Plateの落下位置の判定用オブジェクト
    [SerializeField] private GameObject pressArea;
    //GameOverManagerを参照
    [Inject] private GameOverManager gameOverManager;
    //Playerオブジェクトの参照
    [Inject] private GameObject player;
    //Playerオブジェクトのオブジェクト名
    private string playerName;
    [Header("Plateの座標（ローカル座標）")]
    // Plateの開始位置（ローカル座標）
    [SerializeField] private Vector2 posStart;
    // // Plateのスタンバイ位置（ローカル座標）
    // [SerializeField] private Vector2 posReady;
    // Plateの落下位置（ローカル座標）
    [SerializeField] private Vector2 posPressed;
    [Space]
    //プレス機のクールタイム
    [SerializeField] private float coolTime;
    // PlateオブジェクトのRigidBody2D
    private Rigidbody2D plateRigidBody;
    // Playerの死亡判定用、PressAreaに入ったらtrueになる
    private bool isInsidePressArea;
    // Playerの死亡判定用、Plateに接触している間trueになる
    // PlayerがPressArea内でジャンプし、Plateに接触してからPressAreaに入った時、
    // 正しくゲームオーバーを呼び出すために使用
    private bool isOnPlate;
    //DOTweenのシーケンス
    private Sequence MoveSequence;
    // プレス機が落下中かを判定するbool型変数
    // 落下中にプレイヤーがPressArea内にいて、isOnPlateがtrueならゲームオーバーを呼び出す
    private bool isFalling;
    // ゲームスタート時に停止させておくか判定するbool型変数
    // trueならゲームスタート時は動く状態
    [SerializeField] private bool isMovingAtStart;
    // アニメーションが動作中か判定する変数
    // プレス機が動いているならtrue

    void Start()
    {
        // オブジェクトがアタッチされているかチェック
        if (plate == null)
        {
            Debug.LogError("PressMachineBase: Plate object is undefined.");
        }
        else
        {
            if (plate.GetComponent<BoxCollider2D>() == null)
            {
                Debug.LogError("PressMachineBase: BoxCollider2D cannnot found in Plate");
            }
        }
        if (pressArea == null)
        {
            Debug.LogError("PressMachineBase: PressArea is undefined");
        }
        else
        {
            // PressAreaのBoxCollider2Dがアタッチされているかチェック
            BoxCollider2D boxCollider2D = pressArea.GetComponent<BoxCollider2D>();
            if (boxCollider2D == null)
            {
                Debug.LogError("PressMachineBase: BoxCollider2D cannnot found in PressArea.");
            }
            else
            {
                // isTriggerをオンにする
                boxCollider2D.isTrigger = true;
            }
        }
        plateRigidBody = plate.GetComponent<Rigidbody2D>();
        if (plateRigidBody == null)
        {
            Debug.LogError("PressMachineBase: RigidBody2D component cannot found in Plate object.");
        }
        if (gameOverManager == null)
        {
            Debug.LogError("PressMachineBase: GameOverManager cannnot found.");
        }
        if (player != null)
        {
            playerName = player.name;
        }
        else
        {
            Debug.LogError("PressMachineBase: Player cannot found.");
        }

        isInsidePressArea = false;
        isOnPlate = false;
        // プレス機のPlateを初期位置へ
        plate.transform.localPosition = posStart;
        // DOTweenの初期設定
        DOTween.Init();
        // こちらが指示するまでアニメーションを開始しないようにする
        DOTween.defaultAutoPlay = AutoPlay.None;
        //DOTweenのシーケンスを定義
        MoveSequence = DOTween.Sequence();
        // シーケンスに動作を追加---------------------------------------------
        // // Plateをスタンバイ位置へ移動
        // MoveSequence.Append(plateRigidBody.DOLocalPath(
        //     path: new Vector2[] { posStart, posReady },
        //     duration: 0.2f
        // ));
        // // スタンバイ位置へ移動したらちょっと待つ
        // MoveSequence.AppendInterval(1.0f);
        // Plateを落下位置へ移動
        MoveSequence.Append(plateRigidBody.DOLocalPath(
            path: new Vector2[] { posStart, posPressed },
            duration: 1.0f
        ).SetEase(Ease.InQuint))   // 動きを5次関数に変更（中身を変えたらコメントも変えること）
        .OnStart(() => { isFalling = true; })   // 落下開始時にisFallingをtrueにする
        .OnComplete(() => { isFalling = false; });  // 落下終了時にisFallingをfalseにする
        // 落下位置へ移動したらちょっと待つ
        MoveSequence.AppendInterval(1.5f);
        // Plateを再びスタート位置へ移動
        MoveSequence.Append(plateRigidBody.DOLocalPath(
            path: new Vector2[] { posPressed, posStart },
            duration: 4.0f
        ));
        // 指定されたクールタイム分だけ待つ
        MoveSequence.AppendInterval(coolTime);
        // ループを設定
        MoveSequence.SetLoops(-1, LoopType.Restart);
        // -------------------------------------------------------------------
        // スタート時から動かすのであれば、アニメーションを開始する
        if (isMovingAtStart == true)
        {
            StartGimick();
        }
    }

    // プレス機の動作を止める関数
    public override void StopGimick()
    {
        // アニメーションを停止
        MoveSequence.Pause();
        // Plateをスタート位置へ移動
        Vector2 posNow = plate.transform.localPosition;
        plateRigidBody.DOLocalPath(
            path: new Vector2[] { posNow, posStart },
            duration: 1.0f
        ).Play();
        Debug.Log("Stopped PressMachine!");
    }

    // オブジェクトが破棄される時にアニメーションを停止する
    private void OnDestroy()
    {
        // アニメーションを停止
        MoveSequence.Kill();
    }

    // PlayerがPressAreaへ入ったことを検知する
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pressArea != null && collision.gameObject.name == playerName)
        {
            isInsidePressArea = true;
            Debug.Log("Player entered PressArea");
            // Plateと接触中かつPlateが落下している時にPressAreaに入ったらゲームオーバーを呼び出す
            if (isOnPlate == true && isFalling == true)
            {
                gameOverManager.GameOver();
            }
        }
    }

    // PlayerがPressAreaから出たことを検知する
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (pressArea != null && collision.gameObject.name == playerName)
        {
            isInsidePressArea = false;
            Debug.Log("Player exited PressArea");
        }
    }

    // PlateにPlayerが接触した時に、PressMachinePlateスクリプトから呼び出す
    public void OnPlateCollisionEnter()
    {
        isOnPlate = true;
        // PlayerがPressArea内にいて、かつPlateが落下中なら
        if (isInsidePressArea == true && isFalling == true)
        {
            // ゲームオーバーを呼び出す
            gameOverManager.GameOver();
        }
    }

    // PlayerがPlateから離れた時に、PressMachinePlateスクリプトから呼び出す
    public void OnPlateCollisionExit()
    {
        isOnPlate = false;
    }


    // StoppableGimickに追加した抽象メソッドを実装
    public override bool IsRunning => MoveSequence.IsPlaying(); // ギミックが動作中かどうかを示すプロパティ
    public override void StartGimick()
    {
        // ギミックを再起動するために、シーケンスをリセットして再スタート
        MoveSequence.Restart();
        Debug.Log("Started PressMachine!");
    }
}

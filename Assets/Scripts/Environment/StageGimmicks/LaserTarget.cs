using UnityEngine;
using DG.Tweening;

/// <summary>
/// レーザーのターゲットを上下に動かすスクリプト
/// </summary>
public class LaserTarget : MonoBehaviour
{
    [SerializeField] private Transform laserStand;
    [SerializeField] private Transform underStand;
    
    [Header("アニメーション設定")]
    [SerializeField] private float moveDuration = 2.0f;
    [SerializeField] private float delayBetweenMovement = 0.5f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;
    [SerializeField] private bool startFromTop = true;
    
    [Header("位置調整")]
    [SerializeField] private float targetHeight = 1.0f;
    
    // ゲームスタート時に停止させておくか判定するbool型変数
    // trueならゲームスタート時は動く状態
    [SerializeField] private bool isMovingAtStart;
    
    // DoTweenのシーケンス
    private Sequence moveSequence;
    private float topPosition;
    private float bottomPosition;
    
    void Start()
    {
        // オブジェクトの初期化チェック
        if (laserStand == null || underStand == null)
        {
            Debug.LogError("LaserTarget: LaserStand または UnderStand が設定されていません");
            return;
        }
        
        // 初期設定を行う
        Initialize();
        
        // DoTweenの初期設定
        DOTween.Init();
        // こちらが指示するまでアニメーションを開始しないようにする
        DOTween.defaultAutoPlay = AutoPlay.None;
        
        // DoTweenのシーケンスを定義
        moveSequence = DOTween.Sequence();
        
        // シーケンスにアニメーションを追加
        if (startFromTop)
        {
            // 上から下へ、下から上へのループを作成
            moveSequence.Append(transform.DOMoveY(bottomPosition, moveDuration).SetEase(easeType))
                      .AppendInterval(delayBetweenMovement)
                      .Append(transform.DOMoveY(topPosition, moveDuration).SetEase(easeType))
                      .AppendInterval(delayBetweenMovement);
        }
        else
        {
            // 下から上へ、上から下へのループを作成
            moveSequence.Append(transform.DOMoveY(topPosition, moveDuration).SetEase(easeType))
                      .AppendInterval(delayBetweenMovement)
                      .Append(transform.DOMoveY(bottomPosition, moveDuration).SetEase(easeType))
                      .AppendInterval(delayBetweenMovement);
        }
        
        // ループを設定
        moveSequence.SetLoops(-1, LoopType.Restart);
        
        // 初期位置の設定
        SetInitialPosition();
        
        // スタート時から動かすのであれば、アニメーションを開始する
        if (isMovingAtStart)
        {
            StartTarget();
        }
    }
    
    // 初期化処理
    private void Initialize()
    {
        // Targetの高さを取得（スプライトの場合）
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            targetHeight = renderer.bounds.size.y;
        }
        
        // 上下の位置を計算
        topPosition = laserStand.position.y - (targetHeight / 2);
        bottomPosition = underStand.position.y + (targetHeight / 2);
    }
    
    // 初期位置の設定
    private void SetInitialPosition()
    {
        if (startFromTop)
        {
            transform.position = new Vector3(transform.position.x, topPosition, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, bottomPosition, transform.position.z);
        }
    }
    
    // アニメーション開始
    public void StartTarget()
    {
        if (moveSequence != null)
        {
            // シーケンスをリセットして再スタート
            moveSequence.Restart();
            Debug.Log("LaserTarget: ターゲットの動作を開始しました");
        }
    }
    
    // アニメーション停止
    public void StopTarget()
    {
        if (moveSequence != null)
        {
            // アニメーションを停止
            moveSequence.Pause();
            Debug.Log("LaserTarget: ターゲットを停止しました");

            // ターゲットを非表示にする
            gameObject.SetActive(false);
        }
    }
    
    // オブジェクトが破棄される時にアニメーションを停止する
    private void OnDestroy()
    {
        if (moveSequence != null)
        {
            moveSequence.Kill();
        }
    }
    
    // オブジェクトが非アクティブになる時にアニメーションを停止する
    private void OnDisable()
    {
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Pause();
        }
    }
    
    // オブジェクトがアクティブになる時にアニメーションを再開する（オプション）
    private void OnEnable()
    {
        if (moveSequence != null && isMovingAtStart && !moveSequence.IsActive())
        {
            StartTarget();
        }
    }
    
    // 外部からアニメーションを再開するためのメソッド
    public void RestartTarget()
    {
        gameObject.SetActive(true);
        moveSequence.Restart();
    }
}



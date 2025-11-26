using UnityEngine;
using DG.Tweening;
using VContainer;

/// <summary>
/// レーザーのターゲットを上下に動かすスクリプト
/// </summary>
public class LaserTarget : MonoBehaviour
{
    [SerializeField] private Transform laserStand;
    [SerializeField] private Transform underStand;
    
    // Laserオブジェクトへの参照を追加
    [SerializeField] private Laser laser;
    
    // ナイフのタグ名を定義
    [SerializeField] private string knifeTag = "Knife";
    
    [Header("アニメーション設定")]
    [SerializeField] private float moveDuration = 2.0f;
    [SerializeField] private float delayBetweenMovement = 0.5f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;
    [SerializeField] private bool startFromTop = true;
    
    [Header("位置調整")]
    [SerializeField] private float targetHeight = 10.0f;

    
    // ゲームスタート時に停止させておくか判定するbool型変数
    // trueならゲームスタート時は動く状態
    [SerializeField] private bool isMovingAtStart;
    
    // DoTweenのシーケンス
    private Sequence moveSequence;
    private float topPosition;
    private float bottomPosition;
    
    void Awake()
    {
        // オブジェクトの初期化チェック
        if (laserStand == null || underStand == null)
        {
            Debug.LogError("LaserTarget: LaserStand または UnderStand が設定されていません");
            return;
        }
        
        // Laserオブジェクトの参照チェック
        if (laser == null)
        {
            Debug.LogWarning("LaserTarget: Laserオブジェクトが設定されていません");
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
                      .AppendInterval(delayBetweenMovement)
                      .SetLink(gameObject);
        }
        else
        {
            // 下から上へ、上から下へのループを作成
            moveSequence.Append(transform.DOMoveY(topPosition, moveDuration).SetEase(easeType))
                      .AppendInterval(delayBetweenMovement)
                      .Append(transform.DOMoveY(bottomPosition, moveDuration).SetEase(easeType))
                      .AppendInterval(delayBetweenMovement)
                      .SetLink(gameObject);
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
    
    // ナイフとの衝突を検出する（Collider2Dを使用している場合）
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 衝突したオブジェクトがナイフタグを持っているか確認
        if (collision.CompareTag(knifeTag))
        {
            // Laserギミックを停止
            if (laser != null)
            {
                // 破壊SE（id:5）を再生
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySE(5);
                }
            
                laser.KillLaser();
                laser.StopGimick();
                Debug.Log("LaserTarget: ナイフとの衝突を検知し、レーザーギミックを停止しました");
            }
            else
            {
                Debug.LogError("LaserTarget: Laserオブジェクトが設定されていないため、StopGimmick()を呼び出せません");
            }
        }
    }
    
    // 物理衝突を検出する（Rigidbodyを使用している場合）
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("LaserTarget: OnCollisionEnter2Dが呼び出されました");
        // 衝.突したオブジェクトがナイフタグを持っているか確認
        if (collision.gameObject.CompareTag(knifeTag))
        {
            // Laserギミックを停止
            if (laser != null)
            {
                laser.StopGimick();
                laser.KillLaser();
                // 破壊SE（id:5）を再生
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySE(5);
                }
            
                Debug.Log("LaserTarget: ナイフとの衝突を検知し、レーザーギミックを停止しました");
            }
            else
            {
                Debug.LogError("LaserTarget: Laserオブジェクトが設定されていないため、StopGimmick()を呼び出せません");
            }
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



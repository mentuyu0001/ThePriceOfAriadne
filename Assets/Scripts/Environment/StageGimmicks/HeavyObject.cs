using UnityEngine;
using Cysharp.Threading.Tasks;
using VContainer;
using Unity.VisualScripting;

/// <summary>
/// プレイヤーが重いものを押すためのスクリプト
/// </summary>
public class HeavyObject : MonoBehaviour
{
    [Inject] private GameObject player;
    [Inject] private PlayerStatus playerStatus;
    [Inject] private PlayerAnimationManager playerAnimationManager;
    [Inject] private GameTextDisplay textDisplay;
    [Inject] private PlayerPartsRatio partsRatio;
    [Inject] private ObjectTextData objectTextData;
    
    [SerializeField] private int heavyObjectID = 3; // HeavyObjectのID
    
    [SerializeField, Range(0.1f, 10f)]
    private float maxSpeed = 2f; // 最大速度を制限

    private float distanceThreshold; // 離れすぎないようにする距離

    private float distanceThresholdPlus = 0.55f; // プレイヤーが離れすぎたときの距離閾値
    
    [Header("テキスト表示設定")]
    [SerializeField] private float delayBetweenTexts = 2f; // 複数テキスト間の待機時間
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = false;
    
    [SerializeField] private int pushSEIndex = 1; // 押しSEのインデックス

    private bool isPushing = false;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Vector2 previousPlayerPosition;

    // 既存のフィールド
    private bool canPushAgain = true; // 新しいフラグを追加
    private bool hasShownText = false; // テキストを表示済みかどうか

    // 非同期操作をキャンセルするためのトークン
    private System.Threading.CancellationTokenSource moveCts;

    // デバッグ用
    private bool isLoopRunning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // VContainerで注入されたplayerの確認
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            previousPlayerPosition = player.transform.position;
            if (showDebugLogs) Debug.Log($"VContainerでプレイヤーが正常に注入されました: {player.name}");
        }
        else
        {
            Debug.LogError("VContainerでプレイヤーの注入に失敗しました");
        }

        // VContainerで注入されたplayerStatusの確認
        if (playerStatus != null)
        {
            if (showDebugLogs) Debug.Log($"VContainerでPlayerStatusが正常に注入されました");
        }
        else
        {
            Debug.LogError("VContainerでPlayerStatusの注入に失敗しました");
        }

        // オブジェクトの横の長さを取得してdistanceThresholdを設定
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            distanceThreshold = spriteRenderer.bounds.size.x / 2f + distanceThresholdPlus;
            if (showDebugLogs) Debug.Log($"distanceThresholdが設定されました: {distanceThreshold}");
        }
        else
        {
            Debug.LogWarning("SpriteRendererが見つかりませんでした。distanceThresholdをデフォルト値に設定します。");
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (player != null && collision.gameObject == player && playerStatus != null)
        {
            // 押せない場合はテキストを表示
            if (!playerStatus.CanPushHeavyObject && !hasShownText)
            {
                if (showDebugLogs) Debug.Log("重いものを押せない！");
                ShowHeavyObjectWarningText();
                hasShownText = true;
            }
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (player != null && collision.gameObject == player && playerStatus != null)
        {
            // プレイヤーとの距離をチェック
            //float distance = Vector2.Distance(rb.position, player.transform.position);
            float distance = Mathf.Abs(rb.position.x - player.transform.position.x);

            Vector2 hitPos = collision.GetContact(0).point;
            Vector2 direction = (hitPos - rb.position).normalized;
            
            if (playerStatus.CanPushHeavyObject && canPushAgain && distance <= distanceThreshold && (Vector2.Dot(direction, Vector2.right) > 0.8f || Vector2.Dot(direction, Vector2.left) > 0.8f))
            {
                // 距離が閾値内にあるときのみ押せるようにする
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                if (!isPushing)
                {
                    PushObject();
                }
            }
            else
            {
                // 距離が離れすぎている、または押せない状態
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                if (isPushing)
                {
                    StopPushing();
                }
            }
        }
    }

    private void PushObject()
    {
        // すでに押し中の場合は何もしない
        if (isPushing) return;
        
        isPushing = true;

        // アニメーションを開始
        playerAnimationManager.AniPushTrue();

        // 押しSEをループ再生
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(pushSEIndex, true); // ループ再生
        }
        
        // 既存のループをキャンセル
        moveCts?.Cancel();
        moveCts = new System.Threading.CancellationTokenSource();
        
        if (showDebugLogs) Debug.Log("PushObject: 押し操作開始");
        MoveLoop(moveCts.Token).Forget();
    }

    private void StopPushing()
    {
        if (!isPushing) return;
        
        isPushing = false;

        // アニメーションを停止
        playerAnimationManager.AniPushFalse();

        // 押しSEを停止
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopSE();
        }
        
        // ループをキャンセル
        moveCts?.Cancel();
        moveCts = null;
        if (showDebugLogs) Debug.Log("StopPushing: 押し操作停止");
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        rb.linearVelocity = Vector2.zero;
        if (player != null && collision.gameObject == player)
        {
            StopPushing();
            
            // コリジョンから離れたらテキストを非表示＆フラグをリセット
            hasShownText = false;
            
            if (textDisplay != null && textDisplay.IsDisplaying)
            {
                textDisplay.HideText();
            }
        }
    }
    
    private void ShowHeavyObjectWarningText()
    {
        // 拡張メソッドを使用してテキストを表示
        textDisplay.ShowTextByPartsRatio(
            partsRatio,
            objectTextData,
            heavyObjectID,
            delayBetweenTexts,
            showDebugLogs
        );
    }
    
    private async UniTaskVoid MoveLoop(System.Threading.CancellationToken cancellationToken)
    {
        if (isLoopRunning)
        {
            if (showDebugLogs) Debug.LogWarning("MoveLoop: すでに実行中のループがあります");
            return;
        }
        
        isLoopRunning = true;
        Vector2 lastValidXPosition = rb.position;
        bool isDistanceExceeded = false;
        
        if (showDebugLogs) Debug.Log("MoveLoop: ループ開始");
        
        try
        {
            while (isPushing && !cancellationToken.IsCancellationRequested)
            {
                if (rb != null && playerRb != null)
                {
                    // プレイヤーとの距離を計算
                    //float distance = Vector2.Distance(rb.position, playerRb.position);
                    float distance = Mathf.Abs(rb.position.x - playerRb.position.x);

                    if (distance > distanceThreshold)
                    {
                        if (showDebugLogs) Debug.Log("プレイヤーが離れすぎています - 動きを制限します");
                        
                        canPushAgain = false;
                        
                        if (!isDistanceExceeded)
                        {
                            lastValidXPosition = rb.position;
                            isDistanceExceeded = true;
                        }

                        // X軸方向のみ位置を固定
                        rb.position = new Vector2(lastValidXPosition.x, rb.position.y);
                        
                        // X軸の速度のみゼロに設定（Y軸は変更しない）
                        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                        
                        // X軸方向と回転のみ固定（Y軸は自由）
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                        
                        StopPushing();
                        ResetCanPushAgain().Forget();
                        break;
                    }
                    else
                    {
                        isDistanceExceeded = false;
                        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                        
                        // X軸方向のみプレイヤーの速度を適用
                        float xVelocity = Mathf.Clamp(playerRb.linearVelocity.x, -maxSpeed, maxSpeed);
                        rb.linearVelocity = new Vector2(xVelocity, 0f);
                    }
                }
                
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
            }
        }
        catch (System.OperationCanceledException)
        {
            if (showDebugLogs) Debug.Log("MoveLoop: キャンセルされました");
        }
        finally
        {
            isLoopRunning = false;
            if (showDebugLogs) Debug.Log("MoveLoop: ループ終了");
        }
    }
    
    // 一定時間後にcanPushAgainをリセットする
    private async UniTaskVoid ResetCanPushAgain()
    {
        await UniTask.Delay(1000); // 1秒待機
        canPushAgain = true;
    }

    private void OnDestroy()
    {
        // クリーンアップ
        moveCts?.Cancel();
        moveCts = null;
    }
}
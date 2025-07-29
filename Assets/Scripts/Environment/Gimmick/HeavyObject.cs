using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// プレイヤーが重いものを押すためのスクリプト
/// </summary>
public class HeavyObject : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private PlayerStatus playerStatus;

    [SerializeField, Range(0.1f, 10f)]
    private float maxSpeed = 5f; // 最大速度を制限

    private float distanceThreshold; // 離れすぎないようにする距離

    private float distanceThresholdPlus = 0.6f; // プレイヤーが離れすぎたときの距離閾値
    private bool isPushing = false;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Vector2 previousPlayerPosition;

    // 既存のフィールド
    private bool canPushAgain = true; // 新しいフラグを追加

    // 非同期操作をキャンセルするためのトークン
    private System.Threading.CancellationTokenSource moveCts;

    // デバッグ用
    private bool isLoopRunning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 初期状態ではX方向とY方向を固定
        //rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            previousPlayerPosition = player.transform.position;
        }

        // オブジェクトの横の長さを取得してdistanceThresholdを設定
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            distanceThreshold = spriteRenderer.bounds.size.x / 2f + distanceThresholdPlus;
            Debug.Log($"distanceThresholdが設定されました: {distanceThreshold}");
        }
        else
        {
            Debug.LogWarning("SpriteRendererが見つかりませんでした。distanceThresholdをデフォルト値に設定します。");
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (player != null && collision.gameObject == player && playerStatus != null)
        {
            // プレイヤーとの距離をチェック
            float distance = Vector2.Distance(rb.position, player.transform.position);
            
            if (playerStatus.CanPushHeavyObject && canPushAgain && distance <= distanceThreshold)
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
        
        // 既存のループをキャンセル
        moveCts?.Cancel();
        moveCts = new System.Threading.CancellationTokenSource();
        
        Debug.Log("PushObject: 押し操作開始");
        MoveLoop(moveCts.Token).Forget();
    }

    private void StopPushing()
    {
        if (!isPushing) return;
        
        isPushing = false;
        
        // ループをキャンセル
        moveCts?.Cancel();
        moveCts = null;
        
        Debug.Log("StopPushing: 押し操作停止");
        
        // 物理状態を確実にリセット
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            StopPushing();
        }
    }
    private async UniTaskVoid MoveLoop(System.Threading.CancellationToken cancellationToken)
    {
        if (isLoopRunning)
        {
            Debug.LogWarning("MoveLoop: すでに実行中のループがあります");
            return;
        }
        
        isLoopRunning = true;
        Vector2 lastValidXPosition = rb.position;
        bool isDistanceExceeded = false;
        
        Debug.Log("MoveLoop: ループ開始");
        
        try
        {
            while (isPushing && !cancellationToken.IsCancellationRequested)
            {
                if (rb != null && playerRb != null)
                {
                    // プレイヤーとの距離を計算
                    float distance = Vector2.Distance(rb.position, playerRb.position);
                    
                    // デバッグ情報
                   //Debug.Log($"距離: {distance}, 閾値: {distanceThreshold}, isPushing: {isPushing}");

                    if (distance > distanceThreshold)
                    {
                        //Debug.Log("プレイヤーが離れすぎています - 動きを制限します");
                        
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
            Debug.Log("MoveLoop: キャンセルされました");
        }
        finally
        {
            isLoopRunning = false;
            Debug.Log("MoveLoop: ループ終了");
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
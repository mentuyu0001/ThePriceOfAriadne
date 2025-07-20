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
            if (playerStatus.CanPushHeavyObject)
            {
                // X軸,y軸の移動制限を解除（Z軸回転は禁止）
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                if (!isPushing)
                {
                    PushObject();
                }
            }
            else
            {
                // 全て固定（X軸、Y軸移動とZ軸回転すべて禁止）
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                if (isPushing)
                {
                    StopPushing();
                }
            }
        }
    }

    private void PushObject()
    {
        isPushing = true;
        MoveLoop().Forget(); // 非同期ループを開始
    }

    private void StopPushing()
    {
        isPushing = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            StopPushing();
        }
    }
    private async UniTaskVoid MoveLoop()
    {
        while (isPushing)
        {
            if (rb != null && playerRb != null)
            {
                // プレイヤーとの距離を計算
                float distance = Vector2.Distance(rb.position, playerRb.position);

                // 離れすぎている場合は速度を調整
                if (distance > distanceThreshold)
                {
                    // HeavyObjectの速度を0にする
                    rb.linearVelocity = Vector2.zero;
                }
                else
                {
                    // プレイヤーの速度を直接適用（ただし最大速度を超えないように制限）
                    rb.linearVelocity = Vector2.ClampMagnitude(playerRb.linearVelocity, maxSpeed);
                }

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }
    }
}
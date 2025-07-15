using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Parts.Types;

/// <summary>
/// プレイヤーが操作するコントローラーのスクリプト
/// </summary>

public class Controller : MonoBehaviour
{
    private float maxSpeed; // 最大スピード
    private float jumpForce; // ジャンプ力
    private Vector2 moveInput = Vector2.zero;
    private bool isMoving = false;
    private Rigidbody2D rb; // Rigidbodyを追加
    private Collider2D col; // Collider2Dを追加
    [SerializeField] private LayerMask groundLayer; // 地面のレイヤーを指定
    [SerializeField] private PlayerStatus playerStatus; // プレイヤーステータスを取得
    [SerializeField] private PlayerParts playerParts; // プレイヤーパーを取得

    // 摩擦の設定
    private float friction;
    private float airResistance;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbodyを取得
        if (rb == null)
        {
            Debug.LogError("Rigidbodyがアタッチされていません！");
        }

        col = GetComponent<Collider2D>(); // Colliderの取得

        if (col == null)
        {
            Debug.LogError("Colliderがアタッチされていません！");
        }

        if (col.sharedMaterial == null)
        {
            Debug.LogError("Physics Materialがアタッチされていません！");
        }
    }

    // Controllerに能力を反映させる
    public void SetStatus() 
    {
        // 移動速度に応じて抵抗を変更する
        friction = playerStatus.Friction;
        airResistance = playerStatus.AirResistance;

        // 最大スピードと最大ジャンプ力を変更する
        maxSpeed = playerStatus.MoveSpeed;
        jumpForce = playerStatus.JumpForce;

        if (col == null) {
            col = GetComponent<Collider2D>(); // Colliderの取得
        }

        // 新しい Material を設定
        col.sharedMaterial.friction = friction;
        // Debug.Log(col.sharedMaterial.friction);

        // Unity 6向けの強制更新処理を追加
        col.enabled = false;
        col.enabled = true;
        // 他に良い実装方法あったら募集
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
            if (!isMoving)
            {
                // 動き始めたら摩擦を0にする
                col.sharedMaterial.friction = 0;
                // physicsマテリアルの更新
                col.enabled = false;
                col.enabled = true;

                isMoving = true;
                MoveLoop().Forget(); // 非同期ループを開始
            }
        }
        else if (context.canceled)
        {
            // 止まってる場合は摩擦を有効化する
            col.sharedMaterial.friction = friction;
            // physicsマテリアルの更新
            col.enabled = false;
            col.enabled = true;

            moveInput = Vector2.zero;
            isMoving = false;
        }
    }

    private async UniTaskVoid MoveLoop()
    {
        while (isMoving)
        {
            float targetVelocityX, forceX = 0f;

            if (rb != null)
            {
                // 入力方向(moveInput.x)に最大速度を掛け合わせる
                targetVelocityX = maxSpeed * moveInput.x;
                // (目標速度 - 現在の速度) / 時間 = 必要な加速度
                // これに質量を掛けたものが力になる (AddForceは質量を考慮してくれる)
                forceX = (targetVelocityX - rb.linearVelocity.x);

                // レイキャストで地面判定
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
                if (hit.collider == null)
                {
                    // 空中にいる場合は移動の強さを弱める
                    forceX /= airResistance;
                }
            }
            // 水平方向に力を加える (垂直方向の動きには影響を与えない)
            rb.AddForce(new Vector2(forceX, 0f));

            await UniTask.Yield(PlayerLoopTiming.Update); // 毎フレーム待機
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && rb != null)
        {
            // レイキャストで地面判定
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
            if (hit.collider != null)
            {
                // ヒットしたオブジェクトとの距離をデバッグログに出力
                // Debug.Log($"Hit object: {hit.collider.name}, Distance: {hit.distance}");
                
                // 地面にいる場合のみジャンプ処理を実行
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                // Debug.Log("Jumping");
            }
            else
            {
                // Debug.Log("No ground detected.");
            }
        }
    }
}

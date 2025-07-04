using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class Controller : MonoBehaviour
{
    private float maxSpeed = 10f;
    private float force = 1f;//加速度の大きさ
    private Vector2 moveInput = Vector2.zero;
    private bool isMoving = false;
    private Rigidbody2D rb; // Rigidbodyを追加
    private float jumpForce = 5f; // ジャンプ力
    [SerializeField] private LayerMask groundLayer; // 地面のレイヤーを指定

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbodyを取得
        if (rb == null)
        {
            Debug.LogError("Rigidbodyがアタッチされていません！");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
            if (!isMoving)
            {
                isMoving = true;
                MoveLoop().Forget(); // 非同期ループを開始
            }
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
            isMoving = false;
        }
    }

    private async UniTaskVoid MoveLoop()
    {
        while (isMoving)
        {
            if (rb != null)
            {
                // 入力方向(moveInput.x)に最大速度を掛け合わせる
                float targetVelocityX = maxSpeed * moveInput.x;
                // (目標速度 - 現在の速度) / 時間 = 必要な加速度
                // これに質量を掛けたものが力になる (AddForceは質量を考慮してくれる)
                float forceX = (targetVelocityX - rb.linearVelocity.x);

                // 水平方向に力を加える (垂直方向の動きには影響を与えない)
                rb.AddForce(new Vector2(forceX, 0f));
            }
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
                Debug.Log($"Hit object: {hit.collider.name}, Distance: {hit.distance}");
                
                // 地面にいる場合のみジャンプ処理を実行
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                Debug.Log("Jumping");
            }
            else
            {
                Debug.Log("No ground detected.");
            }
        }
    }
}

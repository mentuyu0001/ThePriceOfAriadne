using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Parts.Types;

public class Controller : MonoBehaviour
{
    private float maxSpeed; // 最大スピード
    private float jumpForce; // ジャンプ力
    private float force = 1f;//加速度の大きさ
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

    // 衝突判定で使う変数
    [Header("壁判定の設定")]
    [Tooltip("壁と判定する地面からの最小角度")]
    [SerializeField, Range(30f, 90f)] private float wallAngleThreshold = 45f;
    [Tooltip("壁に張り付かないようにするための、摩擦ゼロのマテリアル")]
    [SerializeField] private PhysicsMaterial2D slipperyMaterial;
    private PhysicsMaterial2D originalMaterial;
    private int wallContactCount = 0; // 壁との接触数を数えるカウンター


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

        // 既存の PhysicsMaterial2D をコピーして変更（元を直接いじると他にも影響する）

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
            float targetVelocityX, forceX = 0f;

            // Debug.Log(col.sharedMaterial.friction);

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
                    // 空中にいる場合は移動の強さを1/10にする
                    forceX /= airResistance;   // ちょっとマジックナンバーすぎるかも。修正案あれば。
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

    /*
    // 壁との接触判定を考慮するメソッド
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 接触した点の法線をチェック
        foreach (var contact in collision.contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            if (angle > wallAngleThreshold)
            {
                // 壁との接触なのでカウンターを増やす
                wallContactCount++;
                break; // この衝突判定では壁だと分かったのでループを抜ける
            }
        }

        // 壁との接触が1つ以上あれば、滑るマテリアルに切り替える
        if (wallContactCount > 0 && col.sharedMaterial != slipperyMaterial)
        {
            col.sharedMaterial = slipperyMaterial;
        }
    }

    // オブジェクトから離れた瞬間に呼ばれる
    private void OnCollisionExit2D(Collision2D collision)
    {
        // OnCollisionExit2Dでは衝突「点」が取得できないため、
        // 離れたオブジェクトが壁だったかを判定するのは難しい。
        // そのため、ここでは一旦すべての接触が壁だった可能性を考慮してカウンターを減らす。
        if (wallContactCount > 0)
        {
            wallContactCount = 0;
        }

        // 壁との接触がなくなった場合、元のマテリアルに戻す
        if (wallContactCount == 0 && col.sharedMaterial != originalMaterial)
        {
            col.sharedMaterial = originalMaterial;
        }
    }
    */
}

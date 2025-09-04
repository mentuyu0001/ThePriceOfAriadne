using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Parts.Types;
using VContainer;

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
    [SerializeField] private Vector2 sizeModifier = new Vector2(1.0f, 0.1f); // レイを飛ばす際のコライダーサイズ 例：幅は90%、高さは20%
    [SerializeField] private float groundCheckBuffer = 0f; // コライダーの底辺から伸ばすレイの長さ
    
    // VContainerで注入されるコンポーネント
    [Inject] 
    private PlayerStatus playerStatus; // プレイヤーステータスを取得
    [Inject] 
    private PlayerParts playerParts; // プレイヤーパーツを取得
    [Inject] 
    private PlayerAirChecker airChecker; // プレイヤーの空中判定を行うスクリプトの取得
    [Inject] 
    private PlayerRunTimeStatus runTimeStatus; // パーツによって得られるステータス情報の取得
    [Inject] 
    private PlayerAnimationManager playerAnimationManager; // プレイヤーアニメーションマネージャーの取得
    
    // 摩擦の設定
    private float friction;
    private float airResistance;

    public bool isInputEnabled = true; // 入力を受け付けるかどうかのフラグ

    private void Start()
    {
        Debug.Log("=== Controller 依存性注入確認 ===");
        
        // [Inject]による注入の確認
        if (playerStatus != null)
        {
            Debug.Log($"✅ PlayerStatus注入成功: {playerStatus.name} (Type: {playerStatus.GetType()})");
        }
        else
        {
            Debug.LogError("❌ PlayerStatus注入失敗 - nullです");
        }

        if (playerParts != null)
        {
            Debug.Log($"✅ PlayerParts注入成功: {playerParts.name} (Type: {playerParts.GetType()})");
        }
        else
        {
            Debug.LogError("❌ PlayerParts注入失敗 - nullです");
        }

        if (airChecker != null)
        {
            Debug.Log($"✅ PlayerAirChecker注入成功: {airChecker.name} (Type: {airChecker.GetType()})");
        }
        else
        {
            Debug.LogError("❌ PlayerAirChecker注入失敗 - nullです");
        }

        if (runTimeStatus != null)
        {
            Debug.Log($"✅ PlayerRunTimeStatus注入成功: {runTimeStatus.name} (Type: {runTimeStatus.GetType()})");
        }
        else
        {
            Debug.LogError("❌ PlayerRunTimeStatus注入失敗 - nullです");
        }

        if (playerAnimationManager != null)
        {
            Debug.Log($"✅ PlayerAnimationManager注入成功: {playerAnimationManager.name} (Type: {playerAnimationManager.GetType()})");
        }
        else
        {
            Debug.LogError("❌ PlayerAnimationManager注入失敗 - nullです");
        }

        Debug.Log("=== Controller 依存性注入確認完了 ===");

        rb = GetComponent<Rigidbody2D>(); // Rigidbodyを取得
        if (rb == null)
        {
            Debug.LogError("Rigidbodyがアタッチされていません！");
        }
        else
        {
            Debug.Log("Rigidbody2D取得成功");
        }

        col = GetComponent<Collider2D>(); // Colliderの取得
        if (col == null)
        {
            Debug.LogError("Colliderがアタッチされていません！");
        }
        else
        {
            Debug.Log("Collider2D取得成功");
        }

        if (col != null && col.sharedMaterial == null)
        {
            Debug.LogError("Physics Materialがアタッチされていません！");
        }
        else if (col != null)
        {
            Debug.Log("Physics Material確認成功");
        }
    }

    // Controllerに能力を反映させる
    public void SetStatus()
    {
        if (playerStatus == null)
        {
            Debug.LogError("SetStatus呼び出し時にPlayerStatusがnullです");
            return;
        }

        // 移動速度に応じて抵抗を変更する
        friction = playerStatus.Friction;
        airResistance = playerStatus.AirResistance;

        // 最大スピードと最大ジャンプ力を変更する
        maxSpeed = playerStatus.MoveSpeed;
        jumpForce = playerStatus.JumpForce;

        Debug.Log($"ステータス設定完了 - Speed: {maxSpeed}, Jump: {jumpForce}, Friction: {friction}, AirResistance: {airResistance}");

        if (col == null)
        {
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
        if (!isInputEnabled) return; // 入力が無効な場合は何もしない
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

                // Walkアニメーションの開始
                if (playerAnimationManager != null)
                {
                    playerAnimationManager.AniWalkTrue();
                }
                else
                {
                    Debug.LogError("playerAnimationManager is null in OnMove");
                }

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

            // Walkアニメーションの停止
            if (playerAnimationManager != null)
            {
                playerAnimationManager.AniWalkFalse();
            }
            else
            {
                Debug.LogError("playerAnimationManager is null in OnMove");
            }
        }
    }

    private async UniTaskVoid MoveLoop()
    {
        while (isMoving)
        {
            float targetVelocityX, forceX = 0f;

            if (rb != null)
            {
                // 移動方向に応じてプレイヤー画像を反転させる
                // 右向きならY軸の角度を0、左向きなら180にする
                if (moveInput.x == 1) {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                } else if (moveInput.x == -1) {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                // 入力方向(moveInput.x)に最大速度を掛け合わせる
                targetVelocityX = maxSpeed * moveInput.x;
                // (目標速度 - 現在の速度) / 時間 = 必要な加速度
                // これに質量を掛けたものが力になる (AddForceは質量を考慮してくれる)
                forceX = (targetVelocityX - rb.linearVelocity.x);
                if (airChecker != null && !airChecker.IsGround)
                {
                    // 空中にいる場合は移動の強さを弱める
                    forceX /= airResistance;
                }
                else if (airChecker == null)
                {
                    Debug.LogError("airChecker is null in MoveLoop");
                }
            }
            // 水平方向に力を加える (垂直方向の動きには影響を与えない)
            rb.AddForce(new Vector2(forceX, 0f));

            await UniTask.Yield(PlayerLoopTiming.Update); // 毎フレーム待機
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isInputEnabled) return; // 入力が無効な場合は何もしない
        if (context.performed && rb != null)
        {
            if (airChecker != null && airChecker.IsGround)
            {
                // 地面にいる場合のみジャンプ処理を実行
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                // ジャンプアニメーションの開始
                if (playerAnimationManager != null)
                {
                    playerAnimationManager.AniJumpTrue();
                }
            }
            else if (airChecker == null)
            {
                Debug.LogError("airChecker is null in OnJump");
            }
            else
            {
                // Debug.Log("No ground detected.");
                // 二段ジャンプの処理
                if (runTimeStatus != null && runTimeStatus.CanDoubleJump)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // y成分の速さを0にしてからジャンプの力を入れる
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    runTimeStatus.CanDoubleJump = false;

                    // ジャンプアニメーションの開始
                    if (playerAnimationManager != null)
                    {
                        playerAnimationManager.AniJumpTrue();
                    }
                }
                else if (runTimeStatus == null)
                {
                    Debug.LogError("runTimeStatus is null in OnJump");
                }
            }
        }

        // Debug.Log(airChecker.IsGround);
    }

    // 移動入力状態をリセットするメソッド
    public void ResetMoveInput()
    {
        // 移動入力をゼロにリセット
        moveInput = Vector2.zero;
        
        // 移動フラグをリセット
        isMoving = false;
        
        // 必要に応じて他の移動関連の状態もリセット
        // 例: ジャンプフラグなど
    }
}

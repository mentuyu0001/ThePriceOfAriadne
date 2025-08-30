using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// Enteキーで発生するイベントを呼び出すスクリプト
/// </summary>
public class EnterKeyActionTrigger : MonoBehaviour
{
    // マップに落ちているパーツオブジェクトのタグ
    [SerializeField] private string partsTag = "Parts";
    // PlartsManagerの参照
    [SerializeField] private PartsManager partsManager;
    // プレイヤーの参照
    [SerializeField] private GameObject player;
    // 錆びたレバーのタグ
    [SerializeField] private string leverTag = "RustyLever";
    // ストップボタンのタグ
    [SerializeField] private string stopButtonTag = "StopButton";
    // 水タンク用のタグ
    [SerializeField] private string waterTankTag = "WaterTank";
    // 燃え盛る炎のタグ
    [SerializeField] private string burningFireTag = "BurningFire";
    // AnimationManagerの参照
    [SerializeField] private PlayerAnimationManager playerAnimationManager;

    // 接触しているコライダー
    private Collider2D touchingCollision = null;
    // デバッグ用
    [SerializeField] private bool showDebugLogs = false;

    // クラス変数として追加
    private bool isInteracting = false;
    private float lastInteractionTime = 0f;
    private float interactionCooldown = 0.5f; // クールダウン時間（秒）
    private Controller controller;

    // 物理移動制限用の変数を追加
    private Rigidbody2D playerRigidbody;
    private RigidbodyConstraints2D originalConstraints;

    private Vector3 fixedPosition;
    private bool keepPositionFixed = false;

    // Unityの初期化処理
    private void Start()
    {
        // 起動時に周囲のオブジェクトを確認
        CheckSurroundingObjects();
        // コントローラーのコンポーネントを取得
        controller = player.GetComponent<Controller>();
        // プレイヤーのRigidbody2Dを取得
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        // 初期のconstraints値を保存（あとで戻すため）
        if (playerRigidbody != null)
        {
            originalConstraints = playerRigidbody.constraints;
        }
    }

    // オブジェクトに干渉する or ナイフを投げるメソッドを実行
    public void OnEnterKeyAction()
    {
        // 現在時刻を取得
        float currentTime = Time.time;

        // 1. 実行中チェック
        if (isInteracting)
        {
            if (showDebugLogs) Debug.Log("インタラクション実行中のため無視");
            return;
        }

        // 2. クールダウンチェック
        if (currentTime - lastInteractionTime < interactionCooldown)
        {
            if (showDebugLogs) Debug.Log("クールダウン中のため無視");
            return;
        }

        // フラグをセットして実行開始
        isInteracting = true;
        lastInteractionTime = currentTime;

        try
        {
            // 以下、元のインタラクションコード
            bool interacted = false;

            // 何かと接触していれば、そのオブジェクトとインタラクションを試みる
            if (touchingCollision != null)
            {
                if (touchingCollision.gameObject.CompareTag(partsTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<MapParts>();
                    if (component != null)
                    {
                        // インタラクトアニメーションを実行
                        IntractAnimation();
                        partsManager.ExchangeParts(component);
                        interacted = true;
                        if (showDebugLogs) Debug.Log("パーツとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(leverTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<RustyLever>();
                    if (component != null)
                    {
                        // インタラクトアニメーションを実行
                        IntractAnimation();
                        component.RotateLever();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("レバーとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(stopButtonTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<StopButton>();
                    if (component != null)
                    {
                        // インタラクトアニメーションを実行
                        IntractAnimation();
                        component.PressButton();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("ストップボタンとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(waterTankTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<WaterTank>();
                    if (component != null)
                    {
                        // インタラクトアニメーションを実行
                        IntractAnimation();
                        component.ChargeWater();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("水タンクとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(burningFireTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<BurningFireCheckZone>();
                    if (component != null)
                    {
                        // インタラクトアニメーションを実行
                        IntractAnimation();
                        component.FireExtinguished();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("燃え盛る炎とインタラクトしました");
                    }
                }
            }

            // インタラクションがなかった場合、ナイフを投げる
            if (!interacted)
            {
                if (showDebugLogs) Debug.Log("インタラクションなし: ナイフを投げます");
                // Playerオブジェクトからナイフを投げるコンポーネントを取得と実行
                ThrowKnifeController throwKnife = player.GetComponent<ThrowKnifeController>();
                // プレイヤーのThrowKnifeメソッドを呼び出す
                throwKnife.ThrowKnife();
            }
        }
        finally
        {
            // 処理完了後、フラグをリセット
            isInteracting = false;
        }
    }

    // 起動時に周囲のオブジェクトをチェック
    private void CheckSurroundingObjects()
    {
        // プレイヤーの周囲にあるインタラクト可能なオブジェクトを検出
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag(partsTag) ||
                collider.gameObject.CompareTag(leverTag) ||
                collider.gameObject.CompareTag(stopButtonTag) ||
                collider.gameObject.CompareTag(waterTankTag) ||
                collider.gameObject.CompareTag(burningFireTag))
            {
                // すでに接触しているオブジェクトを設定
                touchingCollision = collider;
                break;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(partsTag) ||
            collision.gameObject.CompareTag(leverTag) ||
            collision.gameObject.CompareTag(stopButtonTag) ||
            collision.gameObject.CompareTag(waterTankTag) ||
            collision.gameObject.CompareTag(burningFireTag))
        {
            touchingCollision = collision;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (touchingCollision == collision)
        {
            touchingCollision = null;
        }
    }

    // コライダーのデバッグ描画
    private void OnDrawGizmos()
    {
        if (showDebugLogs)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1.0f);
        }
    }

    // インタラクトアニメーションを実行するメソッド
    private void IntractAnimation()
    {
        // 入力を無効化
        if (controller != null)
            controller.isInputEnabled = false;
        
        if (playerRigidbody != null)
        {
            // 現在の速度をゼロに
            playerRigidbody.linearVelocity = Vector2.zero;
            
            // 現在位置を記録
            fixedPosition = player.transform.position;
            
            // キネマティックモードに設定
            playerRigidbody.isKinematic = true;
            
            // 位置固定フラグを有効化
            keepPositionFixed = true;
        }
        
        // インタラクトのアニメーションを開始
        playerAnimationManager.AniInteractTrue();
        
        // アニメーション完了を監視する非同期処理を開始
        WaitForAnimationCompletion().Forget();
    }

    // アニメーションの完了を待つ非同期メソッド
    private async UniTask WaitForAnimationCompletion()
    {
        // プレイヤーのAnimatorコンポーネントを取得
        Animator animator = player.GetComponent<Animator>();

        // 実際のアニメーション完了を検出する方法（アニメーション長に依存しない）
        if (animator != null)
        {
            int layerIndex = 0;
            float checkInterval = 0.05f;
            float maxWaitTime = 3.0f;  // 最大待機時間（安全対策）
            float elapsedTime = 0f;
            
            while (elapsedTime < maxWaitTime)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
                
                // インタラクトアニメーションが完了したかチェック
                // "Interact"の部分は実際のアニメーション名に置き換えてください
                if ((stateInfo.IsName("Interact") || stateInfo.IsName("Base Layer.Interact")) && 
                    stateInfo.normalizedTime >= 0.95f)
                {
                    break;  // アニメーション完了
                }
                
                await UniTask.Delay(System.TimeSpan.FromSeconds(checkInterval));
                elapsedTime += checkInterval;
            }
        }
        else
        {
            // アニメーターがない場合はフォールバックとして固定時間待機
            await UniTask.Delay(1250); 
        }
        
        // アニメーション完了後の処理
        // アニメーションフラグをオフ
        playerAnimationManager.AniInteractFalse();
        
        // 入力を再度有効化する前に、Controllerの移動入力状態をリセット
        if (controller != null)
        {
            // Controllerの入力状態をリセット（重要）
            controller.ResetMoveInput();  // この関数をControllerクラスに追加する必要があります
            
            // その後、入力を再度有効化
            controller.isInputEnabled = true;
        }
        
        // 位置固定フラグを解除
        keepPositionFixed = false;
        
        // キネマティックモードを解除して物理挙動を再開
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
            
            // 速度と慣性を確実にリセット
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
            
            // 追加の対策として、物理的な力も完全にリセット
            playerRigidbody.Sleep();
            playerRigidbody.WakeUp();
        }
    }
    
    private void FixedUpdate()
    {
        // キネマティックモードでも念のため位置を固定
        if (keepPositionFixed && player != null)
        {
            player.transform.position = fixedPosition;
        }
    }
}

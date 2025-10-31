using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using Parts.Types;

/// <summary>
/// Enteキーで発生するイベントを呼び出すスクリプト
/// </summary>
public class EnterKeyActionTrigger : MonoBehaviour
{
   
    // プレイヤーの参照
    [Inject] private GameObject player;
    // PlayerStatusの参照
    [Inject] private PlayerStatus playerStatus;
    // PlartsManagerの参照
    [Inject] private PartsManager partsManager;
    // AnimationManagerの参照
    [Inject] private PlayerAnimationManager playerAnimationManager;
    // PlayerRunTimeStatusの参照
    [Inject] private PlayerRunTimeStatus playerRunTimeStatus;
    // テキスト表示システムの参照
    [Inject] private GameTextDisplay textDisplay;
    [SerializeField] PartsData partsData;
     // マップに落ちているパーツオブジェクトのタグ
    [SerializeField] private string partsTag = "Parts";
    // 錆びたレバーのタグ
    [SerializeField] private string leverTag = "RustyLever";
    // ストップボタンのタグ
    [SerializeField] private string stopButtonTag = "StopButton";
    // 水タンク用のタグ
    [SerializeField] private string waterTankTag = "WaterTank";
    // 燃え盛る炎のタグ
    [SerializeField] private string burningFireTag = "BurningFire";

    // アニメーション待機時間の設定（SerializeField）
    [Header("アニメーション待機時間設定")]
    [SerializeField] private float interactAnimationDuration = 1.25f;  // Interactアニメーションの時間
    [SerializeField] private float leverAnimationDuration = 1.5f;      // Leverアニメーションの時間
    [SerializeField] private float buttonAnimationDuration = 1.0f;     // Buttonアニメーションの時間
    [SerializeField] private float shootWaterAnimationDuration = 3.0f; // ShootWaterアニメーションの時間
    [SerializeField] private float knifeAnimationDuration = 0.5f;      // Knifeアニメーションの時間

    // 表示するテキストメッセージ
    [Header("表示テキスト設定")]
    [SerializeField] private string partsGetMessage = "パーツを入手しました!";
    [SerializeField] private string leverUseMessage = "レバーを動かした!";
    [SerializeField] private string buttonUseMessage = "ボタンを押した!";
    [SerializeField] private string waterChargeMessage = "水を補給しました!";
    [SerializeField] private string fireExtinguishMessage = "火を消した!";
    [SerializeField] private string noWaterTankMessage = "水タンクが装備されていません";
    [SerializeField] private string noWaterMessage = "水がありません";

    // PlayerPartsRatioの参照
    [Inject] private PlayerPartsRatio partsRatio;

    // テキスト表示間のディレイ秒数
    [SerializeField] private float delayBetweenTexts = 2f; 

    // ObjectTextDataの参照
    [Inject] private ObjectTextData objectTextData;
    [Header("オブジェクトID設定")]
    [SerializeField] private int partsID = 0;    // パーツ取得時のID
    [SerializeField] private int leverID = 1;    // レバー使用時のID
    [SerializeField] private int buttonID = 5;   // ボタン使用時のID
    [SerializeField] private int waterTankID = 6; // 水タンク使用時のID
    [SerializeField] private int canselWaterTankID = 7; // 水タンクが使用不可時のID
    [SerializeField] private int fireExtinguishID = 8; // 火消し時のID


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
    private PlayerAirChecker airChecker; // 空中判定のコンポーネント
    private ThrowKnifeController throwKnife; // ナイフを投げるコンポーネント
    // Unityの初期化処理
    private void Start()
    {
        if (partsManager != null)
        {
            Debug.Log($"✅ PartsManager注入成功: {partsManager.name}");
        }
        else
        {
            Debug.LogError("❌ PartsManager注入失敗 - nullです");
        }

        if (player != null)
        {
            Debug.Log($"✅ Player注入成功: {player.name}");
        }
        else
        {
            Debug.LogError("❌ Player注入失敗 - nullです");
        }

        if (playerAnimationManager != null)
        {
            Debug.Log($"✅ PlayerAnimationManager注入成功: {playerAnimationManager.name}");
        }
        else
        {
            Debug.LogError("❌ PlayerAnimationManager注入失敗 - nullです");
        }
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
        // 空中判定のコンポーネントを取得
        airChecker = player.GetComponent<PlayerAirChecker>();
        // ナイフを投げるコンポーネントを取得と実行
        throwKnife = player.GetComponent<ThrowKnifeController>();

    }

    // オブジェクトに干渉する or ナイフを投げるメソッドを実行
    public void OnEnterKeyAction()
    {
        // テキスト表示中でEnterキー待ち状態の場合は何もしない
        // (GameTextDisplayがEnterキーを処理する)
        if (textDisplay != null && textDisplay.IsDisplaying)
        {
            if (showDebugLogs) Debug.Log("テキスト表示中のため、オブジェクトインタラクションはスキップ");
            return;
        }

        // 実行中チェックのみ
        if (isInteracting)
        {
            if (showDebugLogs) Debug.Log("インタラクション実行中のため無視");
            return;
        }

        // フラグをセットして実行開始
        isInteracting = true;

        try
        {
            // 以下、元のインタラクションコード
            bool interacted = false;

            // 地面と接地しているかどうかを確認
            bool isGrounded = airChecker.IsGround;

            // 何かと接触かつ地面にいる場合、そのオブジェクトとインタラクションを試みる
            if (touchingCollision != null && isGrounded)
            {
                if (touchingCollision.gameObject.CompareTag(partsTag))
                {
                    var component1 = touchingCollision.gameObject.GetComponent<MapParts>();
                    var component2 = touchingCollision.gameObject.GetComponent<MapPartsVisualCustomizer>();
                    if (component1 != null)
                    {
                        HandlePartsInteraction(component1, component2).Forget();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("パーツとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(leverTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<RustyLever>();
                    if (component != null && playerStatus.CanPushHeavyObject)
                    {
                        HandleLeverInteraction(component).Forget();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("レバーとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(stopButtonTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<StopButton>();
                    if (component != null)
                    {
                        HandleButtonInteraction(component).Forget();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("ストップボタンとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(waterTankTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<WaterTank>();
                    if (component != null)
                    {
                        HandleWaterTankInteraction(component).Forget();
                        interacted = true;
                        if (showDebugLogs) Debug.Log("水タンクとインタラクトしました");
                    }
                }
                else if (touchingCollision.gameObject.CompareTag(burningFireTag))
                {
                    var component = touchingCollision.gameObject.GetComponent<BurningFireCheckZone>();
                    if (component != null)
                    {
                        // プレイヤーの向きとCheckZoneの方向をチェック
                        if (IsPlayerFacingCorrectDirection(component))
                        {
                            HandleFireInteraction(component).Forget();
                            interacted = true;
                            if (showDebugLogs) Debug.Log("燃え盛る炎とインタラクトしました");
                        }
                        else
                        {
                            if (showDebugLogs) Debug.Log("炎の方向を向いていないため消火できません");
                        }
                    }
                }
            }

            // インタラクションがなかった場合、ナイフを投げる
            if (!interacted)
            {
                if (isGrounded == false)
                {
                    // アニメーター側で設定したため不要
                    // ジャンプアニメーションをキャンセル
                    //playerAnimationManager.AniJumpFalse();
                    //playerAnimationManager.AniDoubleJumpFalse();

                }
                HandleKnifeThrow().Forget();
                if (showDebugLogs) Debug.Log("インタラクションなし: ナイフを投げます");
            }
        }
        finally
        {
            // 非同期処理の場合はここではリセットしない
            // 各個別のハンドラー内でリセットする
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
            textDisplay.HideText();
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

    // アニメーション開始の準備
    private void PrepareForAnimation()
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
    }
    private void FixedUpdate()
    {
        // キネマティックモードでも念のため位置を固定
        if (keepPositionFixed && player != null)
        {
            player.transform.position = fixedPosition;
        }
    }

    // パーツインタラクション処理
    private async UniTaskVoid HandlePartsInteraction(MapParts component1, MapPartsVisualCustomizer component2)
    {
        try
        {
            PrepareForAnimation();
            playerAnimationManager.AniInteractTrue();

            // パーツチェンジSE（id:4）を再生
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(4);
            }

            // MapPartsからキャラ・部位を取得
            var chara = component1.CharaType;
            var slot = component1.SlotType;

            // PartsDataから形容詞を取得
            var info = partsData.GetPartsInfoByPartsChara(chara);
            string adjective = info != null ? info.adjective : "";

            // 部位名を日本語に変換
            string slotName = slot switch
            {
                PartsSlot.LeftArm => "左腕",
                PartsSlot.RightArm => "右腕",
                PartsSlot.LeftLeg => "左脚",
                PartsSlot.RightLeg => "右脚",
                _ => "部位"
            };

            // テキスト生成
            string getMessage = $"{adjective}{slotName}に切り替えた。";

            await WaitForAnimationCompletion(interactAnimationDuration);

            partsManager.ExchangeParts(component1, component2);
            ResteControllerInput();
            await textDisplay.ShowText(getMessage);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"パーツインタラクション処理中にエラー: {e.Message}");
            ResteControllerInput();
        }
        finally
        {
            isInteracting = false;
        }
    }

    // レバーインタラクション処理
    private async UniTaskVoid HandleLeverInteraction(RustyLever component)
    {
        try
        {
            PrepareForAnimation();
            playerAnimationManager.AniLeverTrue();
            component.RotateLever();
            await WaitForAnimationCompletion(leverAnimationDuration);
            ResteControllerInput();
            await textDisplay.ShowText(leverUseMessage);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"レバーインタラクション処理中にエラー: {e.Message}");
            ResteControllerInput();
        }
        finally
        {
            isInteracting = false;
        }
    }

    // ボタンインタラクション処理
    private async UniTaskVoid HandleButtonInteraction(StopButton component)
    {
        try
        {
            PrepareForAnimation();
            playerAnimationManager.AniButtonTrue();

            // ボタンを押すSE（id:8）を再生
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(8);
            }

            component.PressButton();
            await WaitForAnimationCompletion(buttonAnimationDuration);
            ResteControllerInput();
            textDisplay.ShowTextByPartsRatio(
                partsRatio,
                objectTextData,
                buttonID,
                delayBetweenTexts,
                showDebugLogs
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ボタンインタラクション処理中にエラー: {e.Message}");
            ResteControllerInput();
        }
        finally
        {
            isInteracting = false;
        }
    }

    // 水タンクインタラクション処理
    private async UniTaskVoid HandleWaterTankInteraction(WaterTank component)
    {
        try
        {
            if (!playerStatus.CanChargeWater)
            {
                if (textDisplay != null)
                {
                    textDisplay.ShowTextByPartsRatio(
                        partsRatio,
                        objectTextData,
                        canselWaterTankID,
                        delayBetweenTexts,
                        showDebugLogs
                    );
                }

                isInteracting = false;
                return;
            }

            PrepareForAnimation();
            playerAnimationManager.AniInteractTrue();
            component.ChargeWater();
            await WaitForAnimationCompletion(interactAnimationDuration);
            ResteControllerInput();
            
            textDisplay.ShowTextByPartsRatio(
                partsRatio,
                objectTextData,
                waterTankID,
                delayBetweenTexts,
                showDebugLogs
            );
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"水タンクインタラクション処理中にエラー: {e.Message}");
            ResteControllerInput();
        }
        finally
        {
            isInteracting = false;
        }
    }

    // 消火インタラクション処理
    private async UniTaskVoid HandleFireInteraction(BurningFireCheckZone component)
    {
        try
        {
            if (!playerRunTimeStatus.CanShootWater)
            {
                if (textDisplay != null)
                {
                    await textDisplay.ShowText(noWaterMessage);
                }
                
                isInteracting = false;
                return;
            }
            
            PrepareForAnimation();
            playerAnimationManager.AniShootWaterTrue();
            component.FireExtinguishedAsync();
            // 放射のためモーションを待機(1.17秒)
            await WaitForAnimationCompletion(1.17f);

            // 水を発射するSE（id:7）を再生
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(7);
            }
            await WaitForAnimationCompletion(shootWaterAnimationDuration);
            // 消火SEを停止
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopSE();
            }
            ResteControllerInput();
            //await textDisplay.ShowText(fireExtinguishMessage);
            /*
            textDisplay.ShowTextByPartsRatio(
                partsRatio,
                objectTextData,
                fireExtinguishID,
                delayBetweenTexts,
                showDebugLogs
            );
            */
        }
        catch (System.Exception e)
        {
            Debug.LogError($"火災インタラクション処理中にエラー: {e.Message}");
            ResteControllerInput();
        }
        finally
        {
            isInteracting = false;
        }
    }

    // ナイフ投げ処理
    private async UniTaskVoid HandleKnifeThrow()
    {
        try
        {
            if (!playerRunTimeStatus.CanThrowKnife)
            {
                if (showDebugLogs) Debug.Log("ナイフを投げるパーツが装備されていません");
                isInteracting = false;
                return;
            }
            controller.isInputEnabled = false;

            // ナイフを投げるSE（id:9）を再生
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(9);
            }

            throwKnife.ThrowKnife();
            playerAnimationManager.AniKnifeTrue();
            await WaitForAnimationCompletion(knifeAnimationDuration);
            controller.isInputEnabled = true;

            if (showDebugLogs) Debug.Log("インタラクションなし: ナイフを投げました");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ナイフ投げ処理中にエラー: {e.Message}");
            controller.isInputEnabled = true;
        }
        finally
        {
            isInteracting = false;
        }
    }

    // コントロールの入力を有効化するメソッド
    private void ResteControllerInput()
    {
        // 入力を再度有効化する前に、Controllerの移動入力状態をリセット
        if (controller != null)
        {
            // Controllerの入力状態をリセット（重要）
            controller.ResetMoveInput();
            
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

    // アニメーションの完了を待つ非同期メソッド（時間指定版）
    private async UniTask WaitForAnimationCompletion(float duration, CancellationToken cancellationToken = default)
    {
        if (showDebugLogs) Debug.Log($"アニメーション待機開始: {duration}秒間入力を無効化");
        
        // 指定された時間だけ待機
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
        
        if (showDebugLogs) Debug.Log($"アニメーション待機完了: {duration}秒経過、入力を再有効化");
    }

    // プレイヤーが正しい方向を向いているかチェックするメソッド
    private bool IsPlayerFacingCorrectDirection(BurningFireCheckZone checkZone)
    {
        // プレイヤーの向きを判定
        bool playerFacingRight = GetPlayerDirection();
        
        // CheckZoneの方向を取得
        bool checkZoneIsRight = checkZone.isRightCheckZone;
        
        // プレイヤーの向きとCheckZoneの方向が一致している場合のみtrue
        bool canExtinguish = playerFacingRight != checkZoneIsRight;
        
        if (showDebugLogs)
        {
            Debug.Log($"プレイヤーの向き: {(playerFacingRight ? "右" : "左")}, " +
                     $"CheckZone方向: {(checkZoneIsRight ? "右" : "左")}, " +
                     $"消火可能: {canExtinguish}");
        }
        
        return canExtinguish;
    }
    
    // プレイヤーの向きを取得するメソッド
    private bool GetPlayerDirection()
    {
        if (player == null) return true; // デフォルトは右向き

        /*
        // 方法1: localScale.xで判定（スプライト反転の場合）
        if (player.transform.localScale.x < 0)
        {
            return false; // 左向き
        }
        */
        
        
        // 方法2: Y軸回転で判定（回転による向き変更の場合）
        float yRotation = player.transform.eulerAngles.y;
        if (yRotation > 90f && yRotation < 270f)
        {
            return false; // 左向き
        }
        
        
        return true; // 右向き
    }

    // デバッグ用：現在のアニメーション状態を確認するメソッド
    private void DebugCurrentAnimation()
    {
        if (showDebugLogs && player != null)
        {
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                
                if (clipInfo.Length > 0)
                {
                    Debug.Log($"現在のアニメーション: {clipInfo[0].clip.name}, normalizedTime: {stateInfo.normalizedTime}");
                }
            }
        }
    }
}

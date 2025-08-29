using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// マップ上のオブジェクトを判定し，それに応じたイベントを呼び出すスクリプト
/// </summary>

public class ObjectInteractionTrigger : MonoBehaviour
{
    // マップに落ちているパーツオブジェクトのタグ
    [SerializeField] private string partsTag = "Parts";
    // PlayerManagerの参照
    [SerializeField] private PartsManager partsManager;
    // 錆びたレバーのタグ
    [SerializeField] private string leverTag = "RustyLever";
    // ストップボタンのタグ
    [SerializeField] private string stopButtonTag = "StopButton";
    // 水タンク用のタグ
    [SerializeField] private string waterTankTag = "WaterTank";
    // 燃え盛る炎のタグ
    [SerializeField] private string burningFireTag = "BurningFire";
    // 決定ボタンの入力アクション
    [SerializeField] private InputActionProperty interactAction;
    // 接触しているコライダー
    private Collider2D touchingCollision = null;
    // デバッグ用
    [SerializeField] private bool showDebugLogs = false;

    // Unityの初期化処理
    private void Awake()
    {
        // 入力アクションを有効化
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.Enable();
        }
    }

    // Unityの初期化処理
    private void Start()
    {
        // マップに落ちているパーツにインタラクト可能にする
        InteractParts(this.GetCancellationTokenOnDestroy()).Forget();
        InteractLever(this.GetCancellationTokenOnDestroy()).Forget();
        // ストップボタンにインタラクト可能にする
        InteractStopButton(this.GetCancellationTokenOnDestroy()).Forget();
        // 水タンクにインタラクト可能にする
        InteractWaterTank(this.GetCancellationTokenOnDestroy()).Forget();
        // 燃え盛る炎にインタラクト可能にする
        InteractBurningFire(this.GetCancellationTokenOnDestroy()).Forget();
        
        // 起動時に周囲のオブジェクトを確認
        CheckSurroundingObjects();
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
                collider.gameObject.CompareTag(waterTankTag)||
                collider.gameObject.CompareTag(burningFireTag))
            {
                // すでに接触しているオブジェクトを設定
                touchingCollision = collider;
                if (showDebugLogs) Debug.Log($"起動時に検出: {collider.gameObject.name}");
                break;
            }
        }
    }

    // Interactの共通部分   
    async UniTask Interact(CancellationToken ct)
    {
        
    }

    private async UniTask Interact<T>(string tag, System.Action<T> onInteract, CancellationToken ct) where T : Component
    {
        while (!ct.IsCancellationRequested)
        {
            await interactAction.action.OnStartedAsync(ct);
            
            //if (showDebugLogs) Debug.Log($"インタラクション試行: 現在のコライダー {(touchingCollision ? touchingCollision.gameObject.name : "なし")}");
            
            if (touchingCollision == null || !touchingCollision.gameObject.CompareTag(tag))
            {
                //if (showDebugLogs) Debug.Log($"対象外オブジェクト: {(touchingCollision ? touchingCollision.gameObject.name : "なし")}");
                continue;
            }

            var component = touchingCollision.gameObject.GetComponent<T>();
            if (component == null)
            {
                //Debug.LogError($"{typeof(T).Name} コンポーネントが見つかりません。対象: {touchingCollision.gameObject.name}");
                continue;
            }

            //if (showDebugLogs) Debug.Log($"{typeof(T).Name} と正常にインタラクト: {touchingCollision.gameObject.name}");
            onInteract(component);
        }
    }

    // マップに落ちているパーツにインタラクトするメソッド
    private UniTask InteractParts(CancellationToken ct)
    {
        return Interact<MapParts>(partsTag, mapParts => partsManager.ExchangeParts(mapParts), ct);
    }

    // レバーにインタラクトするメソッド
    private UniTask InteractLever(CancellationToken ct)
    {
        return Interact<RustyLever>(leverTag, lever => lever.RotateLever(), ct);
    }

    // ストップボタンにインタラクトするメソッド
    private UniTask InteractStopButton(CancellationToken ct)
    {
        return Interact<StopButton>(stopButtonTag, stopButton => stopButton.PressButton(), ct);
    }
    // 水タンクにインタラクトするメソッド
    private UniTask InteractWaterTank(CancellationToken ct)
    {
        return Interact<WaterTank>(waterTankTag, waterTank => waterTank.ChargeWater(), ct);
    }
    // 燃え盛る炎にインタラクトするメソッド
    private UniTask InteractBurningFire(CancellationToken ct)
    {
        return Interact<BurningFireCheckZone>(burningFireTag, burningFire => burningFire.FireExtinguished(), ct);
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
            //if (showDebugLogs) Debug.Log($"トリガーエンター: {collision.gameObject.name}");
        }
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (touchingCollision == collision)
        {
            touchingCollision = null;
            //if (showDebugLogs) Debug.Log($"トリガー退出: {collision.gameObject.name}");
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
}

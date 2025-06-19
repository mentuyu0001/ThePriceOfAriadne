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
    // 決定ボタンの入力アクション
    [SerializeField] private InputActionProperty interactAction;
    // 接触しているコライダー
    private Collider2D touchingCollision = null;

    // Unityの初期化処理
    private void Start()
    {
        // マップに落ちているパーツにインタラクト可能にする
        InteractParts(this.GetCancellationTokenOnDestroy()).Forget();
    }

    // マップに落ちているパーツにインタラクトするメソッド
    async UniTask InteractParts(CancellationToken ct)
    {
        // マップ上に落ちているパーツのスクリプト
        MapParts mapParts = null;

        while (true)
        {
            // インタラクトボタンが押されるまで待機
            await interactAction.action.OnStartedAsync(ct);

            // マップに落ちているパーツに接触していない場合は，再びインタラクトボタンが押されるまで待機
            if (touchingCollision == null || !touchingCollision.gameObject.CompareTag(partsTag))
            {
                continue;
            }
            
            // マップに落ちているパーツのスクリプトを取得
            mapParts = touchingCollision.gameObject.GetComponent<MapParts>();
            if (mapParts == null)
            {
                Debug.LogError("MapPartsコンポーネントが見つかりません。");
                continue; // 次のループへ
            }

            // プレイヤーのパーツとマップに落ちているパーツを交換
            partsManager.ExchangeParts(mapParts);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        touchingCollision = collision;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        touchingCollision = null;
    }
}

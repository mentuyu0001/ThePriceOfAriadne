using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerAirChecker : MonoBehaviour
{
    /// <summary>
    /// プレイヤーが地面にいるか判定するスクリプト
    /// </summary>
    
    private bool isGround = false; // 地面にいるかどうか
    public bool IsGround => isGround; // isGroundのgetter

    private Collider2D col; // Collider2Dを追加
    [SerializeField] private LayerMask groundLayer; // 地面のレイヤーを指定
    [SerializeField] private Vector2 sizeModifier = new Vector2(1.0f, 0.1f); // レイを飛ばす際のコライダーサイズ 例：幅は90%、高さは20%
    [SerializeField] private float groundCheckBuffer = 0f; // コライダーの底辺から伸ばすレイの長さ
    [SerializeField] private PlayerRunTimeStatus runTimeStatus; // 二段ジャンプのプロパティを取得
    [SerializeField] private PlayerStatus playerStatus; // 二段ジャンプできるかどうかを取得する

    private void Start() {
        col = GetComponent<Collider2D>(); // Colliderの取得

        if (col == null)
        {
            Debug.LogError("Colliderがアタッチされていません！");
        }

        if (col.sharedMaterial == null)
        {
            Debug.LogError("Physics Materialがアタッチされていません！");
        }

        AirChecker().Forget();
    }

    // レイを飛ばし続け、地面に設置したらisGroundをtrueにする
    private async UniTaskVoid AirChecker() 
    {
        bool wasGround = false; // 過去の接地状態を保持する変数

        // このオブジェクトが破棄されるまでループを続ける
        while (!this.GetCancellationTokenOnDestroy().IsCancellationRequested)
        {

            // GetGroundHitInfo() を呼び出して地面との接触情報を取得
            RaycastHit2D hit = GetGroundHitInfo();
            isGround = hit.collider != null;

            if (!wasGround && isGround) {
                if (playerStatus.CanDoubleJump) {
                    runTimeStatus.CanDoubleJump = true;
                }
            }

            wasGround = isGround; // 変更を同期させる

            // 1フレーム待機
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    // レイを飛ばし、地面との接触情報を取得する関数
    private RaycastHit2D GetGroundHitInfo()
    {
        // colliderの外接短形を取得
        Bounds bounds = col.bounds;

        // キャストのパラメータをboundsから動的に決定
        Vector2 castOrigin = bounds.center; // キャストの開始位置はコライダーの中心
        // レイのサイズ
        float castWidth = bounds.size.x * sizeModifier.x;
        float castHeight = bounds.size.y * sizeModifier.y;
        Vector2 castSize = new Vector2(castWidth, castHeight); // 元のサイズから倍率で実際のサイズを決める
        float castDistance = bounds.extents.y + groundCheckBuffer; // キャスト距離（下方向に飛ばすレイの長さ）

        // ヒットしたかどうかで色分け
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, castSize, 0f, Vector2.down, castDistance, groundLayer);

        return hit;
    }

    /*
    // Gizmoを描画するためのメソッド
    void OnDrawGizmos()
    {
        col = GetComponent<Collider2D>(); // Colliderの取得
        // Gizmoの描画も、同じロジックでサイズを計算して反映させる
        Bounds bounds = col.bounds;
        Vector2 castOrigin = bounds.center;

        // ★★★ 変更点 ★★★
        float castWidth = bounds.size.x * sizeModifier.x;
        float castHeight = bounds.size.y * sizeModifier.y;
        Vector2 castSize = new Vector2(castWidth, castHeight);

        float castDistance = bounds.extents.y + groundCheckBuffer;

        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, castSize, 0f, Vector2.down, castDistance, groundLayer);
        Gizmos.color = hit.collider != null ? Color.red : Color.green;

        Vector3 endPosition = (Vector2)castOrigin + (Vector2.down * castDistance);
        Gizmos.DrawWireCube(endPosition, castSize);
        
        // 可視化のために、Update内と同じパラメータでBoxCastを再度実行します

        // ボックスキャストが何かに当たったかどうかに応じて、ギズモの色を決定します
        if (hit.collider != null)
        {
            // ヒットした場合：赤色で表示
            Gizmos.color = Color.red;
        }
        else
        {
            // ヒットしなかった場合（空中にいる場合）：緑色で表示
            Gizmos.color = Color.green;
        }
    }
    */
}

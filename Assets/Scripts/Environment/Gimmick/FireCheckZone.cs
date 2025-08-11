using UnityEngine;
 /// <summary>
 /// プレイヤーが炎の上を歩けるかどうかをチェックするゾーン
 /// </summary>
public class FireCheckZone : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private GameObject warningUI; // 警告表示用のUI（オプション）
    [SerializeField] private Collider2D fireFieldCollider; // 炎フィールドの物理コライダー
    
    private void Start()
    {
        // 初期状態では炎のコライダーが必要かチェック
        if (player != null && playerStatus != null && fireFieldCollider != null)
        {
            // プレイヤーが炎上を歩ける場合、最初から通れるようにする
            fireFieldCollider.enabled = !playerStatus.CanWalkOnFire;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player && playerStatus != null && fireFieldCollider != null)
        {
            // プレイヤーが炎の上を歩けない場合は物理的な当たり判定を有効化
            if (!playerStatus.CanWalkOnFire)
            {
                // 通過できない場合の処理
                Debug.Log("炎で通れない！");
                
                // 警告UIを表示（オプション）
                if (warningUI != null)
                {
                    warningUI.SetActive(true);
                }
                
                // 炎フィールドの物理コライダーを有効化
                fireFieldCollider.enabled = true;
            }
            else
            {
                Debug.Log("炎を通り抜けられる");
                // 炎フィールドの物理コライダーを無効化
                fireFieldCollider.enabled = false;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            // 警告UIを非表示に（オプション）
            if (warningUI != null)
            {
                warningUI.SetActive(false);
            }
        }
    }
}

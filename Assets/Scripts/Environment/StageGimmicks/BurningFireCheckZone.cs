using UnityEngine;
 /// <summary>
 /// 燃え盛る炎を通れない警告するゾーン（常に通り抜けられない）
 /// </summary>
public class BurningFireCheckZone : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject warningUI; // 警告表示用のUI（オプション）
    [SerializeField] private Collider2D fireFieldCollider; // 炎フィールドの物理コライダー
    [SerializeField] private Collider2D fireFieldColliderOpposite; // 反対側の炎フィールドの物理コライダー
    [SerializeField] private GameObject burnibgFire; // 炎オブジェクト（消火後に非表示にするため）
    [SerializeField] private bool isRightCheckZone; // 右側の炎ゾーンかどうか
    private void Start()
    {
        // 初期状態では炎のコライダーを常に有効化
        if (fireFieldCollider != null)
        {
            fireFieldCollider.enabled = true;
        }

        if (fireFieldColliderOpposite != null)
        {
            fireFieldColliderOpposite.enabled = true;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            // 常に通過できない
            Debug.Log("この炎は消火しないと通れない！");

            // 警告UIを表示（オプション）
            if (warningUI != null)
            {
                warningUI.SetActive(true);
            }

            // 炎フィールドの物理コライダーを常に有効化
            if (fireFieldCollider != null)
            {
                fireFieldCollider.enabled = true;
            }
            
            if (fireFieldColliderOpposite != null)
            {
                fireFieldColliderOpposite.enabled = true;
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

    // 炎が消火された時に呼び出すメソッド
    public void FireExtinguished()
    {

        // Playerオブジェクトから水発射コンポーネントを取得と実行
        ShootWaterController shootWater = player.GetComponent<ShootWaterController>();
        shootWater.ShootWater();
        Debug.Log($"プレイヤーの向き: {(shootWater.playerDirectionRight)}");
        if (shootWater.playerDirectionRight != isRightCheckZone)
        {
            // 炎オブジェクトを非表示にする
            if (burnibgFire != null)
            {
                burnibgFire.SetActive(false);
            }
            Debug.Log("炎が消火されました！");
        }
        else
        {
            Debug.Log("反対向きに水を発射してしまった！");
        }
    }
}

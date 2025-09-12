using UnityEngine;
using VContainer;
using System.Collections;
using Cysharp.Threading.Tasks;
 /// <summary>
 /// 燃え盛る炎を通れない警告するゾーン（常に通り抜けられない）
 /// </summary>
public class BurningFireCheckZone : MonoBehaviour
{
    [Inject] private GameObject player;
    [SerializeField] private GameObject warningUI; // 警告表示用のUI（オプション）
    [SerializeField] private Collider2D fireFieldCollider; // 炎フィールドの物理コライダー
    [SerializeField] private Collider2D fireFieldColliderOpposite; // 反対側の炎フィールドの物理コライダー
    [SerializeField] private GameObject burnibgFire; // 炎オブジェクト（消火後に非表示にするため）
    [SerializeField] public bool isRightCheckZone; // 右側の炎ゾーンかどうか
    private float fireWait; // 水を発射するまでの待機時間(秒)
    private float extinguishDelay; // 水発射後、炎消火までの追加待機時間

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
        
        // 炎オブジェクトを非表示にする（消火完了）
        if (burnibgFire != null)
        {
            burnibgFire.SetActive(false);
            Debug.Log("炎が消火されました！");
        }
        
        // 炎フィールドのコライダーを無効化（通行可能にする）
        if (fireFieldCollider != null)
        {
            fireFieldCollider.enabled = false;
        }
        
        if (fireFieldColliderOpposite != null)
        {
            fireFieldColliderOpposite.enabled = false;
        }
    }

    // 炎が消火された時に呼び出すメソッド（UniTask版）
    public async UniTask FireExtinguishedAsync()
    {
        // Playerオブジェクトから水発射コンポーネントを取得と実行
        ShootWaterController shootWater = player.GetComponent<ShootWaterController>();
        shootWater.ShootWater();
        
        // 水発射のアニメーション待機時間を取得
        fireWait = shootWater.waterWait;
        extinguishDelay = shootWater.waterDuration;
        
        // 水発射のアニメーション完了まで待機
        await UniTask.Delay(System.TimeSpan.FromSeconds(fireWait));
        
        // 追加の消火待機時間
        await UniTask.Delay(System.TimeSpan.FromSeconds(extinguishDelay));
        
        // 炎オブジェクトを非表示にする（消火完了）
        if (burnibgFire != null)
        {
            burnibgFire.SetActive(false);
            Debug.Log("炎が消火されました！");
        }
        
        // 炎フィールドのコライダーを無効化（通行可能にする）
        if (fireFieldCollider != null)
        {
            fireFieldCollider.enabled = false;
        }
        
        if (fireFieldColliderOpposite != null)
        {
            fireFieldColliderOpposite.enabled = false;
        }
    }
}

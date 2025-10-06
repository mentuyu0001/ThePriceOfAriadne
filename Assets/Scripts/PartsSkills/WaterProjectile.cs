using UnityEngine;

public class WaterProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2.0f; // 水の生存時間
    [SerializeField] private GameObject splashEffect; // 水滴エフェクト（オプション）
    
    // 特定の消火対象
    [SerializeField] private GameObject targetFire;
    
    void Start()
    {
        // 一定時間後に自動で破棄（何にも当たらなかった場合）
        Destroy(gameObject, lifeTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 指定したBurningFireオブジェクトのみと反応する
        if (targetFire != null && collision.gameObject == targetFire.gameObject)
        {
            // 炎を消火（非アクティブ化）
            targetFire.SetActive(false);
            Debug.Log("Fire extinguished!");
            
            // 効果音やエフェクトを再生（オプション）
            PlaySplashEffect(collision.transform.position);
            
            // 水は消滅
            Destroy(gameObject);
            return;
        }
    }
    
    // 水滴エフェクトを再生
    private void PlaySplashEffect(Vector3 position)
    {
        Debug.Log("PlaySplashEffect called at: " + position);
        if (splashEffect != null)
        {
            Debug.Log("Instantiate splashEffect!"); // 追加
            Instantiate(splashEffect, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("splashEffect is not assigned!"); // 追加
        }
    }
}

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
            
            // 効果音やエフェクトを再生（オプション）
            PlaySplashEffect(collision.transform.position);
            
            // 水は消滅
            Destroy(gameObject);
            return;
        }
        
        // 地面や壁に当たった場合も消滅する
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            // 効果音やエフェクトを再生
            PlaySplashEffect(collision.transform.position);
            
            // 水は消滅
            Destroy(gameObject);
        }
    }
    
    // 水滴エフェクトを再生
    private void PlaySplashEffect(Vector3 position)
    {
        // エフェクトが設定されている場合は再生
        if (splashEffect != null)
        {
            Instantiate(splashEffect, position, Quaternion.identity);
        }
    }
    
    // 実行時に対象を設定するメソッド（プログラムから設定する場合）
    public void SetTarget(GameObject fire)
    {
        targetFire = fire;
    }
}

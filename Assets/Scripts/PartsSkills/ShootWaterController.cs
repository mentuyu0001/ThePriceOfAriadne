using UnityEngine;
using System.Collections;
using VContainer;

/// <summary>
/// 水を前方に表示するクラス
/// </summary>
public class ShootWaterController : MonoBehaviour
{   
    [Inject] private GameObject player; // プレイヤーオブジェクト
    [Inject] private PlayerRunTimeStatus playerRunTimeStatus; // プレイヤーのステータス
    [Inject] private IWaterFactory waterFactory;

    [SerializeField] private float displayDistance = 1.5f; // 前方に表示する距離
    [SerializeField] public float waterWait = 0.5f; // 水の発射待機時間
    [SerializeField] public float waterDuration = 1.0f; // 表示時間（秒）
    
    private Rigidbody2D playerRigidbody; // プレイヤーのRigidbody2D
    
    private RigidbodyConstraints2D originalConstraints; // 元のconstraints値を保存する変数

    public bool playerDirectionRight; // プレイヤーの向き（右向き:true、左向き:false）
    /*
    private Controller playerController; // プレイヤーのコントローラー（移動制限用）
    private bool wasControlEnabled; // 元のコントローラーの入力状態を保存する変数
    */    
    private void Start()
    {
        // プレイヤーのRigidbodyが設定されていない場合は、親オブジェクトから取得を試みる
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponentInParent<Rigidbody2D>();
        }

        // 初期のconstraints値を保存
        if (playerRigidbody != null)
        {
            originalConstraints = playerRigidbody.constraints;
        }
        /*
        // プレイヤーコントローラーがセットされていない場合は探す
        if (playerController == null)
        {
            playerController = GetComponentInParent<player>();
        }
        */
    }
    
    public void ShootWater()
    {
        // CanShootWaterがtrueの場合のみ水を表示
        if (playerRunTimeStatus != null && playerRunTimeStatus.CanShootWater)
        {
            StartCoroutine(ShootWaterCoroutine());
        }
    }

    private IEnumerator ShootWaterCoroutine()
    {
        // 水を発射したら再度発射できないようにする
        playerRunTimeStatus.CanShootWater = false;
        
        // プレイヤーの移動を制限
        RestrictPlayerMovement();
        
        // waterWait秒待機
        yield return new WaitForSeconds(waterWait);
        
        // プレイヤーの向きに応じて水の発生位置のX座標のプラスマイナスを変更する
        float direction = 1f; // まず右向き(1)で初期化
        playerDirectionRight = true;
        
        // Y軸の回転を正規化して判定（-180〜180度の範囲に変換）
        float normalizedYRotation = Mathf.DeltaAngle(0f, transform.eulerAngles.y);
        if (Mathf.Abs(normalizedYRotation) > 90f)
        {
            direction = -1f; // 左向き(-1)にする
            playerDirectionRight = false;
        }

        // 水の生成座標を計算
        Vector3 waterPosition = new Vector3(
            transform.position.x + (displayDistance * direction),
            transform.position.y,
            0.0f // Z座標を0に
        );

        // 向きに応じて回転を決定（Prefabが右向きの場合）
        Quaternion rotation = Quaternion.identity;
        if (direction < 0f)
        {
            rotation = Quaternion.Euler(0, 180, 0); // 左向き
        }

        GameObject water = null;
        if (waterFactory != null)
        {
            water = waterFactory.CreateWaterEffect(waterPosition, rotation);
            Debug.Log("WaterEffect生成: " + waterPosition);
        }
        else
        {
            Debug.LogWarning("waterFactoryがInjectされていません");
        }

        // 指定時間後に水オブジェクトを破棄し、プレイヤーの移動を再開
        StartCoroutine(DestroyWaterAndAllowMovement(water, waterDuration));
        StartCoroutine(HideWaterEffectAfterDelay(water, waterDuration));
    }

    // プレイヤーの移動を制限する
    private void RestrictPlayerMovement()
    {
        if (playerRigidbody != null)
        {
            // X軸とY軸の移動を制限 (FreezePositionとFreezeRotationを設定)
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        /*
        // コントローラーの入力を無効化
        if (playerController != null)
        {
            // 現在の状態を保存
            wasControlEnabled = playerController.inputEnabled;
            // 入力を無効化
            playerController.inputEnabled = false;
        }
        */
    }

    // プレイヤーの移動制限を解除する
    private void AllowPlayerMovement()
    {
        // Rigidbodyの制約を元に戻す
        if (playerRigidbody != null)
        {
            // 元のconstraints値に戻す
            playerRigidbody.constraints = originalConstraints;
        }
        /*
        // コントローラーの入力を元に戻す
        if (playerController != null)
        {
            playerController.inputEnabled = wasControlEnabled;
        }
        */
    }
    
    // 水オブジェクトを破棄し、プレイヤーの移動を許可するコルーチン
    private IEnumerator DestroyWaterAndAllowMovement(GameObject water, float delay)
    {
        // 指定時間待機
        yield return new WaitForSeconds(delay);
        
        // 水オブジェクトを破棄
        if (water != null)
        {
            Destroy(water);
        }
        
        // プレイヤーの移動を許可
        AllowPlayerMovement();
    }

    // WaterEffectを非表示にするコルーチン
    private IEnumerator HideWaterEffectAfterDelay(GameObject water, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (water != null)
        {
            water.SetActive(false);
        }
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// 水を前方に表示するクラス
/// </summary>
public class ShootWaterController : MonoBehaviour
{   
    [SerializeField] private GameObject waterPrefab; // 水のプレハブ
    [SerializeField] private float displayDistance = 1.5f; // 前方に表示する距離
    [SerializeField] private float displayDuration = 1.0f; // 表示時間（秒）
    
    [SerializeField] private GameObject player; // プレイヤーオブジェクト
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private Rigidbody2D playerRigidbody; // プレイヤーのRigidbody2D
    
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
        if (playerStatus != null && playerStatus.CanShootWater)
        {
            // プレイヤーの向きに応じて水の発生位置のX座標のプラスマイナスを変更する
            float direction = 1f; // まず右向き(1)で初期化
            playerDirectionRight = true;
            
            // Y軸の回転が180度に近いかどうかで左向きかを判定する
            if (Mathf.Approximately(transform.eulerAngles.y, 180f))
            {
                direction = -1f; // 左向き(-1)にする
                playerDirectionRight = false;
            }

            // 水の生成座標を計算
            Vector3 waterPosition = new Vector3(
                transform.position.x + (displayDistance * direction),
                transform.position.y,
                1.0f // Z座標を1に固定
            );

            // 水のプレハブを生成（プレイヤーの前方に配置）
            GameObject water = Instantiate(waterPrefab, waterPosition, transform.rotation);
            
            // プレイヤーの移動を制限
            RestrictPlayerMovement();

            // 指定時間後に水オブジェクトを破棄し、プレイヤーの移動を再開
            StartCoroutine(DestroyWaterAndAllowMovement(water, displayDuration));

            // 水を発射したら再度発射できないようにする
            playerStatus.CanShootWater = false;
        }
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
}

using UnityEngine;
using VContainer;
using System.Collections;

/// <summary>
/// 錆びたレバーを操作するためのスクリプト
/// </summary>
public class RustyLever : MonoBehaviour
{
    [SerializeField]
    private GameObject pivot; // RotationPivotオブジェクトをアサイン
    [Inject]
    private GameObject player; // Playerオブジェクトをアサイン

    [Inject]
    private PlayerStatus playerStatus; // PlayerStatusをアサイン
    [SerializeField] private float reverWait = 0.5f; // レバーを動かすまでの待機時間(秒)   
    private bool isLeverRotated = false; // レバーが傾いているかどうか
    private Quaternion initialRotation; // 初期Rotationを保存


    void Start()
    {
        if (pivot == null)
        {
            Debug.LogError("Leverオブジェクトがアサインされていません！");
        }
        if (player == null)
        {
            Debug.LogError("Playerオブジェクトがアサインされていません！");
        }
        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatusがアサインされていません！");
        }

        // 初期Rotationを保存
        initialRotation = pivot.transform.localRotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            
        }
    }

    public void RotateLever()
    {
        // PlayerStatusがない、またはCanPushHeavyObjectがfalseの場合は処理を行わない
        if (playerStatus.CanPushHeavyObject)
        {
            if (pivot != null)
            {
                if (!isLeverRotated)
                {
                    // レバーを傾ける処理（待機時間付き）
                    StartCoroutine(RotateLeverCoroutine());
                }
                else
                {
                    // レバーを元の状態に戻す処理
                    ResetLeverRotation();
                }
            }
            else
            {
                Debug.LogError("Leverオブジェクトがアサインされていません！");
            }
        }
        else
        {
            Debug.Log("レバーを動かすには重いものを押す能力が必要です。");
            return;
        }
    }

    private IEnumerator RotateLeverCoroutine()
    {
        // reverWait秒待機
        yield return new WaitForSeconds(reverWait);

        // プレイヤーの向きに応じて回転角度を決定
        float direction = -1f; // まず右向き(1)で初期化
        // プレイヤーのY軸の回転が180度に近いかどうかで左向きかを判定する
        if (Mathf.Approximately(player.transform.eulerAngles.y, 180f))
        {
            direction = 1f; // 左向き(-1)にする
        }
        
        // プレイヤーの向きに応じて回転角度を決定
        float rotationAngle = 45f * direction; 

        // レバーを傾ける
        pivot.transform.Rotate(Vector3.forward, rotationAngle);
        isLeverRotated = true;
    }

    private void ResetLeverRotation()
    {
        // レバーを初期状態に戻す
        pivot.transform.localRotation = initialRotation;
        isLeverRotated = false;
    }
}

/// ObjectInteractionTrigger.cs
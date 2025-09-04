using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class ThrowKnifeController : MonoBehaviour
{
    /// <summary>
    /// ナイフの発射に伴う変数管理と発射向き設定、速度設定を行うスクリプト
    /// </summary>

    [Inject] private IKnifeFactory knifeFactory; // ファクトリーを注入
    [SerializeField] private float spawnOffsetX = 1.5f; // プレイヤーからどれだけX軸方向に離れてナイフを出すか
    [SerializeField] private float throwForce = 10.0f;

    [SerializeField] private int throwCoolTime = 1000; // 次のナイフが投げれるまでのクールタイム(1000ms)

    [Inject] private PlayerRunTimeStatus runTimeStatus;
    [Inject] private PlayerStatus playerStatus;

    public async UniTaskVoid ThrowKnife()
    {
        if (playerStatus.CanThrowKnife && runTimeStatus.CanThrowKnife)
        {
            runTimeStatus.CanThrowKnife = false; // ナイフを投げたら連続で投げれないようにfalseにする

            // プレイヤーの向きに応じてナイフの発生位置のX座標のプラスマイナスを変更する
            float direction = 1f; // まず右向き(1)で初期化
            // Y軸の回転が180度に近いかどうかで左向きかを判定する
            if (Mathf.Approximately(transform.eulerAngles.y, 180f))
            {
                direction = -1f; // 左向き(-1)にする
            }

            // ナイフの生成座標を計算
            Vector3 spawnPosition = new Vector3(
                transform.position.x + (spawnOffsetX * direction), // プレイヤーのX座標に、向きに応じたオフセットを加える
                transform.position.y,                              // Y座標はプレイヤーと同じ
                transform.position.z                               // Z座標もプレイヤーと同じ
            );

            if (knifeFactory != null)
            {
                // ファクトリーからナイフインスタンスを生成
                var knifeInstance = knifeFactory.CreateKnife(spawnPosition, Quaternion.identity);
                var knifeComponent = knifeInstance.GetComponent<Knife>();
                
                if (knifeComponent != null)
                {
                    Debug.Log($"ナイフを投げました: {knifeInstance.name}");
                    // ナイフの投げる力の設定
                    Vector2 forceVector;

                    // プレイヤーが左向きなら、ナイフの見た目も左向きにする
                    if (direction < 0)
                    {
                        // 元のスケールを取得
                        Vector3 originalScale = knifeInstance.transform.localScale;

                        knifeInstance.transform.localScale = new Vector3(
                        originalScale.x * -1, 
                        originalScale.y, 
                        originalScale.z
                        );

                        // 左向きの場合：Xにマイナスの力
                        forceVector = new Vector2(-throwForce, 0f);
                    } else {
                        // 左向きの場合：Xにマイナスの力
                        forceVector = new Vector2(throwForce, 0f);
                    }

                    // ナイフのRigidbodyを取得し、力を加える
                    Rigidbody2D knifeRb = knifeInstance.GetComponent<Rigidbody2D>();
                    knifeRb.AddForce(forceVector, ForceMode2D.Impulse);

                    // 1秒まってから次のナイフが投げれるようにする
                    await UniTask.Delay(throwCoolTime);
                    runTimeStatus.CanThrowKnife = true;
                }
            }
            else
            {
                Debug.LogError("KnifeFactoryが注入されていません");
            }
        }
        else
        {
            Debug.LogError("KnifePrefabが注入されていません");
        }
    }
}

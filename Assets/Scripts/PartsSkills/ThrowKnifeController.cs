using Cysharp.Threading.Tasks;
using UnityEngine;

public class ThrowKnifeController : MonoBehaviour
{
    /// <summary>
    /// ナイフの発射に伴う変数管理と発射向き設定、速度設定を行うスクリプト
    /// </summary>

    [SerializeField] private GameObject knifePrefab; // ナイフのPrefabをInspectorから設定
    [SerializeField] private float spawnOffsetX = 1.5f; // プレイヤーからどれだけX軸方向に離れてナイフを出すか
    [SerializeField] private float throwForce = 10.0f;

    [SerializeField] private int throwCoolTime = 1000; // 次のナイフが投げれるまでのクールタイム(1000ms)

    [SerializeField] private PlayerRunTimeStatus runTimeStatus;
    [SerializeField] private PlayerStatus playerStatus;

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

            // 計算した座標でナイフを生成（回転はQuaternion.identityで無回転がシンプル）
            GameObject knife = Instantiate(knifePrefab, spawnPosition, Quaternion.identity);
            // ナイフの投げる力の設定
            Vector2 forceVector;

            // プレイヤーが左向きなら、ナイフの見た目も左向きにする
            if (direction < 0)
            {
                // 元のスケールを取得
                Vector3 originalScale = knife.transform.localScale;

                knife.transform.localScale = new Vector3(
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
            Rigidbody2D knifeRb = knife.GetComponent<Rigidbody2D>();
            knifeRb.AddForce(forceVector, ForceMode2D.Impulse);

            // 1秒まってから次のナイフが投げれるようにする
            await UniTask.Delay(throwCoolTime);
            runTimeStatus.CanThrowKnife = true;
        }
    }
}

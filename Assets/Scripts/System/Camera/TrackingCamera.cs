// CameraFollower.cs
using UnityEngine;
using Cysharp.Threading.Tasks; // UniTaskを使用するために必要
using System.Threading;

public class TrackingCamera : MonoBehaviour
{
    // Unityエディタのインスペクタから追従対象のPlayerオブジェクトを設定する
    [SerializeField] private Transform player; // プレイヤーオブジェクト

    // カメラのTransformコンポーネントをキャッシュするための変数
    private Transform cameraTransform; // カメラのTransform

    async UniTaskVoid Start()
    {
        // 自身のTransformコンポーネントを取得
        cameraTransform = this.transform;

        // オブジェクトが破棄されるまでプレイヤーを追従する
        await FollowPlayerLoop(this.GetCancellationTokenOnDestroy());
    }

    private async UniTask FollowPlayerLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // LateUpdate の代わりに PostLateUpdate を使用
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

            // Playerが設定されていない場合は何もしない
            if (player == null)
            {
                continue;
            }

            // カメラの位置を更新
            cameraTransform.position = new Vector3(
                player.position.x,
                player.position.y,
                cameraTransform.position.z
            );
        }
    }
}

// CameraFollower.cs
using UnityEngine;
using Cysharp.Threading.Tasks; // UniTaskを使用するために必要
using System.Threading;
using VContainer; 

public class TrackingCamera : MonoBehaviour
{
    // Unityエディタのインスペクタから追従対象のPlayerオブジェクトを設定する
    [Inject] private GameObject player; // プレイヤーオブジェクト

    // カメラのTransformコンポーネントをキャッシュするための変数
    private Transform cameraTransform; // カメラのTransform
    private Transform playerTransform; // プレイヤーのTransform

    async UniTaskVoid Start()
    {
        // 自身のTransformコンポーネントを取得
        cameraTransform = this.transform;

        // プレイヤーのTransformを取得してキャッシュ
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("TrackingCamera: Playerが注入されていません");
            return;
        }

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
            if (player == null || playerTransform == null)
            {
                continue;
            }

            // カメラの位置を更新（player.transform.positionを使用）
            cameraTransform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                cameraTransform.position.z
            );
        }
    }
}

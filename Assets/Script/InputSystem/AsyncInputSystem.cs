using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

/// <summary>
/// InputSystemのイベントを非同期で待つタスクを提供するユーティリティクラス
/// </summary>

public static class AsyncInputSystem
{
    // InputActionのstartedイベントを非同期で待機するタスクを返すメソッド
    public static UniTask OnStartedAsync(this InputAction inputAction, CancellationToken ct)
    {
        // タスクを作成
        var taskSource = new UniTaskCompletionSource();

        // strartedイベントが起きた時に呼び出される関数を定義
        void OnStarted(InputAction.CallbackContext context)
        {
            // タスク完了後に呼ばれないように登録を解除
            inputAction.started -= OnStarted;
            // タスク完了のタイミングをここに設定
            taskSource.TrySetResult();
        }

        // startedイベントが起きた時にOnStartedが呼び出されるように登録
        inputAction.started += OnStarted;

        // ctがキャンセルされたときの処理
        ct.Register(() =>
        {
            // 登録を解除
            inputAction.started -= OnStarted;
            // タスクキャンセルのタイミングをここに設定
            taskSource.TrySetCanceled();
        });

        // タスクを返す
        return taskSource.Task;
    }
}

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

/// <summary>
///  ステージスタート時に呼び出すスクリプト
/// </summary>
public class GameStartManager : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;
    [SerializeField] private Controller controller;
    [SerializeField] private PlayerAnimationManager playerAnimationManager;
    [SerializeField] private GameObject startObg;

    // プレイヤーの速度(単位はs)
    private float dashTime = 2.0f;
    private float stopTime = 1.0f;

    // アニメーション時間
    private float animationTime;

    async UniTaskVoid Start()
    {

        startObg.SetActive(false);

        animationTime = dashTime + stopTime;

        // 黒画像をフェードインさせる
        fadeController.FadeOut(animationTime).Forget();

        // 入力を止める
        controller.isInputEnabled = false;

        // プレイヤーを動かす
        playerAnimationManager.AniWalkTrue();

        controller.StartAndGoalSetFrictionZero();

        float startTime = Time.time;
        while (Time.time - startTime < dashTime)
        {
            controller.StartAndGoalVelocity();
            await UniTask.Yield(PlayerLoopTiming.Update); // 毎フレーム待機
        }
        controller.StartAndGoalSetFrictionAdd();
        playerAnimationManager.AniWalkFalse();


        await UniTask.Delay((int)(stopTime * 1000));

        // プレイヤーを止める

        // スタートに障害物を置く
        startObg.SetActive(true);

        // 入力を再開する
        controller.isInputEnabled = true;
    }
}

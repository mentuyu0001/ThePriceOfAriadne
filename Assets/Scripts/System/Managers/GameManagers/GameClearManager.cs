using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;
    [SerializeField] private Controller controller;
    [SerializeField] private PlayerAnimationManager playerAnimationManager;
    [SerializeField] private GameObject GoalObg;

    // プレイヤーの速度(単位はs)
    private float dashTime = 2.0f;
    private float stopTime = 1.0f;

    // アニメーション時間
    private float animationTime;

    // 一度だけ実行するためのフラグ（目印）
    private bool hasTriggered = false;

    private async void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("test");

        if (other.gameObject.tag == "Player" && !hasTriggered)
        {
            hasTriggered = true;

            GoalObg.SetActive(false);

            animationTime = dashTime + stopTime;

            // 黒画像をフェードアウトさせる
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
            GoalObg.SetActive(true);

            // 入力を再開する
            controller.isInputEnabled = true;
        }
    }
}

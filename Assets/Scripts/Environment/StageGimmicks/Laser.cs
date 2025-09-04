using UnityEngine;
using VContainer;
/// <summary>
/// レーザーのスクリプト
/// </summary>      

public class Laser : StoppableGimick
{
    [SerializeField] private Transform laserStand; // レーザーの台座
    [SerializeField] private GameObject laserBeam; // レーザー本体
    [Inject] private GameOverManager gameOverManager; // GameOverManagerの参照
    [Inject] private GameObject player; // プレイヤーオブジェクト
    [SerializeField] private LaserTarget target; // GameObject → LaserTargetに変更

    private bool isActive = true;

    void Start()
    {
        // 必要に応じて初期化処理を記述
    }

    void Update()
    {
        // レーザーがアクティブである場合のみ動作
        if (!isActive)
        {
            laserBeam.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーオブジェクトに当たった場合
        if (collision.gameObject == player)
        {
            gameOverManager.GameOver(); // GameOverManagerのインスタンスを使用して呼び出す
        }
    }
    public override bool IsRunning => isActive; // ギミックが動作中かどうかを示すプロパティ

    public override void StartGimick()
    {
        // StopableGimmickのStartGimickメソッドをオーバーライド
        isActive = true;
        laserBeam.SetActive(true); // レーザー本体を表示する

        // ターゲットの動作を再開
        target.RestartTarget();
    }

    public override void StopGimick()
    {
        // StopableGimmickのStopGimickメソッドをオーバーライド
        isActive = false;
        laserBeam.SetActive(false); // レーザー本体を非表示にする
        target.StopTarget(); // ターゲットの動作を停止
    }
}

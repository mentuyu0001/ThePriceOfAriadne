using UnityEngine;

public class Laser : StoppableGimick
{
    [SerializeField] private Transform laserStand; // レーザーの台座
    [SerializeField] private GameObject laserBeam; // レーザー本体
    [SerializeField] private GameOverManager gameOverManager; // GameOverManagerの参照
    [SerializeField] private GameObject player; // プレイヤーオブジェクト

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

    public override void StopGimick()
    {
        // StopableGimmickのStopGimickメソッドをオーバーライド
        isActive = false;
        laserBeam.SetActive(false); // レーザー本体を非表示にする
    }
}

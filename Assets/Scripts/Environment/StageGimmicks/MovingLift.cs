using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

/// <summary>
/// 上下に動くリフトのスクリプト
/// </summary>

public class MovingLift : StoppableGimick
{
    // リフトのスタート時の座標
    private Vector2 posStart;
    // RigidBody2Dコンポーネント
    private Rigidbody2D rigidBody2D;
    // リフトを上下に動かすアニメーション
    private Tween liftAnimation;
    // アニメーションの1周期にかかる時間
    [SerializeField] private float duration;
    // 上下の振幅
    [SerializeField] private float deltaY;
    // ゲーム開始時に動かすかどうかを判定する変数
    // trueならゲーム開始時に動かす、falseならゲーム開始時は止めておく
    [SerializeField] private bool isMovingAtStart;
    // 初期位相（単位は度数）
    [SerializeField] private float initialPhaseDeg;

    void Start()
    {
        // Rigidbody2Dコンポーネントを取得
        rigidBody2D = GetComponent<Rigidbody2D>();
        // Rigidbody2Dコンポーネントがアタッチされているかチェック
        if (rigidBody2D == null)
        {
            Debug.LogError("MovingLift: RigidBody2D cannot found.");
        }
        // アタッチされている場合
        else
        {
            // スタート時の座標を取得
            posStart = rigidBody2D.transform.position;
        }

        // DOTweenのアニメーションが自動再生されないように設定
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;

        // アニメーションを設定------------------------------------------------
        liftAnimation = DOVirtual.Float(0, 2 * Mathf.PI, duration, value =>
        {
            // リフトを上下に単振動させるアニメーション
            // Rigidbody2Dを使って動かすことで、当たり判定を維持しながら動かせる
            Vector2 posCurrent = posStart;
            posCurrent.y = posCurrent.y + deltaY * Mathf.Sin(value + initialPhaseDeg / 180 * Mathf.PI);
            rigidBody2D.MovePosition(posCurrent);
        })
        .SetLoops(-1, LoopType.Restart) // 無限ループを設定
        .SetEase(Ease.Linear);  // イージングはリニア（線形）にする
        // -------------------------------------------------------------------

        // ゲーム開始時に動かすなら
        if (isMovingAtStart)
        {
            // ギミックを動作開始
            StartGimick();
        }
    }

    public override bool IsRunning => liftAnimation.IsActive();

    public override void StartGimick()
    {
        // 初期位相に合わせてリフトのスタート位置を動かす
        Vector2 movedPos = posStart;
        movedPos.y += Mathf.Sin(initialPhaseDeg / 180 * Mathf.PI);
        rigidBody2D.DOMove(movedPos, 1.5f);
        // アニメーションを開始
        liftAnimation.Restart();
    }

    public override void StopGimick()
    {
        // アニメーションを停止
        liftAnimation.Kill();
        // リフトをスタート時の位置に戻す
        rigidBody2D.DOMove(posStart, 2.0f);
    }
}

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
    // リフトのアニメーション
    private Tween liftAnimation;
    // アニメーションの1周期にかかる時間
    [SerializeField] private float duration;
    // 上下の振幅
    [SerializeField] private float deltaY;
    // ゲーム開始時に動かすかどうかを判定する変数
    // trueならゲーム開始時に動かす、falseならゲーム開始時は止めておく
    [SerializeField] private bool isMovingAtStart;
    [Header("単振動の初期位相（度）")]
    // 初期位相（単位は度数）
    [SerializeField] private float initialPhaseDeg;
    [SerializeField] private Material stopMaterial;
    [SerializeField] private Material runMaterial;
    [SerializeField] private Sprite stopImage;
    [SerializeField] private Sprite runImage;
    private SpriteRenderer _spriteRenderer;

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
            // 初期位相に合わせてリフトのスタート位置を動かす
            posStart.y += deltaY * Mathf.Sin(initialPhaseDeg / 180 * Mathf.PI);
            rigidBody2D.transform.position = posStart;
        }

        // DOTweenのアニメーションが自動再生されないように設定
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;

        // アニメーションを設定------------------------------------------------
        liftAnimation = DOVirtual.Float(0, 2 * Mathf.PI, duration, value =>
        {
            // リフトを上下に単振動させるアニメーション
            // Rigidbody2Dを使って動かすことで、当たり判定を維持しながら動かせる
            Vector2 posCurrent = posStart - new Vector2(0, deltaY * Mathf.Sin(initialPhaseDeg / 180 * Mathf.PI));
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

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("MovingLift: SpriteRendererがアタッチされていません。");
        }
    }

    public override bool IsRunning => liftAnimation.IsPlaying();

    public override void StartGimick()
    {
        // アニメーションを開始
        liftAnimation.Restart();
        // 動作開始時にrunMaterialとrunImageへ
        if (_spriteRenderer != null)
        {
            if (runMaterial != null) _spriteRenderer.material = runMaterial;
            if (runImage != null) _spriteRenderer.sprite = runImage;
        }
    }

    public override void StopGimick()
    {
        // アニメーションを停止
        liftAnimation.Pause();
        // リフトをスタート時の位置に戻す
        rigidBody2D.DOMove(posStart, duration/4).Play();
        // 停止時にstopMaterialとstopImageへ
        if (_spriteRenderer != null)
        {
            if (stopMaterial != null) _spriteRenderer.material = stopMaterial;
            if (stopImage != null) _spriteRenderer.sprite = stopImage;
        }
    }
}

using UnityEngine;
using VContainer;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 燃え盛る炎を通れない警告するゾーン（常に通り抜けられない）
/// </summary>
public class BurningFireCheckZone : MonoBehaviour
{
    [Inject] private GameObject player;
    [Inject] private GameTextDisplay textDisplay;
    [Inject] private PlayerPartsRatio partsRatio;
    [Inject] private ObjectTextData objectTextData;
    
    [SerializeField] private int burningFireID = 1; // BurningFireのID
    [SerializeField] private Collider2D fireFieldCollider; // 炎フィールドの物理コライダー
    [SerializeField] private Collider2D fireFieldColliderOpposite; // 反対側の炎フィールドの物理コライダー
    [SerializeField] private GameObject burnibgFire; // 炎オブジェクト（消火後に非表示にするため）
    [SerializeField] public bool isRightCheckZone; // 右側の炎ゾーンかどうか
    [SerializeField] private ParticleSystem fireParticleSystem; // 炎のParticleSystem
    [SerializeField] private float fadeOutDuration = 2f; // 炎が消えるまでの時間(秒)
    
    [Header("テキスト表示設定")]
    [SerializeField] private float delayBetweenTexts = 2f; // 複数テキスト間の待機時間
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = true;
    
    private float fireWait; // 水を発射するまでの待機時間(秒)
    private float extinguishDelay; // 水発射後、炎消火までの追加待機時間
    private bool isPlayerInZone = false;
    private bool hasShownText = false; // テキストを表示済みかどうか

    private Controller controller;

    private CancellationToken dct; // DestroyCancellationToken

    private void Start()
    {
        // 初期状態では炎のコライダーを常に有効化
        if (fireFieldCollider != null)
        {
            fireFieldCollider.enabled = true;
        }

        if (fireFieldColliderOpposite != null)
        {
            fireFieldColliderOpposite.enabled = true;
        }

        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
        controller = player.GetComponent<Controller>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            // 既にゾーン内にいる場合は何もしない
            if (isPlayerInZone)
            {
                return;
            }
            
            isPlayerInZone = true;
            
            // 常に通過できない
            if (showDebugLogs) Debug.Log("この炎は消火しないと通れない！");

            // テキスト表示（まだ表示していない場合のみ）
            if (!hasShownText && controller.isInputEnabled)
            {
                ShowBurningFireWarningText();
                hasShownText = true;
            }

            // 炎フィールドの物理コライダーを常に有効化
            if (fireFieldCollider != null)
            {
                fireFieldCollider.enabled = true;
            }
            
            if (fireFieldColliderOpposite != null)
            {
                fireFieldColliderOpposite.enabled = true;
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ゾーン内にいる間は何もしない（複数回の呼び出しを防ぐ）
        if (player != null && collision.gameObject == player)
        {
            isPlayerInZone = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            if (showDebugLogs) Debug.Log("炎ゾーンから退出");
            
            isPlayerInZone = false;
            hasShownText = false; // フラグをリセット
            
            // テキストを閉じる
            if (textDisplay != null && textDisplay.IsDisplaying)
            {
                textDisplay.HideText(dct).Forget();
            }
        }
    }

    private void ShowBurningFireWarningText()
    {
        // 拡張メソッドを使用してテキストを表示
        textDisplay.ShowTextByPartsRatio(
            partsRatio,
            objectTextData,
            burningFireID,
            dct,
            delayBetweenTexts,
            showDebugLogs
        ).Forget();
    }

    // 炎が消火された時に呼び出すメソッド
    public void FireExtinguished()
    {
        // Playerオブジェクトから水発射コンポーネントを取得と実行
        ShootWaterController shootWater = player.GetComponent<ShootWaterController>();
        shootWater.ShootWater();
        
        // 炎オブジェクトを非表示にする（消火完了）
        if (burnibgFire != null)
        {
            burnibgFire.SetActive(false);
            Debug.Log("炎が消火されました！");
        }
        
        // 炎フィールドのコライダーを無効化（通行可能にする）
        if (fireFieldCollider != null)
        {
            fireFieldCollider.enabled = false;
        }
        
        if (fireFieldColliderOpposite != null)
        {
            fireFieldColliderOpposite.enabled = false;
        }
    }

    // 炎が消火された時に呼び出すメソッド（UniTask版）
    public async UniTask FireExtinguishedAsync(CancellationToken token)
    {
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
        CancellationToken linkedToken = linkedCts.Token;

        // Playerオブジェクトから水発射コンポーネントを取得と実行
        ShootWaterController shootWater = player.GetComponent<ShootWaterController>();
        shootWater.ShootWater();
        
        // 水発射のアニメーション待機時間を取得
        fireWait = shootWater.waterWait;
        extinguishDelay = shootWater.waterDuration;
        
        // 水発射のアニメーション完了まで待機
        await UniTask.Delay(System.TimeSpan.FromSeconds(fireWait), cancellationToken: linkedToken);
        
        // ParticleSystemを徐々に消す
        if (fireParticleSystem != null)
        {
            await FadeOutParticleSystemAsync(fireParticleSystem, fadeOutDuration, linkedToken);
        }
        
        // 追加の消火待機時間
        //await UniTask.Delay(System.TimeSpan.FromSeconds(extinguishDelay), cancellationToken: linkedToken);
        
        // 炎オブジェクトを非表示にする（消火完了）
        if (burnibgFire != null)
        {
            burnibgFire.SetActive(false);
            Debug.Log("炎が消火されました！");
        }
        
        // 炎フィールドのコライダーを無効化（通行可能にする）
        if (fireFieldCollider != null)
        {
            fireFieldCollider.enabled = false;
        }
        
        if (fireFieldColliderOpposite != null)
        {
            fireFieldColliderOpposite.enabled = false;
        }
    }

  
    /// <summary>
    /// ParticleSystemを徐々にフェードアウトさせる(上から下へ消える)
    /// </summary>
    private async UniTask FadeOutParticleSystemAsync(ParticleSystem ps, float duration, CancellationToken token)
    {
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
        CancellationToken linkedToken = linkedCts.Token;

        float elapsed = 0f;
        var main = ps.main;
        var emission = ps.emission;
        
        // 元の値を保存
        float originalRateOverTime = emission.rateOverTime.constant;
        float originalStartSize = main.startSize.constant;
        float originalStartLifetime = main.startLifetime.constant;
        float originalStartSpeed = main.startSpeed.constant;
        
        // パーティクル配列を確保
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
        
        while (elapsed < duration && !linkedToken.IsCancellationRequested)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // emission rateを徐々に減らす(新しいパーティクルの生成を減らす)
            emission.rateOverTime = originalRateOverTime * (1f - t);
            
            // 速度を落として上に上がらないようにする
            main.startSpeed = originalStartSpeed * (1f - t);
            
            // ライフタイムを大幅に短くして既存のパーティクルを早く消す
            main.startLifetime = originalStartLifetime * (1f - t);
            
            // サイズも徐々に小さく
            main.startSize = originalStartSize * (1f - t * 0.5f);
            
            // 既存のパーティクルを取得して個別に制御
            int particleCount = ps.GetParticles(particles);
            float currentLifetimeThreshold = originalStartLifetime * (1f - t);
            
            for (int i = 0; i < particleCount; i++)
            {
                // パーティクルの残り寿命を計算
                float particleAge = particles[i].startLifetime - particles[i].remainingLifetime;
                
                // 閾値を超えたパーティクル（上部の古いパーティクル）の寿命を強制的に減らす
                if (particleAge > currentLifetimeThreshold)
                {
                    particles[i].remainingLifetime = Mathf.Max(0, particles[i].remainingLifetime - Time.deltaTime * 3f);
                }
            }
            
            // 変更を適用
            ps.SetParticles(particles, particleCount);
            
            await UniTask.Yield(cancellationToken: linkedToken);
        }
        
        // 完全に停止して全てのパーティクルをクリア
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}

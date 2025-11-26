using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class FadeController : MonoBehaviour
{
    // 透明度を操作する真っ黒な画像
    private Image fadeImage;

    void Awake()
    {
        // fadeImageのコンポーネントを取得
        fadeImage = GetComponent<Image>();
        
        if (fadeImage == null)
        {
            Debug.LogError("FadeController: fadeImage cannot found.");
            return;
        }
        
        Debug.Log("FadeController: fadeImage found.");
        // 初期状態: 黒で完全に透明に設定
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    // フェードインを実行する関数
    public async UniTask FadeIn(float duration)
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            Debug.Log("FadeController: Start Fade-in");
            // fadeImageの透明度をduration秒かけて0にする
            await fadeImage.DOFade(0.0f, duration)
                .Play().SetUpdate(true).SetLink(gameObject);
            Debug.Log("FadeController: Finish Fade-in");
        }
    }

    // フェードアウトを実行する関数
    public async UniTask FadeOut(float duration)
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            Debug.Log("FadeController: Start Fade-out");
            // fadeImageの透明度をduration秒かけて1にする
            await fadeImage.DOFade(1.0f, duration)
                .Play().SetUpdate(true).SetLink(gameObject);
            Debug.Log("FadeController: Finish Fade-out");
        }
    }

}

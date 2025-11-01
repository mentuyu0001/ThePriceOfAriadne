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
        // コンポーネントがアタッチされているかチェック
        if (fadeImage == null)
        {
            Debug.LogError("FadeController: fadeImage cannot found.");
        }
        // アタッチされている場合
        else
        {
            Debug.Log("FadeController: fadeImage found.");
            // fadeImageの色を黒にする
            fadeImage.color = Color.black;
        }
    }

    // フェードインを実行する関数
    public async UniTask FadeIn(float duration)
    {
        if (fadeImage != null)
        {
            Debug.Log("FadeController: Start Fade-in");
            // fadeImageの透明度をduration秒かけて0にする
            await fadeImage.DOFade(0.0f, duration)
                .Play();
            Debug.Log("FadeController: Finish Fade-in");
        }
    }

    // フェードアウトを実行する関数
    public async UniTask FadeOut(float duration)
    {
        if (fadeImage != null)
        {
            Debug.Log("FadeController: Start Fade-out");
            // fadeImageの透明度をduration秒かけて1にする
            await fadeImage.DOFade(1.0f, duration)
                .Play();
            Debug.Log("FadeController: Finish Fade-out");
        }
    }

}

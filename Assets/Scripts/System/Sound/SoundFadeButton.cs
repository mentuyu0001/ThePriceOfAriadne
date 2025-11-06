using UnityEngine;
using Cysharp.Threading.Tasks;


public class SoundFadeButton : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private float duration = 2.5f;

    public void OnButtonClicked()
    {
        soundManager.StopBGMFadeOut(duration).Forget();
    }
}

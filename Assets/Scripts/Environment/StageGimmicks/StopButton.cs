using UnityEngine;

public class StopButton : MonoBehaviour
{
    [SerializeField] private StoppableGimick[] stoppableGimicks;
    [SerializeField] private Material stopMaterial;
    [SerializeField] private Material runMaterial;
    [SerializeField] private Sprite stopImage;
    [SerializeField] private Sprite runImage;  

    private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool isStop;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRendererがアタッチされていません。");
        }
    }
    // ボタンが押されたときに呼び出す関数
    public void PressButton()
    {
        if (_spriteRenderer == null) return;

        if (isStop)
        {
            Debug.Log("Switching to runMaterial");
            _spriteRenderer.material = runMaterial;
            _spriteRenderer.sprite = runImage;
        }
        else
        {
            Debug.Log("Switching to stopMaterial");
            _spriteRenderer.material = stopMaterial;
            _spriteRenderer.sprite = stopImage;
        }
        isStop = !isStop;

        // 各ギミックの状態を切り替える
        if (stoppableGimicks == null || stoppableGimicks.Length == 0) return;

        foreach (var gimick in stoppableGimicks)
        {
            if (gimick == null) continue;

            if (gimick.IsRunning)
            {
                Debug.Log("Stopping the gimick");
                gimick.StopGimick();
            }
            else
            {
                Debug.Log("Gimick is not running.");
                gimick.StartGimick();
            }
        }
    }
}

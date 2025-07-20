using UnityEngine;

public class StopButton : MonoBehaviour
{
    // 停止対象のギミックを設定（インスペクターから設定できるようにする）
    [SerializeField] private StoppableGimick stoppableGimick;

    // ボタンが押されたときに呼び出す関数
    public void PressButton()
    {
        if (stoppableGimick != null)
        {
            stoppableGimick.StopGimick();
        }
    }
}

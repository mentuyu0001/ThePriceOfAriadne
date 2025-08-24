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
            if (stoppableGimick.IsRunning)
            {
                Debug.Log("Stopping the gimick");
                stoppableGimick.StopGimick();
            }
            else
            {
                Debug.Log("Gimick is not running.");
                stoppableGimick.StartGimick();
            }
        }
    }
}

using UnityEngine;

public abstract class StoppableGimick : MonoBehaviour
{
    // Abstract method to be implemented by child classes

    public abstract bool IsRunning { get; } // ギミックが動作中かどうかを示すプロパティ
    public abstract void StartGimick();
    public abstract void StopGimick();
}

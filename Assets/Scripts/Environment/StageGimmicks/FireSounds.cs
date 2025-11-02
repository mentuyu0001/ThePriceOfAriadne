using UnityEngine;

public class FireSounds : MonoBehaviour
{
    private bool isFireSoundPlaying = false;
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (isFireSoundPlaying) HandleFireSound();
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (isFireSoundPlaying)
            {
                SoundManager.Instance.StopLoopSE2();
                isFireSoundPlaying = false;
            }
        }
    }
    
    private void HandleFireSound()
    {
        // SoundManagerが存在しない場合は何もしない
        if (SoundManager.Instance == null) return;

        if (!isFireSoundPlaying)
        {
            SoundManager.Instance.PlayLoopSE2(12);
            isFireSoundPlaying = true; // 再生中にフラグを立てる
        }
    }
}

using UnityEngine;

public class Clickable : MonoBehaviour
{
    /// <summary>
    /// 画面遷移次にクリックできなくするスクリプト
    /// </summary>

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void DisClickable()
    {
        this.gameObject.SetActive(true);
    }
}

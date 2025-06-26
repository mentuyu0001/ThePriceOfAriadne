using Unity.Transforms;
using UnityEngine;

/// <summary>
/// プレス機のプレート部分がプレイヤーと衝突したか判定するスクリプト
/// </summary>
public class PressMachinePlate : MonoBehaviour
{
    //Playerオブジェクトの参照
    [SerializeField] private GameObject player;
    //Playerオブジェクトのオブジェクト名
    private string playerName;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("PressMachinePlate: Player cannot found.");
        }
        else
        {
            playerName = player.name;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == playerName)
        {
            // Playerと衝突したら、親オブジェクトのPressMachineBaseスクリプトから関数を呼び出す
            transform.root.gameObject.GetComponent<PressMachineBase>().OnPlateCollisionEnter();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == playerName)
        {
            // Playerが離れたら、親オブジェクトのPressMachineBaseスクリプトから関数を呼び出す
            transform.root.gameObject.GetComponent<PressMachineBase>().OnPlateCollisionExit();
        }
    }
}

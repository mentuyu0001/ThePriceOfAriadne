using UnityEngine;

public class Square : MonoBehaviour
{
    public float speed = 5f; // 移動速度を設定

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // WASDキーの入力を取得
        float moveX = Input.GetAxis("Horizontal"); // A/Dキーで左右移動
        float moveY = Input.GetAxis("Vertical");   // W/Sキーで上下移動

        // 移動処理
        Vector3 movement = new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
}

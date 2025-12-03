using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemInitialize : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    void Start()
    {
        Time.timeScale = 1f; // 時間の流れを元に戻す
        _playerInput.SwitchCurrentActionMap("Player");
    }
}
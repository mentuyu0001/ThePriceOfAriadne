using UnityEngine;

public class ResetPlayerParts : MonoBehaviour
{
    [SerializeField] private PlayerCustomizer customizer;

    private void Start()
    {
        customizer.resetCharacter();
    }
}

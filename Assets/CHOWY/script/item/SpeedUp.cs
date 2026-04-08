using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedUp : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    public float speedUp = 10f;
    private void Update()
    {
        if(Keyboard.current.digit3Key.wasPressedThisFrame)
        {

        }
    }
}

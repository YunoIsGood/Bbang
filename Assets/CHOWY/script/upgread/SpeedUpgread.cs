using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedUpgread : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    public float speedUpgread=2f;
    public float speedUpMax = 15f;
    


    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>(); 
    }
    private void Update()
    {
        if(Keyboard.current.uKey.wasPressedThisFrame)
        {
            float newSpeed = playerMovement.moveSpeed + speedUpgread;
            playerMovement.moveSpeed = Mathf.Min(newSpeed, speedUpMax);

            Debug.Log(playerMovement.moveSpeed);
        }
    }
}

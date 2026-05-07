using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour 
{
    public float moveSpeed = 5f;
    private Vector2 moveDir;
    
    [Header("Movement Limits")]
    public float minX, maxX, minY, maxY;

    void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        
        Vector3 nextPos = transform.position + (Vector3)(moveDir * moveSpeed * Time.fixedDeltaTime);

        
        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);

        
        transform.position = nextPos;
    }
}
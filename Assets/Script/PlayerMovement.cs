using UnityEngine;
using UnityEngine.InputSystem;


//플레이어 이동
public class PlayerMovement : MonoBehaviour 
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDir;

  

    void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
        
    }


    void FixedUpdate()
    {
        transform.position += (Vector3)(moveDir * moveSpeed * Time.fixedDeltaTime);
    }

  
  
}
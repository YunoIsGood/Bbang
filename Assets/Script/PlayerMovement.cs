using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


//플레이어 이동
public class PlayerMovement : MonoBehaviour 
{

    bool _kb = true;

    public float moveSpeed = 5f;


    private Rigidbody2D rb;

    private Vector2 moveDir;
    bool _KuckBacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }





    void FixedUpdate()

    {
        if (_kb == true)
        {
            transform.position += (Vector3)moveDir * moveSpeed * Time.fixedDeltaTime;
        }
    }
    
    // 이건 나중에 OnCollisionEnter2D로 바꿔야 함 안 그러면 Enemy 계열을 전부 isTrigger로 해야함 ㅇㅇ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 knockbackDir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            _kb = false;
            Applyknockback(knockbackDir);
            StartCoroutine(KBing());
        }
    }

    // float를 변경해서 넉백 길이 다르게 가능 단 음수로 하면 이동 방향과 같은 방향으로 튕겨나니 유의바람
    void Applyknockback(Vector2 _Dir)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(_Dir * 10f, ForceMode2D.Impulse);

    }
    
    IEnumerator KBing()
    {
        _kb = false;
        yield return new WaitForSeconds(0.3f);
        _kb = true;
        if(_kb == true)
        {
            rb.linearVelocity = Vector2.zero;
        }

    }



}




    
    



    
    
   



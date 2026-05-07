using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


//플레이어 이동
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    bool _canDash = true;

    public bool _kb = true; // 넉백 상태임 원래 flase로 해야 하는데 실수로 true를 초기값으로 만듦 ㅎㅎ 죄송 대신 작동은 잘 됨

    bool _InvincibilityStatus = false; // 무적 상태

    public float moveSpeed = 5f;

    private float _invicible = 1f; // 이놈도 무적 딜레이 변수임 

    private Rigidbody2D rb;

    private Vector2 moveDir;

    private float maxRange = 40f;

    private float minRange = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        instance = this;
    }

    private void Update()
    {
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame && _canDash && _kb && moveSpeed >= 0)
        {
            Vector2 DashDir = moveDir;
            Dash(DashDir);

        }

    }


    private void FixedUpdate()

    {
        if (_kb == true)
        {
            transform.position += (Vector3)moveDir * moveSpeed * Time.fixedDeltaTime;
            Mathf.Clamp(moveDir.x, 2f, -2f);
        }
    }

    private void LateUpdate()
    {
        Vector3 MoveBlock = transform.position;

      // Mathf,Clamp = 간단히 말해 위치 범위 제한하는 용도임 if문 스파게티가 안되어서 보기 편하고 위치값 변경도 수월함
       MoveBlock.x = Mathf.Clamp(transform.position.x, -19, 19);
       MoveBlock.y =  Mathf.Clamp(transform.position.y, -9, 78);

        transform.position = MoveBlock;        
    }
    
    void Dash(Vector2 _Dashdir)
    {
        rb.linearVelocity = Vector2.zero;
        _canDash = false;
        rb.AddForce(_Dashdir * 4f, ForceMode2D.Impulse);
        StartCoroutine(Dash());

    }




    void OnMove(InputValue value)

    {

        moveDir = value.Get<Vector2>();

    }

    // 이건 나중에 OnCollisionEnter2D로 바꿔야 함 안 그러면 Enemy 계열을 전부 isTrigger로 해야함 ㅇㅇ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !_InvincibilityStatus) // 적과 충돌했을 때 무적 상태가 아니라면 넉백과 무적 시작
        {
            Vector2 knockbackDir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            _kb = false;
            // 넉백 적용
            Applyknockback(knockbackDir);

            // 넉백 코루틴 시1작
            StartCoroutine(KBing());

            // 무적 코루틴 시12작
            StartCoroutine(Invincibility());

            //미안하다 한개더 만든다 데미지 시 감속 코루틴 시123작
            StartCoroutine(DamageSlow());
        }
    }

    // float를 변경해서 넉백 길이 다르게 가능 단 음수로 하면 이동 방향과 같은 방향으로 튕겨나니 유의바람

    void Applyknockback(Vector2 _Dir)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(_Dir * 5f, ForceMode2D.Impulse);


    }

    // 대시관련 
    IEnumerator Dash()
    {
        _canDash = false;
        yield return new WaitForSeconds(0.3f);
        _canDash = true;
        if (_canDash == true)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }



    // 넉백 딜레이임 ㅇㅇ 0.3초간 아무것도 못함 앙앙 업데이트로 이속도 줄여야지 ㅋ
    IEnumerator KBing()
    {
        _kb = false;
        moveSpeed *= 0.5f;
        yield return new WaitForSeconds(0.3f);

        _kb = true;
        if (_kb == true)
        {
            rb.linearVelocity = Vector2.zero;
        }

    }

    // 무적시간 딜레이임 ㅇㅇ 무적이 시작엔 거짓이고 운석이 떨어져도 저 시간만큼은 무적임 ㅇㅇ 
    IEnumerator Invincibility()
    {
        _InvincibilityStatus = true;
        yield return new WaitForSeconds(_invicible);
        _InvincibilityStatus = false;
    }

    IEnumerator DamageSlow()
    {
        moveSpeed *= 0.5f;
        yield return new WaitForSeconds(1f);
        moveSpeed *= 4f;
    }
}




    
    



    
    
   



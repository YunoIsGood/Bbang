using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerManager : MonoBehaviour
{   
    public Animator animator;
    bool isAttack;
    bool hasDeadAnimPlayed; 
    SpriteRenderer spriteRenderer;
    
    [Header("플레이어 이동 관련")]
    public float moveSpeed = 5f;
    private Vector2 moveDir;
    public float minX, maxX, minY, maxY;


    [Header("애니메이션 관련")] 
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDead = Animator.StringToHash("Dead");
    private readonly int hashShout = Animator.StringToHash("Shout");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashShovel = Animator.StringToHash("Shovel");
    private readonly int hashGameRestart = Animator.StringToHash("GameRestart");

    private void Awake() 
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
    // 상태 변수 초기화
        hasDeadAnimPlayed = false;
        isAttack = false;

        animator.ResetTrigger(hashGameRestart);
        animator.ResetTrigger(hashDead);

        animator.Rebind();
        animator.Update(0f);
    }

    void OnMove(InputValue value)
    {
        if (GameManager.instance.isDead)
        {
            moveDir = Vector2.zero;
            return;
        }
        moveDir = value.Get<Vector2>();
    }

    void Update()
    {   
        if (GameManager.instance.isDead)
        {
            if (!hasDeadAnimPlayed)
            {
                animator.SetTrigger(hashDead);
                hasDeadAnimPlayed = true;
            }
            return; 
        }

        float currentSpeed = moveDir.magnitude * moveSpeed;
        animator.SetFloat(hashSpeed, currentSpeed);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(hashShout);
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger(hashShovel);
        }

        if (moveDir.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDir.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance.isDead) return;
        
        Vector3 nextPos = transform.position + (Vector3)(moveDir * moveSpeed * Time.fixedDeltaTime);

        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);

        transform.position = nextPos;
    }

    void OnAttack()
    {
        if(GameManager.instance.isDead || isAttack) return;

        animator.SetTrigger(hashAttack);
        isAttack = true;
        StartCoroutine(AttackCooltime());
    }

    IEnumerator AttackCooltime()
    {
        yield return new WaitForSeconds(1f);
        isAttack = false;
    }
}
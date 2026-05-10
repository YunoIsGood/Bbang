using UnityEngine;
using System.Collections;

public class Monster1 : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 5f;            // 속도를 살짝 올리면 더 위협적입니다.
    public float attackCooldown = 1.0f; // 공격 사이의 간격
    
    [Header("Required Objects")]
    public GameObject attackHitbox;

    protected Rigidbody2D _rb;
    protected Transform _playerTrm;
    protected Animator _anim;
    
    private bool _inChaseRange = false;
    private bool _inAttackRange = false;
    private bool _isAttacking = false;
    protected bool _canAttack = true;

    private Coroutine _logicCoroutine;
    private const string PLAYER_TAG = "Player";

    private readonly int hash_isWalking = Animator.StringToHash("isWalking");
    private readonly int hash_Attack = Animator.StringToHash("Attack");

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        if (attackHitbox != null) attackHitbox.SetActive(false);
    }

    protected virtual void Start()
    {
        GameObject player = GameObject.FindWithTag(PLAYER_TAG);
        if (player != null) _playerTrm = player.transform;
    }

    public virtual void SetChasing(bool value)
    {
        _inChaseRange = value;
        if (_inChaseRange && _logicCoroutine == null)
            _logicCoroutine = StartCoroutine(MainLogicRoutine());
    }

    public void SetAttackRange(bool value) => _inAttackRange = value;

    private IEnumerator MainLogicRoutine()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        while (_inChaseRange && _playerTrm != null)
        {
            // 1. 공격 조건 체크
            if (_inAttackRange && _canAttack && !_isAttacking)
            {
                yield return StartCoroutine(AttackRoutine());
                // 공격 코루틴이 끝나면 바로 아래 '이동' 로직으로 넘어갑니다.
            }

            // 2. 이동 로직 (공격 중이 아닐 때 즉시 실행)
            if (!_isAttacking)
            {
                if (!_inAttackRange) 
                {
                    Vector2 dir = ((Vector2)_playerTrm.position - (Vector2)transform.position).normalized;
                    _rb.linearVelocity = dir * speed;
                    _anim.SetBool(hash_isWalking, true);
                    
                    if (Mathf.Abs(dir.x) > 0.1f)
                        transform.localScale = new Vector3(dir.x > 0 ? -1 : 1, 1, 1);
                }
                else 
                {
                    StopMonster();
                }
            }

            yield return wait;
        }

        StopMonster();
        _logicCoroutine = null;
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _canAttack = false;
        
        _rb.linearVelocity = Vector2.zero;
        _anim.SetBool(hash_isWalking, false);
        _anim.SetTrigger(hash_Attack);

        // --- 공격 판정 타이밍 ---
        yield return new WaitForSeconds(0.3f); 
        if (attackHitbox != null) attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        if (attackHitbox != null)attackHitbox.SetActive(false);

        // --- 포인트: 여기서 바로 공격 상태 해제 ---
        // 이렇게 하면 쿨타임이 도는 중에도 몬스터는 '추격'을 시작할 수 있습니다.
        _isAttacking = false; 

        // 쿨타임은 별도의 코루틴으로 돌려서 메인 로직을 방해하지 않게 합니다.
        StartCoroutine(CooldownRoutine());
    }

    protected virtual IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true;
    }

    private void StopMonster()
    {
        if (_rb != null) _rb.linearVelocity = Vector2.zero;
        if (_anim != null) _anim.SetBool(hash_isWalking, false);
    }
}
using UnityEngine;
using System.Collections;

public class Monster1 : MonoBehaviour
{
    [Header("Data Reference")]
    public MonsterData data; // ScriptableObject 연결

    [Header("Safe Zone")]
    public LayerMask safeZoneLayer;
    public float safeZoneCheckRadius = 0.2f;

    [Header("Required Objects")]
    public GameObject attackHitbox;

    // 개별 인스턴스의 상태 데이터
    private int _currentHp; 
    
    protected Rigidbody2D _rb;
    protected Transform _playerTrm;
    protected Animator _anim;

    private bool _inChaseRange = false;
    private bool _inAttackRange = false;
    private bool _isAttacking = false;
    private bool _canAttack = true;

    private Coroutine _logicCoroutine;
    private const string PLAYER_TAG = "Player";

    private readonly int hash_isWalking = Animator.StringToHash("isWalking");
    private readonly int hash_Attack = Animator.StringToHash("Attack");

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        if (attackHitbox != null) attackHitbox.SetActive(false);
        if (data != null)
        {
            _currentHp = data.maxHp;
        }
    }

    protected virtual void Start()
    {
        GameObject player = GameObject.FindWithTag(PLAYER_TAG);
        if (player != null) _playerTrm = player.transform;
    }

    public void SetChasing(bool value)
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
            if (IsPlayerInSafeZone())
            {
                StopMonster();
                yield return wait;
                continue;
            }

            if (_inAttackRange && _canAttack && !_isAttacking)
            {
                yield return StartCoroutine(AttackRoutine());
            }

            if (!_isAttacking)
            {
                if (!_inAttackRange)
                {
                    Vector2 dir = ((Vector2)_playerTrm.position - (Vector2)transform.position).normalized;
                    _rb.linearVelocity = dir * data.speed; 
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

    private bool IsPlayerInSafeZone()
    {
        if (_playerTrm == null) return false;
        return Physics2D.OverlapCircle(_playerTrm.position, safeZoneCheckRadius, safeZoneLayer) != null;
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _canAttack = false;

        _rb.linearVelocity = Vector2.zero;
        _anim.SetBool(hash_isWalking, false);
        _anim.SetTrigger(hash_Attack);

        yield return new WaitForSeconds(0.3f);
        if (attackHitbox != null) attackHitbox.SetActive(true);
        
        
        yield return new WaitForSeconds(0.2f);
        if (attackHitbox != null) attackHitbox.SetActive(false);

        _isAttacking = false;
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(data.attackCooldown);
        _canAttack = true;
    }

    private void StopMonster()
    {
        if (_rb != null) _rb.linearVelocity = Vector2.zero;
        if (_anim != null) _anim.SetBool(hash_isWalking, false);
    }
    
    public void TakeDamage(int amount)
    {
        _currentHp -= amount;
        if(_currentHp <= 0) 
        {
            Debug.Log($"{data.monsterName} 사망!");
            Destroy(gameObject);
        }
    }
}
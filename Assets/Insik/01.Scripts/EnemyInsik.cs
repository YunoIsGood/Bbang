using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
public class EnemyInsik : MonoBehaviour, IEnemyinterface
{
  private GameObject _Player;
    
    
    public Vector2 _moveDir;
    
    bool _Attacked = false;

    private float _moveSpeed = 3f;

    Rigidbody2D _rb;

    // 추격 여부 처음엔 안하고 태어난 곳에서 어슬렁
    public bool _chasing = false;
    
    // 플레이어 위치
    private Transform _playerPlace;

    //태어난 곳
    private Vector2 _BirthPoint;

    private float timer = 10f;

    
    private void Start()
    {
        _Player = GameObject.Find("Player");
       
        _playerPlace = _Player.transform;
        
        _rb = GetComponent<Rigidbody2D>();
        // 태어난 자리 저장
        _BirthPoint = transform.position;

        
    }

    // 세이프 존 안에서는 무조건 추격 안함. 젤다 몽환의 모레시계 그 성역 참고함 
    private void Update()
    {
        _moveDir = ((Vector3)_BirthPoint - transform.position).normalized;
        IEnemyinterface enemy = GetComponent<EnemyInsik>();
        enemy.SaveZone();
        
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDir * _moveSpeed;
    }

   public void SaveZone()
    {
        if ( GameManager.instance.isSafeZone == true)
        {
            _chasing = false;
           Looking();
        }
        else
        {
            _chasing = true;
            Chase();
        }

    }


    // 공격 당함 그리고 피격 시 효과 와 효과 지속시간.
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.gameObject.CompareTag("Player") && _Attacked == false)
        {
           PlayerMovement.instance.moveSpeed *= -1;
            StartCoroutine(Revalsaltime());
            _Attacked = true;
        }
    }

    // 공격 맞으면 조작 반전됨 그리고 10 초 기다림
    IEnumerator Revalsaltime()
    {
        yield return new WaitForSeconds(10f);
        PlayerMovement.instance.moveSpeed *= -1;
        _Attacked = false;
    }

    // 적 추격 
    public void Chase()
    {
        _chasing = true;
            //시아에 걸리면 빠르게 플레이어를 향해 돌격함 단 세이브 존일 경우 추격이 멈추고 있던 자리로 돌아감
            _moveDir = (_playerPlace.transform.position - transform.position ).normalized;
           
    }
    
  


    //추격 실폐 혹은 처음 상태
    public void Looking()
    {
        _chasing = false;
        // 찾다가 놓치면 태어난 자리로 돌아감 참고한 몬스터는 젤다의 전설 몽환의 모레시계의 펜텀.
        _moveDir = ((Vector3)_BirthPoint - transform.position).normalized;
        
    }
        
}


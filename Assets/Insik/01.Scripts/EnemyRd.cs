using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class EnemyRd : MonoBehaviour, IEnemyinterface
{
    private Vector2 _moveDir;

    private float _moveSpeed = 3f;

    Rigidbody2D _rb;

    Vector2 _BirthPoint;

    Transform _playerPlace;

 

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _BirthPoint = transform.position;
        _playerPlace = GameObject.Find("Player").transform;
       
    }
    private void Update()
    {
        _moveDir = ((Vector3)_BirthPoint - transform.position).normalized;
        IEnemyinterface enemy = GetComponent<EnemyRd>();
        enemy.SaveZone();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDir * _moveSpeed;
    }

    public void SaveZone()
    {
        if (GameManager.instance.isSafeZone == true)
        {

            Looking();
        }
        else
        {

            Chase();
        }

    }

    // 적 추격 
    public void Chase()
    {

        //시아에 걸리면 빠르게 플레이어를 향해 돌격함 단 세이브 존일 경우 추격이 멈추고 있던 자리로 돌아감
        _moveDir = (_playerPlace.transform.position - transform.position).normalized;

    }

    //추격 실폐 혹은 처음 상태
    public void Looking()
    {

        // 찾다가 놓치면 태어난 자리로 돌아감 참고한 몬스터는 젤다의 전설 몽환의 모레시계의 펜텀.
        _moveDir = ((Vector3)_BirthPoint - transform.position).normalized;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.TakeDamage(10);
            PlayerMovement.instance.moveSpeed *= 0.78f;
            StartCoroutine(StopTime());
        }
    }
    IEnumerator StopTime()
    {
        _moveSpeed = 0;
        yield return new WaitForSeconds(1f);
        _moveSpeed = 3f;
        PlayerMovement.instance.moveSpeed = 5f; 
    }
}
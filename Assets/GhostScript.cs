using UnityEngine;
using System.Collections;

public class GhostScript : MonoBehaviour, IDamageable
{
    public MonsterData data;
    public Transform playerTrm;
    float moveSpeed = 3;
    int damage = 10;
    bool isAttack = true;
    public bool isChasing = false;
    private const string PLAYER_TAG = "Player";

    void Start()
    {
        GameObject player = GameObject.FindWithTag(PLAYER_TAG);
        if (player != null) playerTrm = player.transform;
    }

    void Update()
    {
        Vector3 moveDir = (playerTrm.position - transform.position).normalized;
        if(isChasing)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")&&isAttack)
        {
            GameManager.instance.TakeDamage(damage);
            isAttack = false;
            StartCoroutine(CoolTime(1));

        }
    }

    IEnumerator CoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        isAttack = true;
    }

    public void TakeDamage(int damage)
    {
        if (data != null)
        {
            data.health -= damage;
            if (data.health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

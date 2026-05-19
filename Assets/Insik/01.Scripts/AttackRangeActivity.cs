using JetBrains.Annotations;
using UnityEngine;
using System.Collections;


public class AttackRangeActivity : MonoBehaviour
{
    private Vector2 _AttackPoint;
    
    public Transform Player;

    [SerializeField] private GameObject AttackEffect;  

    private void Start()
    {
        Player = GameObject.Find("Player").transform;
        transform.position = Player.transform.position;
       

    }
    private void Update()
    {
         transform.rotation = Player.transform.rotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("공격 성공!");
            
            Destroy(collision.gameObject);
           
            Instantiate(AttackEffect, collision.transform.position, Quaternion.identity);
            StartCoroutine(DestroyEffect());

        }
    }

    private IEnumerator DestroyEffect()
    {
        Destroy(AttackEffect);
        yield return new WaitForSeconds(1f);
    }
}

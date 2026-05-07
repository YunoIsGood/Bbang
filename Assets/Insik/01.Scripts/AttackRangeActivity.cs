using JetBrains.Annotations;
using UnityEngine;
using System.Collections;


public class AttackRangeActivity : MonoBehaviour
{
    private Vector2 _AttackPoint;
    
    public Transform Player;


    private void Start()
    {
        Player = GameObject.Find("Player").transform;
        transform.position = Player.transform.position;

    }
    private void Update()
    {
         float v = Input.GetAxisRaw("Vertical");

         float h = Input.GetAxisRaw("Horizontal");
        if (v > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (v < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (h > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (h < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("공격 성공!");
            Destroy(collision.gameObject);
            

        }
    }
}

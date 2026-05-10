using UnityEngine;

public class AttackRangeCheck : MonoBehaviour
{

    private IChaseRange owner; //모든 몬스터 사용가능

    private void Awake()
    {
        if (owner != null) //null이지 않았을때
        {
            owner = GetComponentInParent<IChaseRange>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (owner != null)
            {
                owner.SetAttackRange(true);
            }
       
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (owner != null)
            {
                owner.SetAttackRange(false);
            }
        
        }
    }
}
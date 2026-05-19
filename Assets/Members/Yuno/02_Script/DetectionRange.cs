using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    // 부모의 Monster1 스크립트를 인스펙터에서 배정해야 함
    private IChaseRange owner;

    private void Awake()
    {
        // 혹은 자동으로 부모에서 찾아옴
        
        if (owner == null)
        {
            owner = GetComponentInParent<IChaseRange>();
            Debug.Log($"owner 찾음: {owner}"); // null이면 여기서 잡힘
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Player")) 
        {
            if (owner != null)
            {
                 owner.SetChasing(true);
                 Debug.Log($"트리거 감지: {collision.tag}"); // 플레이어 감지 자체가 되는지 확인
            }
            else
            {
               owner.SetChasing(true);
            }
            
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (owner != null)
            {
                owner.SetChasing(false);
            }
            else
            {
                owner.SetChasing(false);
            }
            
        }
    }
}
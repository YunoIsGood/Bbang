using Unity.VisualScripting;
using UnityEngine;

public class HP : MonoBehaviour
{

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.instance.TakeDamage(10);
         
        }
        if(collision.gameObject.CompareTag("Respawn"))
        {
            GameManager.instance.currentHealth = GameManager.instance.maxHealth;
        }
    }
}
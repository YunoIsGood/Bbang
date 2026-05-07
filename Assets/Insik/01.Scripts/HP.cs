using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using TMPro;

public class HP : MonoBehaviour
{
    
    bool isInvincible = false;
    private float _invicible = 1f; // 무적 딜레이 변수임


    // 이건 나중에 OnCollisionEnter2D로 바꿔야 함 안 그러면 Enemy 계열을 전부 isTrigger로 해야함 ㅇㅇ 그리고 무적시간 때문에 적을 통과함 필 수정
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            // 만약 강욱이가 적 혹은 그 공격에 따라 데미지를 다르게들 두면 수정함
            GameManager.instance.TakeDamage(10);
            StartCoroutine(Invincibility());
        }
        if(collision.gameObject.CompareTag("Respawn"))
        {
            GameManager.instance.currentHealth = GameManager.instance.maxHealth;
           
        }
        if (GameManager.instance.currentHealth <= 0)
        {
            //사1망시 더 월드 되고 나중에 사2망 애니메이션 넣을듯 암튼 더 월드 뒤 게임오버 페널 오픈함 됨 
            Time.timeScale = 0;
        }
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(_invicible); // 1초 동안 무적 상태 유지 단점: 여기서도 시간 바꿔야해서 귀찮음 무적시간 바꾸면 PlayerMovement에서 변수로 해결할 예정
        isInvincible = false;
    }

}

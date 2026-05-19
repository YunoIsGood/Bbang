using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Attack : MonoBehaviour
{
   
    public GameObject sword; // 원본 프리팹, 이놈을 어떡게 하면 됨
    bool cooltime = true; 

    



    private void Update()
    {
        
        
        if (Keyboard.current.lKey.wasPressedThisFrame && cooltime)
        {
            // 1. 복사본 생성, 무기가 많아지면 수정예정
            GameObject DemoWeapon = Instantiate(sword);

            // 2. 위치, 무기와 플레이어가 따로 노는 문제가 있어서 수정예정
            DemoWeapon.transform.position = transform.position;

           
            Destroy(DemoWeapon, 0.5f);
            cooltime = false;

            StartCoroutine(AttackDelay());
        }
    }

    //공격 딜레이,  WaitForSeconds()의 괄호 안의 숫자를 바꿔서 딜레이 시간 변동 가능.
    IEnumerator AttackDelay()
    {
       cooltime=false;
        yield return new WaitForSeconds(0.5f);
        cooltime = true;
    }
    
}

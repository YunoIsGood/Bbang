using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
public class Attack : MonoBehaviour
{
   
    public GameObject sword; // 원본 프리팹 이였는데 지금은 공격 범워 넣음 
    
    bool cooltime = true;

    public Transform Player;

  
    
    private Vector2 LastMoved;


    private void Start()
    {
        Player = GameObject.Find("Player").transform;
        
 
    }



    private void Update()
    {
        float v = Input.GetAxisRaw("Vertical");

        float h = Input.GetAxisRaw("Horizontal");
        if(h > 0)
        {
            Player.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if(h < 0)
        {
            Player.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if(v > 0)
        {
            Player.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if(v < 0)
        {
            Player.transform.rotation = Quaternion.Euler(0, 0, 180);
        }


        if (Keyboard.current.lKey.wasPressedThisFrame && cooltime)
        {
            // 1. 복사본 생성
            GameObject WeaponRange = Instantiate(sword);

            // 2. 부모를 Player로 설정 (이제 자동으로 따라다님)
            WeaponRange.transform.SetParent(Player);

            // 3. 위치값 설정, 특:플레이어 이동 방향에 따라서 위치값이 달라짐, 코드가 아주 나폴리 스파게티가 따로 없어서 추후 더 좋은거 알아내면 바꿀 예정



            Destroy(WeaponRange, 0.1f);
            cooltime = false;

            StartCoroutine(AttackDelay());
        }
    }

    //공격 딜레이,  WaitForSeconds()의 괄호 안의 숫자를 바꿔서 딜레이 시간 변동 가능.
    IEnumerator AttackDelay()
    {
       cooltime=false;
        yield return new WaitForSeconds(0.2f);
        cooltime = true;
    }
   

}

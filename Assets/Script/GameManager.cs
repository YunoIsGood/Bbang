using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;


  

public class GameManager : MonoBehaviour

{

    public static GameManager instance;//다른 스크립트에서 GameManager.instance.~~~로 접근가능하게 해줌(싱글톤)
    [SerializeField] private ScoreManager scoreManager;



    [Header("Player Stats")]

    public float currentBattery = 100f;//현재 배터리 용량

    public float maxBattery = 100f;//최대 배터리 용량

    public int currentHealth = 100;//현재 체력

    public int maxHealth = 100;//최대 체력

    public int money = 0;//돈


    [Header("SafeZone")]

    public bool isSafeZone = false; // 안전지대 여부

    void Start()
    {
        isSafeZone = true;
        scoreManager.Battery(currentBattery);
        scoreManager.AddHp(currentHealth);
    }


    void Update()  

    {

        if(isSafeZone == false)

        {

            float decreaseAmount = 3f * Time.deltaTime;

            GameManager.instance.UseBattery(decreaseAmount);



        }

        else if(isSafeZone == true)

        {

            currentHealth = maxHealth;

            currentBattery = maxBattery;

            scoreManager.Battery(currentBattery);

            scoreManager.AddHp(currentHealth);


        }

    }

    void Awake()

    {
        scoreManager = GetComponent<ScoreManager>();
        //싱글톤



        if (instance == null)

        {

            instance = this;

            DontDestroyOnLoad(gameObject);

        }

        else

        {

            Destroy(gameObject);

        }

    }

  
  

    // 1. 돈 관리

    public void AddMoney(int amount)

    {

        money += amount;

        Debug.Log($"보물을 팔았습니다! 현재 잔액: {money}원");

    }

  

    // 2. 체력 관리

    public void TakeDamage(int damage)

    {

        if (isSafeZone) return; // 안전지대 안이면 체력 안닳음

  

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); //Mathf.Clamp(현재값, 최소값, 최대값), 체력이 -10이나 150 이렇게 되는거 막아줌

        scoreManager.AddHp(currentHealth);

        if (currentHealth <= 0)

        {

            GameOver();

        }

    }

  

    // 3. 배터리 관리

    public void UseBattery(float amount)

    {

        if (isSafeZone) return; // 안전지대에서는 소모 안 함

  

        currentBattery -= amount;

        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);

        scoreManager.Battery(currentBattery);

    }

  

  

    // 4. 침대에서 잘 때

    public void RestoreAll()

    {

        currentHealth = maxHealth;

        currentBattery = maxBattery;

        scoreManager.AddHp(currentHealth); //체력 회복 보이게 하는 메서드 호출

        scoreManager.Battery(currentBattery); //배터리 회복 보이게 하는 메서드 호출

        Debug.Log("휴식을 취해 체력과 배터리가 회복되었습니다.");

        // 여기에 맵 리셋(몬스터/보물 리스폰) 신호를 추가하면 됨

    }

    //플레이어 사망시

    void GameOver()

    {

        Debug.Log("게임 오버");
        
        // 여기에 아이템 초기화 및 맵 리셋 하는거 넣기

    }
    

    

  

}
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("플레이어 능력치")]
    public float currentBattery = 100f; // 현재 배터리 용량
    public float maxBattery = 100f;    // 최대 배터리 용량
    public int currentHealth = 100;    // 현재 체력
    public int maxHealth = 100;       // 최대 체력
    public int money = 0;             // 돈

    [Header("세이프존")]
    public bool isSafeZone = false;    // 세이프존 확인 

    [Header("헤드라이트")]
    public Light2D headLight;          // 헤드라이트(Light2D)
    public bool isBatteryOff = false;  // 배터리 꺼짐 확인
    public float baseInnerRadius = 5f; // 헤드라이트 깔끔하게 보이는 범위
    public float baseOuterRadius = 6.5f; // 헤드라이트 흐릿하게 보이는 범위

    [Header("인벤토리 설정")]
    public List<TreasureData> myInventory = new List<TreasureData>(); // 인벤토리 리스트
    public int maxInventorySlots = 4; // 최대 가방 용량
    public GameObject GameOverPanel; // 게임오버 패널

    void Awake() //싱글톤 패턴
    {
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

    void Update()
    {
        if (!isSafeZone) //세이프존이 아니라면 매 초마다 3만큼 배터리 줄어듦
        {
            UseBattery(3f * Time.deltaTime);
        }
        else //세이프존 이라면 현재 체력,배터리를 최대 체력,배터리로 설정
        {
            currentHealth = maxHealth;
            currentBattery = maxBattery;
        }

        isBatteryOff = currentBattery <= 0; // 현재 배터리가 0보다 작다면 isBatteryOff

        // [수정] 씬 리로드 후 headLight가 null이 될 수 있으므로 체크 추가
        if (headLight != null)
        {
            if (isBatteryOff) //배터리가 꺼졌으면 헤드라이트 범위 0으로
            {
                headLight.pointLightInnerRadius = 0;
                headLight.pointLightOuterRadius = 0;
            }
            else //배터리가 안꺼졌을때는 헤드라이트 범위를 변수에 설정한 값으로
            {
                headLight.pointLightInnerRadius = baseInnerRadius;
                headLight.pointLightOuterRadius = baseOuterRadius;
            }
        }
    }

    public void AddMoney(int amount) //돈 관리 AddMoney(매개변수)
    {
        money += amount;//보유한 돈에 매개변수만큼 더해주기
    }

    public void TakeDamage(int damage)//데미지,체력 관리 
    {
        if (isSafeZone) return;//세이프존 안이라면 반환

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);//현재 체력의 최소값은 0 최댓값은 최대체력까지만

        if (currentHealth <= 0)//체력이 0보다 낮아지면 게임오버
        {
            GameOverPanelOpen();
        }
    }

    public void UseBattery(float amount)// 배터리 관리 
    {
        if (isSafeZone) return;//세이프존 안이라면 반환

        currentBattery = Mathf.Clamp(currentBattery - amount, 0, maxBattery);//현재 배터리의 최소값은 0 최댓값은 최대 배터리 용량까지만
    }

    public bool CanAddItem()//인벤토리에 넣을 수 있는지 확인
    {
        return myInventory.Count < maxInventorySlots;//현재 인벤토리 카운트가 최대 인벤토리 용량보다 크면 반환 
    }

    public void AddToInventory(TreasureData data)//인벤토리에 찾은 보물 추가하기
    {
        if (CanAddItem())//인벤토리에 넣을 수 있는지 확인 후
        {
            myInventory.Add(data);//인벤토리에 찾은 보물 데이터 추가
            Debug.Log($"{data.treasureName} 획득! ({myInventory.Count}/{maxInventorySlots})");
        }
    }

    public void GameOver()
    {
        currentHealth = maxHealth; // 현재 체력 초기화
        currentBattery = maxBattery; // 현재 배터리 초기화
        money = 0; // 돈 초기화
        myInventory.Clear(); // 인벤토리 비우기
        isSafeZone = false; // 세이프존 상태 초기화

        // 현재 씬을 다시 로드 (씬이 다시 시작됨)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void GameOverPanelOpen()
    {
        if (GameOverPanel != null)
        {
            Time.timeScale = 0;
            GameOverPanel.SetActive(true);
        }
    }
}
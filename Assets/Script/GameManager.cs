using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering; // 추가됨
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;//싱글톤 패턴
    public bool isDead = false;

    [Header("플레이어 능력치")]
    public float currentBattery = 100f;//현재 배터리
    public float maxBattery = 100f;//최대 배터리
    public int currentHealth = 100;//현재 체력
    public int maxHealth = 100;//최대 체력
    public int money = 0;//돈

    [Header("세이프존")]
    public bool isSafeZone = false;//세이프존 여부

    [Header("헤드라이트")]
    public Light2D headLight;//헤드라이트 
    public bool isBatteryOff = false;//배터리 방전 여부
    public float baseInnerRadius = 5f;//헤드라이트 반경
    public float baseOuterRadius = 6.5f;//헤드라이트 그라데이션 반경(헤드라이트 반경보다 약간 더 크게 설정)

    [Header("포스트 프로세싱 설정")]
    public Volume globalVolume;//포스트 프로세싱 볼륨      
    public float pulseSpeed = 5f;//체력 낮을 때 깜빡이는 효과 속도
    public float maxIntensity = 0.6f; //체력 낮을 때 빨갛게 되는 정도
    private Vignette vignette;//비네트

    [Header("인벤토리 설정")]
    public List<TreasureData> myInventory = new List<TreasureData>();//인벤토리 리스트
    public int maxInventorySlots = 4;//최대 인벤토리 슬롯 수
    public GameObject GameOverPanel;//게임오버 패널

    void Awake()
    {
        if (instance == null)//싱글톤
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (globalVolume != null && globalVolume.profile.TryGet<Vignette>(out var tmpVignette))//포스트 프로세싱에서 비네트 효과 가져오기
            {
                vignette = tmpVignette;//비네트 효과가 있으면 저장
            }
        }
        else
        {
            Destroy(gameObject);//이미 인스턴스가 있으면 자신을 파괴(2개 이상 존재 방지)
        }
    }

    void Start()
    {
        isDead = false;
    }

    void Update()
    {
        // 1. 배터리 소모 로직
        if (!isSafeZone)//세이프존 안이면 반환
        {
            UseBattery(3f * Time.deltaTime);//초당 3씩 배터리 소모
        }
        else
        {
            currentHealth = maxHealth;//세이프존 안에서는 체력 회복
            currentBattery = maxBattery;//세이프존 안에서는 배터리 회복
        }

        isBatteryOff = currentBattery <= 0;//배터리가 0이하면 방전 상태로 설정

        // 2. 헤드라이트 로직
        if (headLight != null)//헤드라이트 없으면 반환
        {
            if (isBatteryOff)//배터리 꺼졌으면
            {
                headLight.pointLightInnerRadius = 0;//헤드라이트 반경 0으로 설정
                headLight.pointLightOuterRadius = 0;//헤드라이트 그라데이션 반경 0으로 설정
            }
            else//배터리 안꺼졌으면
            {
                headLight.pointLightInnerRadius = baseInnerRadius;//헤드라이트 반경 현재값으로 설정
                headLight.pointLightOuterRadius = baseOuterRadius;//헤드라이트 그라데이션 반경 현재값으로 설정
            }
        }

        VignetteEffect();
    }

    void VignetteEffect()
    {
        if (globalVolume == null)//글로벌 볼륨 없으면 찾아서 가져오기
        {
            globalVolume = GameObject.FindFirstObjectByType<Volume>();//씬에 있는 볼륨을 찾아서 가져오기
            if (globalVolume != null) globalVolume.profile.TryGet<Vignette>(out vignette);//볼륨 안에서 비네트 효과 가져오기
        }

        if (vignette == null) return;//비네트 효과 없으면 반환

        float healthRatio = (float)currentHealth / maxHealth;//체력 비율 계산
        float invHealthRatio = 1f - healthRatio;//체력 비율의 역수 계산(체력이 낮을수록 높아짐)

        float baseIntensity = invHealthRatio * (maxIntensity * 0.4f);//체력 낮을수록 기본적으로 빨갛게 되는 정도 계산(최대의 40%까지만 적용)
        float pulse = 0f;//깜빡이는 효과 초기값

        if (healthRatio <= 0.3f && healthRatio > 0)//체력이 30% 이하이면서 0보다 크면
        {
            pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;//사인 함수 이용해서 0과 1 사이로 깜빡이는 효과 계산
            pulse *= invHealthRatio * maxIntensity;//체력이 낮을수록 깜빡이는 효과가 더 강해지도록 체력 비율의 역수와 최대 강도를 곱해서 적용
        }

        vignette.color.Override(Color.red);//비네트 색상은 빨간색으로 설정
        vignette.intensity.Override(baseIntensity + pulse);//비네트 강도는 기본적으로 체력 낮을수록 빨갛게 되는 정도에 깜빡이는 효과를 더해서 설정
    }


    public void AddMoney(int amount)//돈 추가 함수
    {
        money += amount;//현재 돈에 매개변수만큼 더하기
    }

    public void TakeDamage(int damage)
    {
        if (isSafeZone || isDead) return; // 이미 죽었으면 무시

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        StartCoroutine(Flash());

        if (currentHealth <= 0)
        {
            isDead = true; // 죽음 상태 확정
            if(vignette != null) vignette.intensity.Override(0);
        
            Time.timeScale = 0; // 즉시 시간 정지 (애니메이터는 Unscaled이므로 작동함)
            StartCoroutine(Timer(3)); // 3초 대기 시작
        }
    }

    public void UseBattery(float amount) //배터리 사용 함수
    {
        if (isSafeZone) return;//세이프존 안이면 반환
        currentBattery = Mathf.Clamp(currentBattery - amount, 0, maxBattery);//배터리 범위를 0과 최대 배터리 사이로 제한
    }

    public bool CanAddItem()//인벤토리에 아이템 추가 가능한지 확인하는 함수
    {
        return myInventory.Count < maxInventorySlots;//현재 인벤토리 아이템 수가 최대 슬롯 수보다 작은지 반환 (AddToInventory에 bool값 반환)
    }

    public void AddToInventory(TreasureData data)//인벤토리에 아이템 추가하는 함수
    {
        if (CanAddItem())//아이템 추가 가능한지 확인
        {
            myInventory.Add(data);//인벤토리에 아이템 추가
            Debug.Log($"{data.treasureName} 획득! ({myInventory.Count}/{maxInventorySlots})");//획득한 아이템 이름과 현재 인벤토리 상태 출력
        }
    }
    
    IEnumerator Flash() //피격 시 깜빡이는 효과 코루틴
    {
        for (float t = 0; t < 1f; t += Time.deltaTime / 0.01f) { vignette.intensity.Override(Mathf.Lerp(0, 0.25f, t)); yield return null; }//0.01초만큼 0에서 0.25까지 깜빡이는 효과
        for (float t = 0; t < 1f; t += Time.deltaTime / 0.3f) { vignette.intensity.Override(Mathf.Lerp(0.25f, 0, t)); yield return null; }//0.3초만큼 0.25에서 0까지 사라지는 효과
        vignette.intensity.Override(0);//끝나면 비네트 강도 초기화
    }   

    public void GameOver()
    {
        // 다시 시작할 때 필수 데이터 초기화
        isDead = false; 
        currentHealth = maxHealth;
        currentBattery = maxBattery;
        money = 0;
        myInventory.Clear();
        isSafeZone = false;

        if(vignette != null) {vignette.intensity.Override(0);}
    
        Time.timeScale = 1; // 시간 원상복구
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator Timer(int time)
    {
        yield return new WaitForSecondsRealtime(time); 
        GameOverPanelOpen();
    }

    public void GameOverPanelOpen()
    {
        if (GameOverPanel != null)//게임오버 패널이 있으면
        {
            GameOverPanel.SetActive(true);//게임오버 패널 활성화
        }
    }
}
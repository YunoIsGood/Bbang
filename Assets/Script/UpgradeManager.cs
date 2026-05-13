using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;//상점 패널 (이 안에 상점 버튼,텍스트 넣으면 됨)
    [SerializeField] private Button shopButton;//상점 버튼

    [Header("업그레이드 수치 및 UI")]
    int lightUpgradeLevel = 0; //헤드라이트 업그레이드 레벨
    int lightUpgradeGold = 10;//헤드라이트 업그레이드에 필요한 돈
    [SerializeField] private TextMeshProUGUI lightUpgradeText;//헤드라이트 업그레이드 UI 텍스트

    int hpUpgradeLevel = 0; //체력 업그레이드 레벨
    int hpUpgradeGold = 50;//체력 업그레이드에 필요한 돈
    [SerializeField] private TextMeshProUGUI hpUpgradeText;//체력 업그레이드 UI 텍스트

    int batteryUpgradeLevel = 0; //배터리 업그레이드 레벨
    int batteryUpgradeGold = 50;//배터리 업그레이드에 필요한 돈
    [SerializeField] private TextMeshProUGUI batteryUpgradeText;//배터리 업그레이드 UI 텍스트

    int bagUpgradeLevel = 0;//가방 업그레이드 레벨
    int bagUpgradeGold = 80; //가방 업그레이드에 필요한 돈
    [SerializeField] private TextMeshProUGUI bagUpgradeText;//가방 업그레이드 UI 텍스트

    void Start()
    {
        UpdateAllUI(); //상점 UI 초기 업데이트
        shopPanel.SetActive(false);//초기에는 상점 패널 비활성화
    }

    void UpdateAllUI() 
    {
        lightUpgradeText.text = $"시야 범위: {GameManager.instance.baseInnerRadius:F1}\n레벨: {lightUpgradeLevel}\n{lightUpgradeGold}G"; //헤드라이트 업그레이드 UI 텍스트 업데이트 (현재 시야 범위, 레벨, 필요한 돈 표시)
        hpUpgradeText.text = $"체력: {GameManager.instance.maxHealth}\n레벨: {hpUpgradeLevel}\n{hpUpgradeGold}G";//체력 업그레이드 UI 텍스트 업데이트 (현재 최대 체력, 레벨, 필요한 돈 표시)
        batteryUpgradeText.text = $"배터리 용량: {GameManager.instance.maxBattery}\n레벨: {batteryUpgradeLevel}\n{batteryUpgradeGold}G";//배터리 업그레이드 UI 텍스트 업데이트 (현재 최대 배터리, 레벨, 필요한 돈 표시)
        bagUpgradeText.text = $"가방 용량: {GameManager.instance.maxInventorySlots} \n레벨: {bagUpgradeLevel}\n{bagUpgradeGold}G";//가방 업그레이드 UI 텍스트 업데이트 (현재 최대 인벤토리 슬롯, 레벨, 필요한 돈 표시)
    }

    public void BagUpgrade() //가방 업그레이드 함수
    { 
        if (GameManager.instance.money < bagUpgradeGold) return;//돈이 부족하면 반환

        GameManager.instance.money -= bagUpgradeGold;//돈 차감
        GameManager.instance.maxInventorySlots += 2; //최대 인벤토리 슬롯 2개 증가
        bagUpgradeLevel++;//가방 업그레이드 레벨 증가
        bagUpgradeGold += 100; //다음 가방 업그레이드에 필요한 돈 증가
        
        UpdateAllUI(); // 상점 UI 갱신

        UIManager ui = Object.FindFirstObjectByType<UIManager>();//씬 내에서 UIManager 인스턴스 찾아서 참조 저장
        if (ui != null) ui.UpdateInventoryUI();//UIManager의 UpdateInventoryUI 함수 호출하여 인벤토리 UI 업데이트
    }

    public void HPUpgrade() //체력 업그레이드 함수
    {
        if(GameManager.instance.money < hpUpgradeGold) return;//돈이 부족하면 반환
        GameManager.instance.money -= hpUpgradeGold;//돈 차감
        GameManager.instance.maxHealth += 10;//최대 체력 10 증가
        hpUpgradeLevel++; hpUpgradeGold += 30; //체력 업그레이드 레벨 증가, 다음 체력 업그레이드에 필요한 돈 증가
        UpdateAllUI();//상점 UI 갱신
    }

    public void BatteryUpgrade() //배터리 업그레이드 함수
    {
        if(GameManager.instance.money < batteryUpgradeGold) return;//돈이 부족하면 반환
        GameManager.instance.money -= batteryUpgradeGold;//돈 차감
        GameManager.instance.maxBattery += 10;//최대 배터리 10 증가
        batteryUpgradeLevel++; //배터리 업그레이드 레벨 증가,
        batteryUpgradeGold += 30;//다음 배터리 업그레이드에 필요한 돈 증가
        UpdateAllUI();//상점 UI 갱신
    }

    public void LightUpgrade()//헤드라이트 업그레이드 함수
     {
        if(GameManager.instance.money < lightUpgradeGold) return;//돈이 부족하면 반환
        GameManager.instance.money -= lightUpgradeGold;//돈 차감
        lightUpgradeLevel++;//헤드라이트 업그레이드 레벨 증가
        GameManager.instance.baseInnerRadius += 1.15f;//헤드라이트 반경 증가
        GameManager.instance.baseOuterRadius += 1.15f;//헤드라이트 그라데이션 반경 증가
        lightUpgradeGold += 20;//다음 헤드라이트 업그레이드에 필요한 돈 증가
        UpdateAllUI();//상점 UI 갱신
    }

        void OnTriggerEnter2D(Collider2D collision) { if(collision.CompareTag("Player")) shopPanel.SetActive(true);}//플레이어가 상점 트리거에 들어오면 상점 패널 활성화
        void OnTriggerExit2D(Collider2D collision) { if(collision.CompareTag("Player")) shopPanel.SetActive(false);}//플레이어가 상점 트리거에서 나가면 상점 패널 비활성화


    
}
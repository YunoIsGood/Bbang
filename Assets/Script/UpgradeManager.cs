using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;//상점 패널 (이 안에 상점 버튼,텍스트 넣으면 됨)
    [SerializeField] private Button shopButton;

    [Header("업그레이드 수치 및 UI")]
    int lightUpgradeLevel = 0; //헤드라이트 업그레이드 레벨
    int lightUpgradeGold = 10;//헤드라이트 업그레이드에 필요한 돈
    [SerializeField] private TextMeshProUGUI lightUpgradeText;

    int hpUpgradeLevel = 0; 
    int hpUpgradeGold = 50;
    [SerializeField] private TextMeshProUGUI hpUpgradeText;

    int batteryUpgradeLevel = 0; 
    int batteryUpgradeGold = 50;
    [SerializeField] private TextMeshProUGUI batteryUpgradeText;

    int bagUpgradeLevel = 0;
    int bagUpgradeGold = 80; 
    [SerializeField] private TextMeshProUGUI bagUpgradeText;

    void Start()
     {
        UpdateAllUI(); 
        shopPanel.SetActive(false);
    }

    void UpdateAllUI() 
    {
        lightUpgradeText.text = $"시야 범위: {GameManager.instance.baseInnerRadius:F1}\n레벨: {lightUpgradeLevel}\n{lightUpgradeGold}G";
        hpUpgradeText.text = $"체력: {GameManager.instance.maxHealth}\n레벨: {hpUpgradeLevel}\n{hpUpgradeGold}G";
        batteryUpgradeText.text = $"배터리 용량: {GameManager.instance.maxBattery}\n레벨: {batteryUpgradeLevel}\n{batteryUpgradeGold}G";
        bagUpgradeText.text = $"가방 용량: {GameManager.instance.maxInventorySlots} \n레벨: {bagUpgradeLevel}\n{bagUpgradeGold}G";
    }

    public void BagUpgrade() 
    { 
        if (GameManager.instance.money < bagUpgradeGold) return;

        GameManager.instance.money -= bagUpgradeGold;
        GameManager.instance.maxInventorySlots += 2; 
        bagUpgradeLevel++;
        bagUpgradeGold += 100; 
        
        UpdateAllUI(); // 상점 UI 갱신

        // 인벤토리 UI 즉시 갱신 (추가된 부분)
        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui != null) ui.UpdateInventoryUI();
    }

    public void HPUpgrade() 
    {
        if(GameManager.instance.money < hpUpgradeGold) return;
        GameManager.instance.money -= hpUpgradeGold;
        GameManager.instance.maxHealth += 10;
        hpUpgradeLevel++; hpUpgradeGold += 30; 
        UpdateAllUI();
    }

    public void BatteryUpgrade() 
    {
        if(GameManager.instance.money < batteryUpgradeGold) return;
        GameManager.instance.money -= batteryUpgradeGold;
        GameManager.instance.maxBattery += 10;
        batteryUpgradeLevel++; batteryUpgradeGold += 30;
        UpdateAllUI();
    }

    public void LightUpgrade()
     {
        if(GameManager.instance.money < lightUpgradeGold) return;
        GameManager.instance.money -= lightUpgradeGold;
        lightUpgradeLevel++;
        GameManager.instance.baseInnerRadius += 1.15f;
        GameManager.instance.baseOuterRadius += 1.15f;
        lightUpgradeGold += 20;
        UpdateAllUI();
    }

        void OnTriggerEnter2D(Collider2D collision) { if(collision.CompareTag("Player")) shopPanel.SetActive(true);}
        void OnTriggerExit2D(Collider2D collision) { if(collision.CompareTag("Player")) shopPanel.SetActive(false);}


    
}
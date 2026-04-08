using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Image hpBar;
    public Image batteryBar;

    [Header("보물 획득 UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Image resultImage;
    [SerializeField] private TextMeshProUGUI resultNameText;
    [SerializeField] private TextMeshProUGUI resultPriceText;
    [SerializeField] private Button keepButton; 
    [SerializeField] private TextMeshProUGUI warningText; 
    private TreasureData currentFoundData;

    void Start() 
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        UpdateInventoryUI(); // 시작 시 초기 상태 표시
    }

    void Update() 
    {
        moneyText.text = $"Money: {GameManager.instance.money}";
        hpBar.fillAmount = (float)GameManager.instance.currentHealth / GameManager.instance.maxHealth;
        batteryBar.fillAmount = (float)GameManager.instance.currentBattery / GameManager.instance.maxBattery;
    }

    // 인벤토리 텍스트 및 버튼 상태를 새로고침하는 공용 함수 (추가됨)
    public void UpdateInventoryUI()
    {
        if (warningText == null) return;

        int currentCount = GameManager.instance.myInventory.Count;
        int maxCount = GameManager.instance.maxInventorySlots;

        if (currentCount < maxCount) 
        {
            keepButton.interactable = true;
            warningText.text = $"Storage: {currentCount}/{maxCount}";
        } 
        else 
        {
            keepButton.interactable = false;
            warningText.text = "<color=red>Bag is Full!</color>";
        }
    }

    public void ShowTreasureResult(TreasureData data) 
    {
        currentFoundData = data;
        resultNameText.text = data.treasureName;
        resultPriceText.text = $"Price: {data.price}G";
        resultImage.sprite = data.treasureIcon;

        UpdateInventoryUI(); // 결과창 뜰 때 갱신

        resultPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ClickKeep() 
    {
        if (currentFoundData != null) 
        {
            if (GameManager.instance.CanAddItem())
            {
                GameManager.instance.AddToInventory(currentFoundData);
                UpdateInventoryUI(); // 아이템 획득 후 갱신
            }
            else
            {
                Debug.Log("버튼 누를 때 이미 가방이 찼음");
            }
        }
        CloseResult();
    }

    public void ClickDiscard() 
    {
        CloseResult();
    }

    void CloseResult()
    {
        resultPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void SellAllTreasures() 
    {   
        var inventory = GameManager.instance.myInventory;
        if (inventory.Count <= 0) return;

        int totalProfit = 0;
        foreach (var item in inventory) totalProfit += item.price;

        GameManager.instance.AddMoney(totalProfit);
        inventory.Clear();

        UpdateInventoryUI(); // 판매 후 갱신

        InventoryManager invManager = Object.FindFirstObjectByType<InventoryManager>();
        if (invManager != null)
        {
            invManager.SendMessage("RefreshInventory", SendMessageOptions.DontRequireReceiver);
        }

        Debug.Log("보물을 모두 판매하여 가방이 비었습니다.");
    }
}
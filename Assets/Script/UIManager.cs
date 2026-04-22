using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    [Header("연출 설정")]
    [SerializeField] private Animator uiChestAnimator; // UI 내 보물상자 애니메이터
    [SerializeField] private GameObject rewardContent;  // 아이템 정보 + 버튼들을 묶은 부모 오브젝트
    [SerializeField] private float animationDuration = 1.0f; // 애니메이션이 재생되는 시간

    private TreasureData currentFoundData;
    private GameObject currentWorldTreasure;
    private TreasureManager treasureManager;

    void Awake()
    {
        treasureManager = Object.FindFirstObjectByType<TreasureManager>();
    }

    void Start() 
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        UpdateInventoryUI(); 
    }

    void Update() 
    {
        moneyText.text = GameManager.instance.money.ToString();
        hpBar.fillAmount = (float)GameManager.instance.currentHealth / GameManager.instance.maxHealth;
        batteryBar.fillAmount = (float)GameManager.instance.currentBattery / GameManager.instance.maxBattery;
    }

    public void UpdateInventoryUI()
    {
        if (warningText == null) return;

        int currentCount = GameManager.instance.myInventory.Count;
        int maxCount = GameManager.instance.maxInventorySlots;

        if (currentCount < maxCount) 
        {
            keepButton.interactable = true;
            warningText.text = $"{currentCount}/{maxCount}";
        } 
        else 
        {
            keepButton.interactable = false;
            warningText.text = "<color=red>가방이 가득 찼습니다!</color>";
        }
    }

    public void ShowTreasureResult(TreasureData data, GameObject worldObj) 
    {
        currentFoundData = data;
        currentWorldTreasure = worldObj;

        resultNameText.text = data.treasureName;
        resultPriceText.text = $"가격: {data.price}G";
        resultImage.sprite = data.treasureIcon;

        UpdateInventoryUI();

        // [수정] 새로운 보물을 열 때 상자 이미지를 다시 보이게 설정
        if (uiChestAnimator != null)
        {
            uiChestAnimator.gameObject.SetActive(true);
            uiChestAnimator.SetTrigger("UIOpen");
        }

        resultPanel.SetActive(true);
        rewardContent.SetActive(false); 

        Time.timeScale = 0f; 
        StartCoroutine(RevealRewardRoutine());
    }

    private IEnumerator RevealRewardRoutine()
    {
        yield return new WaitForSecondsRealtime(animationDuration);
        
        // [수정] 연출이 끝나면 상자 이미지를 비활성화하여 숨김
        if (uiChestAnimator != null)
        {
            uiChestAnimator.gameObject.SetActive(false);
        }

        rewardContent.SetActive(true);
    }

    public void ClickKeep() 
    {
        if (currentFoundData != null && GameManager.instance.CanAddItem())
        {
            GameManager.instance.AddToInventory(currentFoundData);
            UpdateInventoryUI();
            RemoveTreasureFromWorld();
        }
        CloseResult();
    }

    public void ClickDiscard() 
    {
        RemoveTreasureFromWorld();
        CloseResult();
    }

    private void RemoveTreasureFromWorld()
    {
        if (currentWorldTreasure != null)
        {
            if (treasureManager != null)
                treasureManager.RemoveTreasureFromList(currentWorldTreasure.transform);
            
            Destroy(currentWorldTreasure);
        }
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
        UpdateInventoryUI();

        Object.FindFirstObjectByType<InventoryManager>()?.SendMessage("RefreshInventory", SendMessageOptions.DontRequireReceiver);
    }
}
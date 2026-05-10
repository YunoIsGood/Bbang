using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Image hpBar;
    public Image batteryBar;

    [Header("게임 오버 설정")]
    [SerializeField] private GameObject sceneGameOverPanel; // 인스펙터에서 GameOverPanel 연결
    [SerializeField] private Button restartButton;        // [추가] GameOverPanel 안에 있는 다시시작 버튼 연결

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
    [SerializeField] GameObject questUi;

    private TreasureData currentFoundData;
    private GameObject currentWorldTreasure;
    private TreasureManager treasureManager;

   

    void Awake()
    {
        treasureManager = Object.FindFirstObjectByType<TreasureManager>();
    }

    void Start() 
    {
        // [핵심 수정] 씬이 시작될 때마다 살아있는 GameManager 인스턴스에 현재 씬의 UI들을 연결
        if (GameManager.instance != null)
        {
            // 1. 게임오버 패널 참조 갱신
            GameManager.instance.GameOverPanel = sceneGameOverPanel;

            // 2. 버튼 클릭 이벤트 코드로 연결 (Missing 방지)
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners(); // 기존에 잘못 연결된 리스너 제거
                restartButton.onClick.AddListener(GameManager.instance.GameOver); // 진짜 인스턴스의 함수 연결
            }
        }

        if (resultPanel != null) resultPanel.SetActive(false);
        UpdateInventoryUI();
        questUi.SetActive(false);
    }

    void Update() 
    {
        if (GameManager.instance == null) return;

        moneyText.text = GameManager.instance.money.ToString();
        hpBar.fillAmount = (float)GameManager.instance.currentHealth / GameManager.instance.maxHealth;
        batteryBar.fillAmount = (float)GameManager.instance.currentBattery / GameManager.instance.maxBattery;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            bool isActive = questUi.gameObject.activeSelf;
            questUi.SetActive(!isActive);

            Time.timeScale = isActive ? 1.0f : 0.0f;
        }
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
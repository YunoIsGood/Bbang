using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WonyoungUIManager : MonoBehaviour
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

    [Header("UI모음")]
    [SerializeField] private GameObject escUIPanel;
    [SerializeField] private GameObject settingUIPanel;


    void Awake()
    {
        treasureManager = Object.FindFirstObjectByType<TreasureManager>();
    }

    void Start()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        UpdateInventoryUI();
        escUIPanel.SetActive(false);

    }


    public void OnClickPlay()
    {
        escUIPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnClickSetting()
    {
        settingUIPanel.SetActive(true);


    }

    public void OnSettingExit()
    {
        settingUIPanel.SetActive(false);
    }
    public void GameOver()
    {

        Time.timeScale = 0;
    }


    void Update()
    {
        moneyText.text = GameManager.instance.money.ToString();
        hpBar.fillAmount = (float)GameManager.instance.currentHealth / GameManager.instance.maxHealth;
        batteryBar.fillAmount = (float)GameManager.instance.currentBattery / GameManager.instance.maxBattery;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)//세팅 UI 코드
        {

            bool isActive = escUIPanel.activeSelf;
            escUIPanel.SetActive(!isActive);

            Time.timeScale = isActive ? 1 : 0;

        }

    }

    public void UpdateInventoryUI()
    {
        if (warningText == null) return;

        Debug.Assert(GameManager.instance != null, "게임메니저를 구해오지 못함");
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

    public void ShowTreasureResult(TreasureData data, GameObject worldObj) //보물 획득 결과 보여주는 함수(보물 상호작용 시 TreasureManager에서 호출)
    {
        currentFoundData = data;//현재 발견한 보물 데이터 저장
        currentWorldTreasure = worldObj;//현재 발견한 보물의 월드 오브젝트 참조 저장

        resultNameText.text = data.treasureName;//보물 이름 텍스트 업데이트
        resultPriceText.text = $"가격: {data.price}G";//보물 가격 텍스트 업데이트
        resultImage.sprite = data.treasureIcon;//보물 아이콘 이미지 업데이트

        UpdateInventoryUI();//인벤토리 UI 업데이트하여 Keep 버튼 활성화 여부 결정

        if (uiChestAnimator != null)//UI 보물상자 애니메이터가 있으면 연출 시작
        {
            uiChestAnimator.gameObject.SetActive(true);//애니메이터 오브젝트 활성화하여 보이게 함
            uiChestAnimator.SetTrigger("UIOpen");//애니메이터에서 "UIOpen" 트리거를 설정하여 연출 재생 시작
        }

        resultPanel.SetActive(true);//보물 획득 결과 패널 활성화
        rewardContent.SetActive(false); //아이템 정보 + 버튼들을 묶은 부모 오브젝트는 처음에 비활성화하여 연출이 끝난 후에 보여지도록 함

        Time.timeScale = 0f; //게임 일시정지하여 연출이 끝날 때까지 시간 멈춤
        StartCoroutine(RevealRewardRoutine());//연출이 끝난 후 보물 정보와 버튼들을 보여주는 코루틴 시작
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

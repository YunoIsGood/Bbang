using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;//돈 텍스트
    public Image hpBar;//체력 바 이미지
    public Image batteryBar;//배터리 바 이미지

    [Header("게임 오버 설정")]
    [SerializeField] private GameObject sceneGameOverPanel;//씬 내 게임오버 패널
    [SerializeField] private Button restartButton;//게임오버 패널 내 재시작 버튼

    [Header("보물 획득 UI")]
    [SerializeField] private GameObject resultPanel;//보물 획득 결과 패널
    [SerializeField] private Image resultImage;//보물 아이콘 이미지
    [SerializeField] private TextMeshProUGUI resultNameText;//보물 이름 텍스트
    [SerializeField] private TextMeshProUGUI resultPriceText;//보물 가격 텍스트
    [SerializeField] private Button keepButton; //보물 획득 후 "Keep" 버튼
    [SerializeField] private TextMeshProUGUI warningText; //인벤토리 가득 찼을 때 경고 텍스트

    [Header("연출 설정")]
    [SerializeField] private Animator uiChestAnimator; // UI 내 보물상자 애니메이터
    [SerializeField] private GameObject rewardContent;  // 아이템 정보 + 버튼들을 묶은 부모 오브젝트
    [SerializeField] private float animationDuration = 1.0f; // 애니메이션이 재생되는 시간
    [SerializeField] GameObject questUi;//퀘스트 UI

    private TreasureData currentFoundData;//현재 발견한 보물 데이터
    private GameObject currentWorldTreasure;//현재 발견한 보물의 월드 오브젝트 참조
    private TreasureManager treasureManager;//보물 관리 매니저 참조

   

    void Awake()
    {
        treasureManager = Object.FindFirstObjectByType<TreasureManager>();//씬 내에서 TreasureManager 인스턴스 찾아서 참조 저장
    }

    void Start() 
    {
        if (GameManager.instance != null)//게임매니저 인스턴스 있으면
        {
            GameManager.instance.GameOverPanel = sceneGameOverPanel;//게임매니저에 게임오버 패널 참조 전달

            if (restartButton != null)//재시작 버튼 있으면
            {
                restartButton.onClick.RemoveAllListeners(); // 기존에 잘못 연결된 리스너 제거
                restartButton.onClick.AddListener(GameManager.instance.GameOver); // 게임매니저의 GameOver 메서드에 버튼 클릭 이벤트 연결
            }
        }

        if (resultPanel != null) resultPanel.SetActive(false);//보물 획득 결과 패널 초기에는 비활성화
        UpdateInventoryUI();//인벤토리 UI 초기 업데이트
        questUi.SetActive(false);//퀘스트 UI 초기에는 비활성화
    }

    void Update() 
    {
        if (GameManager.instance == null) return;//게임매니저 인스턴스 없으면 반환

        moneyText.text = GameManager.instance.money.ToString();//돈 텍스트 업데이트
        hpBar.fillAmount = (float)GameManager.instance.currentHealth / GameManager.instance.maxHealth;//체력 바 업데이트
        batteryBar.fillAmount = (float)GameManager.instance.currentBattery / GameManager.instance.maxBattery;//배터리 바 업데이트

        if (Keyboard.current.tabKey.wasPressedThisFrame)//탭 키를 눌렀을 때 퀘스트 UI 토글
        {
            bool isActive = questUi.gameObject.activeSelf;//현재 퀘스트 UI 활성화 상태 저장
            questUi.SetActive(!isActive);//퀘스트 UI 활성화 상태 반전

            Time.timeScale = isActive ? 1.0f : 0.0f;//퀘스트 UI 켜면 게임 일시정지, 끄면 게임 재개
        }
    }

    public void UpdateInventoryUI()//인벤토리 UI 업데이트 함수
    {
        if (warningText == null) return;//경고 텍스트가 없으면 반환

        int currentCount = GameManager.instance.myInventory.Count;//현재 인벤토리에 있는 아이템 수
        int maxCount = GameManager.instance.maxInventorySlots;//최대 인벤토리 슬롯 수

        if (currentCount < maxCount) //현재 아이템 수가 최대 슬롯 수보다 적으면
        {
            keepButton.interactable = true;//Keep 버튼 활성화
            warningText.text = $"{currentCount}/{maxCount}";//현재 아이템 수 / 최대 슬롯 수 형식으로 텍스트 업데이트
        } 
        else 
        {
            keepButton.interactable = false;//Keep 버튼 비활성화
            warningText.text = "<color=red>가방이 가득 찼습니다!</color>";//경고 텍스트 업데이트
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

    private IEnumerator RevealRewardRoutine()//상자 까는 애니메이션이 끝난 후 보물 정보와 버튼들을 보여주는 코루틴
    {
        yield return new WaitForSecondsRealtime(animationDuration);//애니메이션이 재생되는 시간만큼 실제 시간으로 대기
        
        if (uiChestAnimator != null)
        {
            uiChestAnimator.gameObject.SetActive(false);//애니메이터 오브젝트 비활성화하여 보이지 않게 함
        }

        rewardContent.SetActive(true);// 보물 정보와 버튼들있는 콘텐츠(패널) 활성화
    }

    public void ClickKeep() //Keep 버튼 클릭 시 인벤토리에 아이템 추가하고 결과 패널 닫는 함수
    {
        if (currentFoundData != null && GameManager.instance.CanAddItem())//현재 발견한 보물 데이터가 있고 인벤토리에 아이템 추가 가능한 경우
        {
            GameManager.instance.AddToInventory(currentFoundData);//게임매니저의 AddToInventory 함수 호출하여 현재 찾은 아이템 추가
            UpdateInventoryUI();//인벤토리 UI 업데이트하여 Keep 버튼 활성화 여부 결정
            RemoveTreasureFromWorld();//현재 보물을 월드에서 제거
        }
        CloseResult();//결과 패널 닫기
    }

    public void ClickDiscard() //Discard 버튼 클릭 시 결과 패널 닫는 함수
    {
        RemoveTreasureFromWorld();//현재 보물을 월드에서 제거
        CloseResult();//결과 패널 닫기
    }

    private void RemoveTreasureFromWorld()//현재 보물을 월드에서 제거하는 함수
    {
        if (currentWorldTreasure != null)
        {
            if (treasureManager != null)
                treasureManager.RemoveTreasureFromList(currentWorldTreasure.transform);//TreasureManager에서 현재 보물의 트랜스폼을 리스트에서 제거하여 더 이상 관리하지 않도록 함
            
            Destroy(currentWorldTreasure);//현재 보물의 월드 오브젝트 파괴하여 보이지 않게 함
        }
    }

    void CloseResult()
    {
        resultPanel.SetActive(false);//보물 획득 결과 패널 비활성화
        Time.timeScale = 1f; //게임 재개
    }

    public void SellAllTreasures() //인벤토리에 있는 모든 보물을 판매하는 함수
    {   
        var inventory = GameManager.instance.myInventory;//게임매니저의 인벤토리 리스트 참조
        if (inventory.Count <= 0) return;//인벤토리에 아이템이 없으면 반환
        int totalProfit = 0;//판매로 얻는 총 이익 계산
        foreach (var item in inventory) //인벤토리에 있는 각 아이템에 대해
        {
            totalProfit += item.price;//아이템 가격을 총 이익에 더함
        }

        GameManager.instance.AddMoney(totalProfit);//게임매니저의 AddMoney 함수 호출하여 총 이익만큼 돈 추가
        inventory.Clear();//인벤토리 리스트 초기화하여 모든 아이템 제거
        UpdateInventoryUI();//인벤토리 UI 업데이트

        Object.FindFirstObjectByType<InventoryManager>()?.SendMessage("RefreshInventory", SendMessageOptions.DontRequireReceiver);//인벤토리 매니저가 있으면 SendMessage로 RefreshInventory 함수 호출하여 인벤토리 UI 새로고침
    }

   
}
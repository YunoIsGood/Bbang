using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private RectTransform canvasRect;
    private Transform playerTransform;

    [Header("데이터")]
    public string treasureName; // 보물 이름
    public int price;          // 보물 가격

    [Header("타일맵 설정")]
    public Tilemap groundTilemap;    // 땅 타일맵 (파내야 할 대상)
    public Tilemap safeZoneTilemap;  // 안전 구역 타일맵 (파낼 수 없는 곳)

    [Header("UI 설정")]
    public GameObject uiPrefab;      // 보물 노출 시 머리 위에 띄울 버튼 UI 프리팹
    public float showDistance = 3.0f; // UI가 나타날 플레이어와의 최소 거리
    
    private GameObject myUI;         // 생성된 UI 인스턴스 저장용
    private RectTransform uiRect;    // 생성된 UI의 위치 조절용

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 하이Hierarchy에서 "Ground"라는 이름의 오브젝트를 찾아 타일맵 컴포넌트 할당
        GameObject groundObj = GameObject.Find("Ground");
        if (groundObj != null) groundTilemap = groundObj.GetComponent<Tilemap>();

        // 하이Hierarchy에서 "SafeZone"이라는 이름의 오브젝트를 찾아 타일맵 컴포넌트 할당
        GameObject safeZoneObj = GameObject.Find("SafeZone");
        if (safeZoneObj != null) safeZoneTilemap = safeZoneObj.GetComponent<Tilemap>();

        // 씬 내에 존재하는 첫 번째 캔버스를 찾아 캔버스 사각형(RectTransform) 참조
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();
        
        // "Player" 태그를 가진 오브젝트를 찾아 트랜스폼 참조
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    // 외부(예: 보물 생성 매니저)에서 보물 정보를 주입할 때 사용하는 함수
    public void SetTreasure(TreasureData data)
    {
        if (data == null) return;
        treasureName = data.name;
        price = data.price;
        if (spriteRenderer != null) spriteRenderer.sprite = data.sprite;
        gameObject.name = data.name;
    }

    void Update()
    {
        // 필수 참조 요소 중 하나라도 없으면 에러 방지를 위해 업데이트 중단
        if (groundTilemap == null || safeZoneTilemap == null || Camera.main == null || 
            uiPrefab == null || canvasRect == null || playerTransform == null) 
            return;

        // 보물의 현재 월드 위치를 타일맵의 셀 좌표로 변환
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);

        // 1. 타일 존재 여부 체크 (땅 타일과 세이프존 타일이 모두 없어야 '드러난' 상태)
        bool hasGround = groundTilemap.HasTile(cellPos);
        bool hasSafeZone = safeZoneTilemap.HasTile(cellPos);

        // 땅도 없고 세이프존 타일도 없을 때만 보물이 밖으로 노출된 것으로 판정
        bool isExposed = !hasGround && !hasSafeZone;

        // 2. 플레이어와 보물 사이의 거리 계산
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 3. 조건 충족 시(노출됨 + 사거리 안) UI 표시
        if (isExposed && distance <= showDistance)
        {
            // UI가 아직 생성되지 않았다면 프리팹을 캔버스 자식으로 생성
            if (myUI == null)
            {
                myUI = Instantiate(uiPrefab, canvasRect);
                uiRect = myUI.GetComponent<RectTransform>();
            }
            myUI.SetActive(true); // UI 활성화

            // --- 월드 좌표를 UI 좌표로 변환하는 과정 ---
            // 보물의 약간 위쪽(0.8f) 지점을 UI 표시 지점으로 설정
            Vector3 worldPos = transform.position + Vector3.up * 0.8f;
            // 카메라를 통해 월드 좌표를 화면 스크린 좌표로 변환
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            // 스크린 좌표를 캔버스 내의 로컬 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPoint, Camera.main, out Vector2 localPoint);

            // 변환된 좌표를 UI의 앵커 위치에 적용
            uiRect.anchoredPosition = localPoint;
        }
        else if (myUI != null)
        {
            // 범위를 벗어나거나 다시 묻히면 UI 비활성화
            myUI.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // 보물 오브젝트가 파괴될 때 생성했던 UI도 함께 파괴 (메모리 관리)
        if (myUI != null) Destroy(myUI);
    }
}
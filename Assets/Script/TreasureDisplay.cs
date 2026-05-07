using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private RectTransform canvasRect;
    private Transform playerTransform;
    private UIManager uiManager;

    [Header("보관 데이터")]
    public TreasureData myData; 

    [Header("타일맵 설정")]
    public Tilemap groundTilemap;
    public Tilemap safeZoneTilemap;

    [Header("UI 설정")]
    public GameObject uiPrefab;
    public float showDistance = 3.0f;
    
    private GameObject myUI;
    private RectTransform uiRect;
    public Sprite chestSprite;

    private bool isInteracted = false; // 중복 상호작용 방지

    void Awake()
    {
        // 최적화: Find 작업은 Awake에서 한 번만 수행
        GameObject spawner = GameObject.Find("TreasureSpawner") ?? new GameObject("TreasureSpawner");
        transform.SetParent(spawner.transform);

        spriteRenderer = GetComponent<SpriteRenderer>();
        groundTilemap = GameObject.Find("Ground")?.GetComponent<Tilemap>();
        safeZoneTilemap = GameObject.Find("SafeZone")?.GetComponent<Tilemap>();
        
        uiManager = Object.FindFirstObjectByType<UIManager>();
        
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    public void SetTreasure(TreasureData data)
    {
        myData = data;
        if (spriteRenderer != null) spriteRenderer.sprite = chestSprite;
        gameObject.name = data.treasureName;
    }

    void Update()
    {
        if (groundTilemap == null || playerTransform == null || Camera.main == null || isInteracted) return;

        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);
        bool isExposed = !groundTilemap.HasTile(cellPos) && (safeZoneTilemap == null || !safeZoneTilemap.HasTile(cellPos));
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (isExposed && distance <= showDistance)
        {
            HandleInteractionUI(true);

            // E 키 입력 시 상호작용
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
        else
        {
            HandleInteractionUI(false);
        }
    }

    private void HandleInteractionUI(bool show)
    {
        if (show)
        {
            if (myUI == null && uiPrefab != null)
            {
                myUI = Instantiate(uiPrefab, canvasRect);
                uiRect = myUI.GetComponent<RectTransform>();
            }
            
            if (myUI != null)
            {
                myUI.SetActive(true);
                UpdateUIPosition();
            }
        }
        else if (myUI != null)
        {
            myUI.SetActive(false);
        }
    }

    private void UpdateUIPosition()
    {
        Vector3 worldPos = transform.position + Vector3.up * 0.8f;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out Vector2 localPoint);
        uiRect.anchoredPosition = localPoint;
    }

    private void Interact()
    {
        isInteracted = true;
        if (myUI != null) myUI.SetActive(false);

        if (uiManager != null)
        {
            uiManager.ShowTreasureResult(myData, this.gameObject);
        }
    }

    void OnDestroy() 
    {
        if (myUI != null) Destroy(myUI);
    }
}
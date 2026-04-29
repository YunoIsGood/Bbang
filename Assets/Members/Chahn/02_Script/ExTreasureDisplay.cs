using UnityEngine;
using UnityEngine.Tilemaps;

public class ExTreasureDisplay : MonoBehaviour
{


public class TreasureDisplay : MonoBehaviour//보물 프리팹에 넣는 스크립트
{
    private SpriteRenderer spriteRenderer;//보물 이미지 보여줄 스프라이트 렌더러
    private RectTransform canvasRect;//캔버스 위치
    private Transform playerTransform;//플레이어 위치

    [Header("보관 데이터")]
    public TreasureData myData; //보물 데이터

    [Header("타일맵 설정")]
    public Tilemap groundTilemap;//맵 타일맵
    public Tilemap safeZoneTilemap;//세이프존 타일맵

    [Header("UI 설정")]
    public GameObject uiPrefab;//보물 근처 가면 뜨는 버튼 프리팹
    public float showDistance = 3.0f;//버튼 뜨게 할 범위 

    private GameObject myUI;
    private RectTransform uiRect;

    public Sprite chestSprite;//보물상자 이미지

    void Awake()
    {
        //Awake시에 다 가져오기
        GameObject spawner = GameObject.Find("TreasureSpawner");//보물 부모로 설정할 TreasureSpawner
        if (spawner == null)
        {
            spawner = new GameObject("TreasureSpawner");
        }
        transform.SetParent(spawner.transform);

        spriteRenderer = GetComponent<SpriteRenderer>();//스프라이트 렌더러
        groundTilemap = GameObject.Find("Ground")?.GetComponent<Tilemap>();//맵 타일맵
        safeZoneTilemap = GameObject.Find("SafeZone")?.GetComponent<Tilemap>();//세이프존 타일맵

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();//캔버스
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();

        GameObject player = GameObject.FindWithTag("Player");//플레이어
        if (player != null) playerTransform = player.transform;
    }

    public void SetTreasure(TreasureData data)
    {
        myData = data; //보물상자 안의 데이터를 보물 데이터로 설정
        if (spriteRenderer != null) //보물상자 이미지 없으면
        {
            spriteRenderer.sprite = chestSprite; //보물상자 이미지 넣기
        }
        gameObject.name = data.name;
    }

    void Update()
    {
        if (groundTilemap == null || playerTransform == null || Camera.main == null) return;// 맵 타일맵, 플레이어 위치, 카메라 설정 안했으면 반환

        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);
        bool isExposed = !groundTilemap.HasTile(cellPos) && (safeZoneTilemap == null || !safeZoneTilemap.HasTile(cellPos));
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (isExposed && distance <= showDistance && uiPrefab != null)//땅이 파져있고 감지 범위 안이라면
        {
            if (myUI == null)
            {
                myUI = Instantiate(uiPrefab, canvasRect);
                uiRect = myUI.GetComponent<RectTransform>();
            }
            myUI.SetActive(true);

            Vector3 worldPos = transform.position + Vector3.up * 0.8f;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out Vector2 localPoint);
            uiRect.anchoredPosition = localPoint;
        }
        else if (myUI != null)
        {
            myUI.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (myUI != null)
        {
            Destroy(myUI);
        }
    }
  }
}

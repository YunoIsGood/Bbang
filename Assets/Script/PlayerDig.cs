using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDig : MonoBehaviour
{
    [Header("참조 설정")]
    public Tilemap groundTilemap;     // 파낼 수 있는 땅 타일맵
    public Tilemap safeZoneTilemap;   // 파낼 수 없는 안전 구역 타일맵
    public LayerMask treasureLayer;   // 보물 오브젝트를 식별하기 위한 레이어

    [Header("설정값")]
    public float digRadius = 1.5f;     // 땅을 파는 범위 (반지름)
    public float collectRadius = 1.2f; // 보물을 수집하는 범위 (반지름)

    void Update()
    {
        // 게임 매니저가 있고, 현재 플레이어가 안전 구역에 있다면 아래 로직(파기/수집)을 실행하지 않음
        if (GameManager.instance != null && GameManager.instance.isSafeZone) return;

        // C 키를 누르면 땅 파기 실행
        if (Input.GetKeyDown(KeyCode.C)) DigGround();
        
        // E 키를 누르면 보물 수집 시도
        if (Input.GetKeyDown(KeyCode.E)) TryCollectTreasure();
    }

    // 주변의 땅 타일을 제거하는 함수
    void DigGround()
    {
        if (groundTilemap == null) return;

        // 플레이어의 월드 좌표를 타일맵의 셀 좌표(인덱스)로 변환
        Vector3Int playerCellPos = groundTilemap.WorldToCell(transform.position);
        
        // 반지름을 기준으로 검사할 타일의 칸 수 계산
        int range = Mathf.CeilToInt(digRadius);

        // 플레이어 중심 주변 (X, Y) 범위를 루프 돌며 검사
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                Vector3Int tilePos = new Vector3Int(playerCellPos.x + x, playerCellPos.y + y, 0);
                
                // 해당 타일의 중심점과 플레이어 사이의 실제 거리가 digRadius 이내인지 확인 (원형 파기)
                if (Vector2.Distance(transform.position, groundTilemap.GetCellCenterWorld(tilePos)) <= digRadius)
                {
                    // 만약 해당 위치가 safeZoneTilemap에 타일이 있는 곳이라면 파지 않고 건너뜀
                    if (safeZoneTilemap != null && safeZoneTilemap.HasTile(tilePos)) continue;

                    // 해당 위치의 타일을 제거 (null로 설정)
                    groundTilemap.SetTile(tilePos, null);
                }
            }
        }
    }

    // 드러난 보물을 수집하는 함수
    void TryCollectTreasure()
    {
        // 플레이어 위치 중심 collectRadius 범위 내에 있는 treasureLayer 레이어의 콜라이더들을 모두 찾음
        Collider2D[] treasures = Physics2D.OverlapCircleAll(transform.position, collectRadius, treasureLayer);
        
        foreach (Collider2D t in treasures) {
            // 보물이 위치한 곳의 타일 좌표를 계산
            Vector3Int cellPos = groundTilemap.WorldToCell(t.transform.position);
            
            // 중요: 해당 위치의 땅 타일이 제거된 상태(HasTile이 false)일 때만 수집 가능
            if (!groundTilemap.HasTile(cellPos)) {
                Debug.Log("보물을 수집했습니다.");
                Destroy(t.gameObject); // 보물 오브젝트 제거
            }
        }
    }
}
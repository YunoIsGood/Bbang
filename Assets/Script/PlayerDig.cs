using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDig : MonoBehaviour
{
    [Header("참조 설정")]
    public Tilemap groundTilemap;//맵 타일맵
    public Tilemap safeZoneTilemap;//세이프존 타일맵
    public LayerMask treasureLayer;//보물 레이어

    [Header("설정값")]
    public float digRadius = 1.5f;//땅 파지는 범위
    public float collectRadius = 1.2f;//보물 먹어지는 범위

    void Update()
    {
        // 게임 매니저가 있고, 현재 플레이어가 안전 구역에 있다면 아래 로직(파기/수집)을 실행하지 않음
        if (GameManager.instance != null && GameManager.instance.isSafeZone) return;

        // C 키를 누르면 땅 파기 실행
        if (Input.GetKeyDown(KeyCode.Space)) DigGround();
        
        // E 키를 누르면 보물 수집 시도
        if (Input.GetKeyDown(KeyCode.E)) TryCollectTreasure();
    
    if (Time.timeScale == 0) return;//시간 흐르는 값(timeScale)이 0이면 반환

    if (Input.GetKeyDown(KeyCode.C)) DigGround();//C 누르면 땅파는 함수 호출
    if (Input.GetKeyDown(KeyCode.E)) TryCollectTreasure();//E 누르면 보물먹는 함수 호출
    }



    //타일맵 파는 코드 (몰라도됨)
    void DigGround()
    {
        if (groundTilemap == null) return;//맵 타일맵 설정 안했으면 반환
        if(GameManager.instance.currentBattery<=0) return;//배터리가 0 이하면 반환

        GameManager.instance.UseBattery(2.5f);//땅 팔때마다 일정한 값만큼 배터리 소모
        Vector3Int playerCellPos = groundTilemap.WorldToCell(transform.position);//플레이어의 현재 월드 좌표를 타일맵의 칸 번호로 변환
        int range = Mathf.CeilToInt(digRadius);//땅 파지는 범위를 int 형식으로 만듬

        for (int x = -range; x <= range; x++) //땅 파지는 x범위 만큼 반복
        {
            for (int y = -range; y <= range; y++) //땅 파지는 y범위 만큼 반복
            {
                Vector3Int tilePos = new Vector3Int(playerCellPos.x + x, playerCellPos.y + y, 0); // 내 현재 칸 위치에 x, y만큼 더해서 주변 타일의 좌표를 계산
                if (Vector2.Distance(transform.position, groundTilemap.GetCellCenterWorld(tilePos)) <= digRadius)//내 위치 기준 범위 안에 들어오는 타일맵이면
                {
                    if (safeZoneTilemap != null && safeZoneTilemap.HasTile(tilePos)) continue;//세이프존이면 그 타일은 건너뜀
                    groundTilemap.SetTile(tilePos, null);//위치에 있는 타일맵 삭제
                }
            }
        }
    }
    //보물 먹는 코드 (몰라도됨22)
   void TryCollectTreasure()
    {
    Collider2D[] treasures = Physics2D.OverlapCircleAll(transform.position, collectRadius, treasureLayer);//내 주변에 보물 레이어가 
    if (treasures.Length <= 0) return;//보물 개수가 0이면 반환
    
    Collider2D closestTreasure = null;//가장 가까운 보물을 담을 콜라이더
    float minDistance = Mathf.Infinity; 
    
    //여기부터는 모르겠다
    foreach (Collider2D t in treasures)
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(t.transform.position);
        if (groundTilemap.HasTile(cellPos)) continue; 

        float dist = Vector2.Distance(transform.position, t.transform.position);//거리 계산
        if (dist < minDistance)
        {
            minDistance = dist;
            closestTreasure = t;
        }
    }

    
    if (closestTreasure != null)
    {
        TreasureDisplay display = closestTreasure.GetComponent<TreasureDisplay>();
        if (display != null && display.myData != null)
        {
            
            Object.FindFirstObjectByType<UIManager>().ShowTreasureResult(display.myData);
            
            
            Destroy(closestTreasure.gameObject); 
            return; 
        }
    }
    }
}
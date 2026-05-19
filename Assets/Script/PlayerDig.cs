using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDig11 : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap safeZoneTilemap;
    public LayerMask treasureLayer;
    public float digRadius = 1.5f;
    public float collectRadius = 1.2f;
    public ParticleSystem digEffect;

    void Update()
    {

    
    if (Time.timeScale == 0) return;//시간 흐르는 값(timeScale)이 0이면 반환

        // UI가 떠서 시간이 멈춘 상태라면 입력 무시
        if (Time.timeScale == 0) return;


    if (Input.GetKeyDown(KeyCode.C)) DigGround();//C 누르면 땅파는 함수 호출
    if (Input.GetKeyDown(KeyCode.E)) TryCollectTreasure();//E 누르면 보물먹는 함수 호출
    }

    void DigGround()
    {
        if (groundTilemap == null) return;
        if (GameManager.instance.currentBattery <= 0) return;

        Vector3Int playerCellPos = groundTilemap.WorldToCell(transform.position);
        int range = Mathf.CeilToInt(digRadius);
        
        bool hasDigSomething = false; // 실제로 타일을 팠는지 체크하는 변수

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int tilePos = new Vector3Int(playerCellPos.x + x, playerCellPos.y + y, 0);
                
                // 타일이 있는지 먼저 확인
                if (groundTilemap.HasTile(tilePos)) 
                {
                    if (Vector2.Distance(transform.position, groundTilemap.GetCellCenterWorld(tilePos)) <= digRadius)
                    {
                        if (safeZoneTilemap != null && safeZoneTilemap.HasTile(tilePos)) continue;

                        groundTilemap.SetTile(tilePos, null);
                        hasDigSomething = true; // 타일을 하나라도 팠다면 true
                    }
                }
            }
        }

        // 실제로 타일을 팠을 때만 배터리 소모 및 파티클 생성
        if (hasDigSomething)
        {
            GameManager.instance.UseBattery(2.5f);
            
            if (digEffect != null)
            {
                ParticleSystem effectInstance = Instantiate(digEffect, transform.position, Quaternion.identity);
                Destroy(effectInstance.gameObject, effectInstance.main.duration + 0.5f);
            }
        }
    }

    void TryCollectTreasure()
    {
        Collider2D[] treasures = Physics2D.OverlapCircleAll(transform.position, collectRadius, treasureLayer);
        if (treasures.Length <= 0) return;

        Collider2D closestTreasure = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D t in treasures)
        {
            Vector3Int cellPos = groundTilemap.WorldToCell(t.transform.position);
            // 땅 속에 있는 보물은 먹을 수 없음
            if (groundTilemap.HasTile(cellPos)) continue;

            float dist = Vector2.Distance(transform.position, t.transform.position);
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
                // 최적화: UIManager를 직접 찾아 ShowTreasureResult 호출
                UIManager uiMgr = Object.FindFirstObjectByType<UIManager>();
                if (uiMgr != null)
                {
                    // [수정 핵심] 함수 인자를 (데이터, 게임오브젝트)로 전달합니다.
                    uiMgr.ShowTreasureResult(display.myData, closestTreasure.gameObject);
                }
                
                // [주의] 여기서 Destroy를 하면 안 됩니다! 
                // 애니메이션이 끝나고 저장/버리기 버튼을 눌렀을 때 UIManager에서 파괴해야 합니다.
            }
        }
    }
}

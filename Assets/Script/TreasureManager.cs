using UnityEngine;
using System.Collections.Generic;

public class TreasureManager : MonoBehaviour
{
    [Header("구역(스테이지) 설정")]
    public List<ZoneData> zones = new List<ZoneData>();
    
    [Header("프리팹")]
    public GameObject treasurePrefab;
    
    // 금속탐지기가 실시간으로 참조할 리스트
    public List<Transform> activeTreasures = new List<Transform>();

    void Start() 
    { 
        SpawnEverything(); 
    }

    public void SpawnEverything()
    {
        if (zones.Count == 0 || treasurePrefab == null) return;

        foreach (var zone in zones)
        {
            // 1. 보물 스폰
            for (int i = 0; i < zone.treasureCount; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), Random.Range(zone.minY, zone.maxY), 0);
                GameObject newTreasure = Instantiate(treasurePrefab, spawnPos, Quaternion.identity);

                // 구역 리스트 중 랜덤하게 보물 데이터 할당
                if (zone.zoneTreasures.Count > 0)
                {
                    TreasureData randomData = zone.zoneTreasures[Random.Range(0, zone.zoneTreasures.Count)];
                    newTreasure.GetComponent<TreasureDisplay>().SetTreasure(randomData);
                }
                
                activeTreasures.Add(newTreasure.transform);
            }

            // 2. 몬스터 스폰
            for (int i = 0; i < zone.monsterCount; i++)
            {
                if (zone.zoneMonsters.Count == 0) break;
                Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), Random.Range(zone.minY, zone.maxY), 0);
                Instantiate(zone.zoneMonsters[Random.Range(0, zone.zoneMonsters.Count)], spawnPos, Quaternion.identity);
            }
        }
    }

    // 보물을 먹었을 때 리스트에서 제거해주는 함수
    public void RemoveTreasureFromList(Transform t)
    {
        if (activeTreasures.Contains(t))
        {
            activeTreasures.Remove(t);
        }
    }
}
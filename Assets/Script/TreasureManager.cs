using UnityEngine;
using System.Collections.Generic;

public class TreasureManager : MonoBehaviour
{
    [Header("구역(스테이지) 설정")]
    public List<ZoneData> zones = new List<ZoneData>();//구역(스테이지) 데이터 리스트
    
    [Header("프리팹")]
    public GameObject treasurePrefab;//보물 프리팹
    
    [Header("활성화된 보물 리스트")]
    public List<Transform> activeTreasures = new List<Transform>();//현재 활성화된 보물들의 트랜스폼 리스트

    void Start() 
    { 
        SpawnEverything(); //게임 시작 시 보물과 몬스터 스폰
    }

    public void SpawnEverything()//시작 시 보물과 몬스터 스폰하는 함수
    {
        if (zones.Count == 0 || treasurePrefab == null) return;//구역 데이터나 보물 프리팹이 없으면 반환

        foreach (var zone in zones)//각 구역마다
        {
            for (int i = 0; i < zone.treasureCount; i++)//보물 개수만큼
            {
                Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), Random.Range(zone.minY, zone.maxY), 0);//랜덤한 위치에 보물 스폰
                GameObject newTreasure = Instantiate(treasurePrefab, spawnPos, Quaternion.identity);//보물 프리팹으로 보물 오브젝트 생성

                if (zone.zoneTreasures.Count > 0)//구역에 보물 데이터가 있으면
                {
                    TreasureData randomData = zone.zoneTreasures[Random.Range(0, zone.zoneTreasures.Count)];//랜덤한 보물 데이터 선택
                    newTreasure.GetComponent<TreasureDisplay>().SetTreasure(randomData);//랜덤 위치에 생성한 보물 오브젝트에 보물 데이터 주입
                }
                
                activeTreasures.Add(newTreasure.transform);//활성화된 보물 리스트에 새로 생성한 보물의 트랜스폼 추가
            }

            for (int i = 0; i < zone.monsterCount; i++)//몬스터 개수만큼
            {
                if (zone.zoneMonsters.Count == 0) break;//구역에 몬스터 프리팹이 없으면 반복문 종료
                Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), Random.Range(zone.minY, zone.maxY), 0);//랜덤한 위치에 몬스터 스폰
                Instantiate(zone.zoneMonsters[Random.Range(0, zone.zoneMonsters.Count)], spawnPos, Quaternion.identity);//구역에 몬스터 프리팹이 있으면 랜덤한 몬스터 프리팹으로 몬스터 오브젝트 생성
            }
        }
    }

    public void RemoveTreasureFromList(Transform t)//보물이 획득되거나 파괴될 때 해당 보물의 트랜스폼을 활성화된 보물 리스트에서 제거하는 함수
    {
        if (activeTreasures.Contains(t))//보물 리스트에 해당 트랜스폼이 있으면
        {
            activeTreasures.Remove(t);//활성화된 보물 리스트에서 제거
        }
    }
}
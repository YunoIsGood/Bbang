using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Zone Data", menuName = "ScriptableObjects/ZoneData")]
public class ZoneData : ScriptableObject
{
    public string zoneName;
    public float minY;
    public float maxY;
    
    [Header("스폰 설정")]
    public List<TreasureData> zoneTreasures; // 이 구역 전용 보물
    public List<GameObject> zoneMonsters;    // 이 구역 전용 몬스터
    public int treasureCount = 10;
    public int monsterCount = 5;
}
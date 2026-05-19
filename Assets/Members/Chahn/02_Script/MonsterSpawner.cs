using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct TreasurePlan
{
    public GameObject treasurePrefab;
    public int spawnChance;
}
public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]private ZoneData zoneData;
    public List<TreasurePlan> treasureSpawnPlan = new List<TreasurePlan>();
    public GameObject[] Treasure;
    public int spawnCount;
    public Vector2 spawnChoice;

    void Start()
    {
        SetTreasureList();
    }
   void SetTreasureList()
    {
        GameObject[] allTreasure = Resources.LoadAll<GameObject>("Treasure");
        List<GameObject> filteredTreasure = new List<GameObject>();
        foreach (GameObject prefab in allTreasure)
        {
             if (prefab.CompareTag(zoneData.zoneName))
                {
                filteredTreasure.Add(prefab);
                }
        }
        Treasure = filteredTreasure.ToArray();
    }
    void TreasureRandom()
    {
        spawnChoice.x = Random.Range(-19, 19);
        spawnChoice.y = Random.Range(10, 70);
        
    } 
    
}

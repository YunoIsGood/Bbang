using UnityEngine;

using System.Collections.Generic;

  

[System.Serializable]

public class TreasureData

{

    public string name;      // 보물 이름

    public Sprite sprite;    // 보물 이미지

    public int price;        // 보물 가격

    public int chance;       // 보물 확률

}

  

public class TreasureManager : MonoBehaviour

{

    [Header("보물 리스트")]

    public List<TreasureData> treasureList = new List<TreasureData>();

    [Header("스폰 설정")]

    public GameObject treasurePrefab;

    public int spawnCount = 20;      

  

    [Header("스폰 범위")]

    public float minX = -15f;

    public float maxX = 15f;

    public float minY = -15f;

    public float maxY = 15f;

  

    void Start()

    {

        SpawnTreasures();

    }

  

    public void SpawnTreasures()

    {

        if (treasureList.Count == 0 || treasurePrefab == null) return;

  

        int totalChance = 0;

        foreach (var treasure in treasureList) totalChance += treasure.chance;

  

        for (int i = 0; i < spawnCount; i++)

        {

            int randomValue = Random.Range(0, totalChance);

            TreasureData selectedTreasure = null;

            int currentSum = 0;

  

            foreach (var treasure in treasureList)

            {

                currentSum += treasure.chance;

                if (randomValue < currentSum)

                {

                    selectedTreasure = treasure;

                    break;

                }

            }

  

            if (selectedTreasure != null)

            {

                Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);

                GameObject newTreasure = Instantiate(treasurePrefab, spawnPos, Quaternion.identity);

  

                // 생성된 보물에 데이터 꽂아주기

                TreasureDisplay display = newTreasure.GetComponent<TreasureDisplay>();

                if (display != null)

                {

                    display.SetTreasure(selectedTreasure);

                }

            }

        }

    }

}
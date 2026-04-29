using UnityEngine;
//보물 데이터 (ScriptableObject <-- 한번쯤 검색해보기)
[CreateAssetMenu(fileName = "New Treasure", menuName = "ScriptableObjects/TreasureData")]
public class TreasureData : ScriptableObject
{
    public string treasureName; // 보물 이름
    public Sprite treasureIcon; // 보물 이미지
    public int price;           // 보물 가격
    [Range(0, 100)]             // 인스펙터에서 왔다갔다로 조정 가능하게
    public int chance;          // 스폰 확률 (0~100)
}
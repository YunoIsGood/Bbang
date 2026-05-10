using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Stat Settings")]
    public string monsterName;
    public int maxHp;
    public float speed = 5f;
    public int damage;
    public float attackCooldown = 1.0f;

    [Header("Visual Settings")]
    public GameObject modelPrefab; // 필요한 경우 외형도 SO에서 관리
}
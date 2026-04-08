using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TreasureDetector : MonoBehaviour
{
    [Header("보물 오브젝트")]
    public GameObject treasure;

    [Header("빛 효과 오브젝트")]
    public GameObject treasureGlow;

    [Header("탐지 거리")]
    public float detectDistance = 3f;

    [Header("탐지 배터리 소모량")]
    public float scanBatteryCost = 5f;

    [Header("빛이 보이는 시간")]
    public float glowDuration = 2f;

    private Coroutine glowCoroutine;

    void Start()
    {
        if (treasureGlow != null)
        {
            treasureGlow.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            CheckTreasure();
        }
    }

    void CheckTreasure()
    {
        if (GameManager.instance == null)
        {
            Debug.Log("GameManager가 없습니다.");
            return;
        }

        if (GameManager.instance.currentBattery <= 0)
        {
            Debug.Log("배터리가 없습니다.");
            return;
        }

        if (treasure == null)
        {
            Debug.Log("보물이 지정되지 않았습니다.");
            return;
        }

        GameManager.instance.UseBattery(scanBatteryCost);

        float distance = Vector2.Distance(transform.position, treasure.transform.position);

        if (distance <= detectDistance)
        {
            Debug.Log("보물 발견!");
            ShowGlow();
        }
    }

    void ShowGlow()
    {
        if (treasureGlow == null)
        {
            Debug.Log("빛 효과 오브젝트가 지정되지 않았습니다.");
            return;
        }

        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }

        glowCoroutine = StartCoroutine(GlowRoutine());
    }

    IEnumerator GlowRoutine()
    {
        treasureGlow.SetActive(true);
        yield return new WaitForSeconds(glowDuration);
        treasureGlow.SetActive(false);
    }
}
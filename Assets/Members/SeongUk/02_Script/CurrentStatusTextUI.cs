using TMPro;
using UnityEngine;

public class CurrentStatusTextUI : MonoBehaviour
{
    [Header("Current Text")]
    [SerializeField] private TextMeshProUGUI CurrentGoldText;
    [SerializeField] private TextMeshProUGUI CurrentInventoryText;

    private void Start()
    {
        UpdateCurrentStatusText();
    }

    private void Update()
    {
        UpdateCurrentStatusText();
    }

    public void UpdateCurrentStatusText()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        // D3는 숫자를 세 자리로 보여줌 유노바보. 예: 5 -> 005, 50 -> 050, 100 -> 100
        if (CurrentGoldText != null)
        {
            CurrentGoldText.text = GameManager.instance.money.ToString("D3");
        }

        // 현재 인벤토리에 들어있는 아이템 개수를 세 자리 숫자로 보여줍니다람쥐.
        if (CurrentInventoryText != null)
        {
            CurrentInventoryText.text = GameManager.instance.myInventory.Count.ToString("D3");
        }
    }
}

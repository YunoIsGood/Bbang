using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPromptText;
    [SerializeField] private MonoBehaviour playerMovement; // 상점이 열려 있을 때 플레이어 이동을 막기 위한 참조입니다.
    private bool isPlayerInShopRange = false;
    private bool isShopOpen = false;

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button shopButton;
    [SerializeField] private TextMeshProUGUI messageText;
    private Coroutine messageCoroutine;

    [Header("업그레이드 수치 및 UI")]
    int lightUpgradeLevel = 0;
    int lightUpgradeGold = 10;
    [SerializeField] private TextMeshProUGUI lightUpgradeText;

    int speedUpgradeLevel = 0;
    int speedUpgradeGold = 10;
    [SerializeField] private TextMeshProUGUI speedUpgradeText;

    int attackUpgradeLevel = 0;
    int attackUpgradeGold = 10;
    [SerializeField] private TextMeshProUGUI attackUpgradeText;

    int hpUpgradeLevel = 0;
    int hpUpgradeGold = 50;
    [SerializeField] private TextMeshProUGUI hpUpgradeText;

    int batteryUpgradeLevel = 0;
    int batteryUpgradeGold = 50;
    [SerializeField] private TextMeshProUGUI batteryUpgradeText;

    int bagUpgradeLevel = 0;
    int bagUpgradeGold = 80;
    [SerializeField] private TextMeshProUGUI bagUpgradeText;

    void Start()
    {
        ClearMessageText();
        UpdateAllUI();
        SetShopPanelActive(false);
    }

    private void Update()
    {
        if (isPlayerInShopRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isShopOpen)
            {
                CloseShop();
            }
            else
            {
                OpenShop();
            }
        }

        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        SetShopPanelActive(true);
        isShopOpen = true;
        SetShopPromptActive(false);
        SetPlayerMovementEnabled(false);
        UpdateAllUI();
    }

    public void CloseShop()
    {
        SetShopPanelActive(false);
        isShopOpen = false;

        if (isPlayerInShopRange)
        {
            SetShopPromptActive(true);
        }

        SetPlayerMovementEnabled(true);
    }

    private void SetShopPromptActive(bool active)
    {
        if (shopPromptText != null)
        {
            shopPromptText.SetActive(active);
        }
    }

    private void SetShopPanelActive(bool active)
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(active);
        }
    }

    private void SetPlayerMovementEnabled(bool enabled)
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = enabled;
        }
    }

    private void UpdateAllUI()
    {
        if (GameManager.instance == null) return;

        if (lightUpgradeText != null)
        {
            lightUpgradeText.text = $": {GameManager.instance.baseInnerRadius:F1}\n  레벨: {lightUpgradeLevel}\n       X {lightUpgradeGold}";
        }

        if (speedUpgradeText != null)
        {
            speedUpgradeText.text = $"스피드 업\n  레벨: {speedUpgradeLevel}\n       X {speedUpgradeGold}";
        }

        if (attackUpgradeText != null)
        {
            attackUpgradeText.text = $"공격력 업\n  레벨: {attackUpgradeLevel}\n       X {attackUpgradeGold}";
        }

        if (hpUpgradeText != null)
        {
            hpUpgradeText.text = $": {GameManager.instance.maxHealth}\n  레벨: {hpUpgradeLevel}\n       X {hpUpgradeGold}";
        }

        if (batteryUpgradeText != null)
        {
            batteryUpgradeText.text = $": {GameManager.instance.maxBattery}\n  레벨: {batteryUpgradeLevel}\n       X {batteryUpgradeGold}";
        }

        if (bagUpgradeText != null)
        {
            bagUpgradeText.text = $": {GameManager.instance.maxInventorySlots:F1}\n  레벨: {bagUpgradeLevel}\n       X {bagUpgradeGold}";
        }
    }

    public void BagUpgrade()
    {
        if (!TrySpendGold(bagUpgradeGold)) return;

        ApplyBagUpgrade(1);
        UpdateAllUI();
        UpdateInventoryUIIfNeeded();
    }

    public void HPUpgrade()
    {
        if (!TrySpendGold(hpUpgradeGold)) return;

        ApplyHPUpgrade(1);
        UpdateAllUI();
    }

    public void BatteryUpgrade()
    {
        if (!TrySpendGold(batteryUpgradeGold)) return;

        ApplyBatteryUpgrade(1);
        UpdateAllUI();
    }

    public void LightUpgrade()
    {
        if (!TrySpendGold(lightUpgradeGold)) return;

        ApplyLightUpgrade(1);
        UpdateAllUI();
    }

    public void SpeedUpgrade()
    {
        if (!TrySpendGold(speedUpgradeGold)) return;

        ApplySpeedUpgrade(1);
        UpdateAllUI();
    }

    public void AttackUpgrade()
    {
        if (!TrySpendGold(attackUpgradeGold)) return;

        ApplyAttackUpgrade(1);
        UpdateAllUI();
    }

    private bool TrySpendGold(int price)
    {
        if (GameManager.instance == null) return false;

        if (GameManager.instance.money < price)
        {
            ShowNotEnoughMoneyMessage();
            return false;
        }

        ClearRunningMessage();
        GameManager.instance.money -= price;
        return true;
    }

    private void ShowNotEnoughMoneyMessage()
    {
        if (messageText == null) return;

        ClearRunningMessage();
        messageCoroutine = StartCoroutine(ShowMessageForSeconds("돈이 부족합니다", 2f));
    }

    private IEnumerator ShowMessageForSeconds(string message, float seconds)
    {
        // 돈 부족 메시지 표시용임
        messageText.text = message;
        yield return new WaitForSeconds(seconds);
        ClearMessageText();
        messageCoroutine = null;
    }

    private void ClearMessageText()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    private void ClearRunningMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        ClearMessageText();
    }

    private void ApplyLightUpgrade(int count)
    {
        if (count <= 0) return;

        lightUpgradeLevel += count;
        GameManager.instance.baseInnerRadius += 1.15f * count;
        GameManager.instance.baseOuterRadius += 1.15f * count;
        lightUpgradeGold += 20 * count;
    }

    private void ApplySpeedUpgrade(int count)
    {
        if (count <= 0) return;

        speedUpgradeLevel += count;
        speedUpgradeGold += 20 * count;

        // 실제 스피드 업 기능 연결 전 임시 로그임
        Debug.Log("스피드 업!");
    }

    private void ApplyAttackUpgrade(int count)
    {
        if (count <= 0) return;

        attackUpgradeLevel += count;
        attackUpgradeGold += 20 * count;

        // 실제 공격력 업 기능 연결 전 임시 로그임
        Debug.Log("공격력 업!");
    }

    private void ApplyHPUpgrade(int count)
    {
        if (count <= 0) return;

        hpUpgradeLevel += count;
        GameManager.instance.maxHealth += 10 * count;
        hpUpgradeGold += 30 * count;
    }

    private void ApplyBatteryUpgrade(int count)
    {
        if (count <= 0) return;

        batteryUpgradeLevel += count;
        GameManager.instance.maxBattery += 10 * count;
        batteryUpgradeGold += 30 * count;
    }

    private void ApplyBagUpgrade(int count)
    {
        if (count <= 0) return;

        bagUpgradeLevel += count;
        GameManager.instance.maxInventorySlots += 2 * count;
        bagUpgradeGold += 100 * count;
    }

    private void UpdateInventoryUIIfNeeded()
    {
        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.UpdateInventoryUI();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopRange = true;
            SetShopPromptActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopRange = false;
            SetShopPromptActive(false);

            if (isShopOpen)
            {
                CloseShop();
            }
        }
    }
}

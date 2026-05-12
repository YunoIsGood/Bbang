using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public Slider batterySlider;

    void Start()
    {
        if (GameManager.instance != null)
        {
            batterySlider.maxValue = GameManager.instance.maxBattery;
            batterySlider.value = GameManager.instance.currentBattery;
        }
    }

    void Update()
    {
        if (GameManager.instance == null || batterySlider == null)
            return;

        batterySlider.maxValue = GameManager.instance.maxBattery;
        batterySlider.value = GameManager.instance.currentBattery;
    }
}
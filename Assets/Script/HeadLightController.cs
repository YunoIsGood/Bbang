using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HeadLightController : MonoBehaviour
{
    public Light2D headLight;

    void Update()
    {
        if (GameManager.instance == null || headLight == null)
            return;

        headLight.enabled = GameManager.instance.currentBattery > 0f;
    }
}
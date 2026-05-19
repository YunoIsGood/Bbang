using UnityEngine;

public class StopEffect : MonoBehaviour
{
    private float StopTime = 1.0f;

    private void OnEnable()
    {
        Invoke("StopEffectFunc", StopTime);
    }

    private void StopEffectFunc()
    {
        gameObject.SetActive(false);
    }
}

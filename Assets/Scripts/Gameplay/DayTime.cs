using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DayTime : MonoBehaviour
{
    private HDAdditionalLightData sun;

    public static bool IsDay { get; private set; } = true;
    private float TargetIntensity => IsDay ? 120000f : 0.6f;

    private void Start() => sun = GetComponent<HDAdditionalLightData>();

    private void Update()
    {
        sun.intensity = Mathf.Lerp(sun.intensity, TargetIntensity, Time.deltaTime * 0.5f);

        if (Input.GetKeyDown(KeyCode.Q))
            IsDay = !IsDay;
    }
}

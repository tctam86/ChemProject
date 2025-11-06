using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScreenFlickerEffect : MonoBehaviour
{
    private Light2D light2D;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f;

    private void Start()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogError("Light2D component not found on the GameObject.");
        }
    }

    private void Update()
    {
        if (light2D != null)
        {
            float randomIntensity = Random.Range(minIntensity, maxIntensity);
            light2D.intensity = Mathf.Lerp(light2D.intensity, randomIntensity, flickerSpeed);
        }
    }
}
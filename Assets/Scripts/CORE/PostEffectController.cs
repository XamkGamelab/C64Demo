using UnityEngine;
using UnityEngine.Rendering;

public class PostEffectController : MonoBehaviour
{
    public Volume m_Volume;
    public float LerpSpeed = 5f;
    private  VolumeProfile profile;

    private PixelatedPaletteComponent pixelatedPaletteComponent;
    private bool isOn = false;
    private float intensityTarget = 0;
    

    private void Awake()
    {
        profile = m_Volume.sharedProfile;
    }
    //Quick & dirty hack to control some volume settings
    private void Update()
    {
        
        if (profile.TryGet<PixelatedPaletteComponent>(out pixelatedPaletteComponent))
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isOn = !isOn;
                intensityTarget = isOn ? 1 : 0;
                Debug.Log("Intensity t: " + intensityTarget);
            }

            pixelatedPaletteComponent.EffectIntensity.value = intensityTarget; // Mathf.Lerp(doomComponent.EffectIntensity.value, intensityTarget, Time.deltaTime * LerpSpeed);
        }
        
    }
}

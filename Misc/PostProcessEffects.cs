using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostProcessEffects : MonoBehaviour
{

    public static PostProcessEffects instance;

    Volume volume;

    Vignette vignette;
    Bloom bloom;
    LiftGammaGain liftGammaGain;
    MotionBlur motionBlur;
    ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this; 
        }
        volume = GetComponent<Volume>();

        if (volume.profile.TryGet(out Vignette _vignette))
        {
            vignette = _vignette;
        }

        if (volume.profile.TryGet(out Bloom _bloom))
        {
            bloom = _bloom;
        }

        if (volume.profile.TryGet(out LiftGammaGain _liftGammaGain))
        {
            liftGammaGain = _liftGammaGain;
        }

        if (volume.profile.TryGet(out MotionBlur _motionBlur))
        {
            motionBlur = _motionBlur;
        }

        if (volume.profile.TryGet(out ColorAdjustments _colorAdjustments))
        {
            colorAdjustments = _colorAdjustments;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetVignetteValue(float intensity)
    {
        vignette.intensity.value = intensity;
    }

    public float GetVignetteValue()
    {
        return vignette.intensity.value;
    }

    public void SetBloomValue(float intensity)
    {
        bloom.intensity.value = Mathf.Lerp(0f, 0.2f, intensity);
    }

    public float GetBloomValue()
    {
        return bloom.intensity.value;
    }

    public void SetGainValue(float value)
    {
        liftGammaGain.gain.value = Vector4.Lerp(Vector4.one * 0f, Vector4.one * 2f, value);
    }

    public void SetMotionBlurValue(float intensity)
    {
        motionBlur.intensity.value = intensity;
    }

    public float GetMotionBlurValue()
    {
        return motionBlur.intensity.value;
    }

    public void SetColorAdjustmentPostExposure(float value)
    {
        colorAdjustments.postExposure.value = value;
    }

    public float GetColorAdjustmentPostExposure()
    {
        return colorAdjustments.postExposure.value;
    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeirdPostProcessingVolume : MonoBehaviour
{
    public Volume volumeobject;

    private VolumeProfile profile;

    private void Start()
    {
        if (volumeobject == null)
        {
            //Debug.LogError("volume isnt linked ");
            return;
        }

        profile = volumeobject.profile;
    }

    private void Update()
    {
        
    }

    public void RandomizePostProcessing()
    {
        if (profile == null) return;

        if (profile.TryGet<Bloom>(out var bloom))
        {
            bloom.intensity.value = Random.Range(0f, 10f);
            bloom.threshold.value = Random.Range(0.1f, 2f);
            bloom.tint.value = RandomColor();
        }

        if (profile.TryGet<ShadowsMidtonesHighlights>(out var smh))
        {
            smh.shadows.value = RandomColor();  
            smh.midtones.value = RandomColor(); 
            smh.highlights.value = RandomColor(); 
        }

        if (profile.TryGet<PaniniProjection>(out var paniniProjection))
        {
            paniniProjection.distance.value = Random.Range(0f, 1f);
            paniniProjection.cropToFit.value = Random.Range(0f, 1f); 
        }

        if (profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.hueShift.value = Random.Range(-180f, 180f);
            colorAdjustments.saturation.value = Random.Range(-100f, 100f);
            colorAdjustments.contrast.value = Random.Range(-50f, 50f);
        }

        if (profile.TryGet<LensDistortion>(out var lensDistortion))
        {
            lensDistortion.intensity.value = Random.Range(-1f, 1f);
            lensDistortion.scale.value = Random.Range(0.5f, 1.5f);
        }

        if (profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.value = Random.Range(0f, 1f);
            vignette.smoothness.value = Random.Range(0f, 1f);
        }

        if (profile.TryGet<ChromaticAberration>(out var chromaticAberration))
        {
            chromaticAberration.intensity.value = Random.Range(0f, 1f);
        }

        if (profile.TryGet<FilmGrain>(out var filmGrain))
        {
            filmGrain.intensity.value = Random.Range(0f, 1f);
            filmGrain.response.value = Random.Range(0.1f, 1f);
        }

        //Debug.Log("here ");
    }

    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }
}

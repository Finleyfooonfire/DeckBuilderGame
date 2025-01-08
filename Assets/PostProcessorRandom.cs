using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeirdPostProcessingVolume : MonoBehaviour
{
    public Volume volume; // Reference to the Global Volume in your scene
    public KeyCode activateKey = KeyCode.Space; // Key to trigger the weirdness

    private VolumeProfile profile;

    private void Start()
    {
        if (volume == null)
        {
            Debug.LogError("Volume is not assigned!");
            return;
        }

        profile = volume.profile;
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateKey))
        {
            RandomizePostProcessing();
        }
    }

    public void RandomizePostProcessing()
    {
        if (profile == null) return;

        // Bloom
        if (profile.TryGet<Bloom>(out var bloom))
        {
            bloom.intensity.value = Random.Range(0f, 10f);
            bloom.threshold.value = Random.Range(0.1f, 2f);
            bloom.tint.value = RandomColor(); // Random bloom tint
        }

        // Shadows, Midtones, Highlights
        if (profile.TryGet<ShadowsMidtonesHighlights>(out var smh))
        {
            smh.shadows.value = RandomColor();   // Randomize shadows
            smh.midtones.value = RandomColor(); // Randomize midtones
            smh.highlights.value = RandomColor(); // Randomize highlights
        }

        // Panini Projection
        if (profile.TryGet<PaniniProjection>(out var paniniProjection))
        {
            paniniProjection.distance.value = Random.Range(0f, 1f); // Random Panini distance
            paniniProjection.cropToFit.value = Random.Range(0f, 1f); // Random Panini crop
        }

        // Color Adjustments
        if (profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.hueShift.value = Random.Range(-180f, 180f);
            colorAdjustments.saturation.value = Random.Range(-100f, 100f);
            colorAdjustments.contrast.value = Random.Range(-50f, 50f);
        }

        // Lens Distortion
        if (profile.TryGet<LensDistortion>(out var lensDistortion))
        {
            lensDistortion.intensity.value = Random.Range(-1f, 1f);
            lensDistortion.scale.value = Random.Range(0.5f, 1.5f);
        }

        // Vignette
        if (profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.value = Random.Range(0f, 1f);
            vignette.smoothness.value = Random.Range(0f, 1f);
        }

        // Chromatic Aberration
        if (profile.TryGet<ChromaticAberration>(out var chromaticAberration))
        {
            chromaticAberration.intensity.value = Random.Range(0f, 1f);
        }

        // Film Grain
        if (profile.TryGet<FilmGrain>(out var filmGrain))
        {
            filmGrain.intensity.value = Random.Range(0f, 1f);
            filmGrain.response.value = Random.Range(0.1f, 1f);
        }

        Debug.Log("Post-processing randomized!");
    }

    // Generates a random color for effects like Shadows, Midtones, Highlights, and Bloom Tint
    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }
}

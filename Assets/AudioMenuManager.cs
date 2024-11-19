using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource[] audioSources;  // Array to hold the AudioSource components
    public float fadeDuration = 2f;      // Duration of the fade
    public float minInterval = 5f;       // Minimum interval time to wait before fading
    public float maxInterval = 10f;      // Maximum interval time to wait before fading

    private int currentTrackIndex = -1;  // Index to track the current playing track

    void Start()
    {
        // Find all AudioSource components in this GameObject (and its children)
        audioSources = GetComponentsInChildren<AudioSource>();

        // Ensure all audio sources are initially stopped and set to volume 0
        foreach (var source in audioSources)
        {
            source.volume = 0f;
            source.Play();  // Start playback so they are ready
        }

        // Start the fading process
        StartCoroutine(FadeBetweenTracks());
    }

    IEnumerator FadeBetweenTracks()
    {
        while (true)
        {
            // Wait for a random interval before fading to the next track
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Choose the next track randomly, ensuring it's not the same as the current one
            int nextTrackIndex = GetNextTrackIndex();

            // Fade out the current track (if any)
            if (currentTrackIndex >= 0)
            {
                yield return StartCoroutine(FadeOut(audioSources[currentTrackIndex]));
            }

            // Fade in the next track
            yield return StartCoroutine(FadeIn(audioSources[nextTrackIndex]));

            // Update the current track index
            currentTrackIndex = nextTrackIndex;
        }
    }

    IEnumerator FadeIn(AudioSource audioSource)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;
        audioSource.Play();

        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 1f;
    }

    IEnumerator FadeOut(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    // Get a random track index that is not the current one
    int GetNextTrackIndex()
    {
        int nextTrack;
        do
        {
            nextTrack = Random.Range(0, audioSources.Length);
        } while (nextTrack == currentTrackIndex); // Ensure it’s not the same as the current track
        return nextTrack;
    }
}

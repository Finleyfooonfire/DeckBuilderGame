using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DynamicMusicController : MonoBehaviour
{
    private AudioSource[] activeTracks;  // Currently active tracks (stems)
    private bool isTransitioning = false; // Flag to check if we're transitioning between tracks
    public float fadeDuration = 2f;      // Duration of the fade effect
    public float minInterval = 3f;       // Minimum interval time to wait before adding/removing tracks
    public float maxInterval = 7f;       // Maximum interval time to wait before adding/removing tracks
    public int maxActiveTracks = 3;      // Maximum number of tracks that can play at the same time

    // GameObjects that contain AudioSources for each song (Main Menu and Lore)
    public GameObject mainMenuMusicObject;
    public GameObject loreMusicObject;

    private AudioSource[] mainMenuAudioSources;
    private AudioSource[] loreAudioSources;

    void Start()
    {
        // Gather AudioSource components from the provided GameObjects
        mainMenuAudioSources = mainMenuMusicObject.GetComponentsInChildren<AudioSource>();
        loreAudioSources = loreMusicObject.GetComponentsInChildren<AudioSource>();

        if (mainMenuAudioSources.Length == 0)
        {
            Debug.LogError("No AudioSource components found for Main Menu Music! Please assign them in the Inspector.");
            return;
        }

        if (loreAudioSources.Length == 0)
        {
            Debug.LogError("No AudioSource components found for Lore Music! Please assign them in the Inspector.");
            return;
        }

        // Initialize the activeTracks array for dynamic management
        activeTracks = new AudioSource[maxActiveTracks];

        // Ensure all tracks are initially stopped and not playing
        foreach (var source in mainMenuAudioSources)
        {
            source.volume = 0f;
            source.Stop();  // Stop all audio sources initially
        }

        foreach (var source in loreAudioSources)
        {
            source.volume = 0f;
            source.Stop();  // Stop all audio sources initially
        }

        // Start playing Main Menu music dynamically
        StartCoroutine(DynamicTrackManager(mainMenuAudioSources));
        Debug.Log("Main Menu music started dynamically.");
    }

    // Coroutine that manages adding and removing tracks at random intervals
    IEnumerator DynamicTrackManager(AudioSource[] sources)
    {
        Debug.Log("Started DynamicTrackManager for music.");
        while (!isTransitioning)
        {
            // Wait for a random interval before adding/removing a track
            float waitTime = Random.Range(minInterval, maxInterval);
            Debug.Log($"Waiting for {waitTime} seconds before adding/removing a track.");
            yield return new WaitForSeconds(waitTime);

            // Randomly decide whether to add or remove a track
            if (Random.value > 0.5f && GetActiveTrackCount() < maxActiveTracks)
            {
                Debug.Log("Adding a random track.");
                // Add a random track (if not already playing)
                AddRandomTrack(sources);
            }
            else
            {
                Debug.Log("Removing a random track.");
                // Remove a random track (if more than 1 track is active)
                if (GetActiveTrackCount() > 1)
                {
                    RemoveRandomTrack();
                }
            }
        }
    }

    // Add a random track to the active list and fade it in
    void AddRandomTrack(AudioSource[] sources)
    {
        int newTrackIndex = Random.Range(0, sources.Length);
        Debug.Log($"Attempting to add track {newTrackIndex}.");
        while (IsTrackActive(sources[newTrackIndex]))
        {
            newTrackIndex = Random.Range(0, sources.Length);
            Debug.Log($"Track {newTrackIndex} is already active. Trying again.");
        }

        for (int i = 0; i < activeTracks.Length; i++)
        {
            if (activeTracks[i] == null)
            {
                activeTracks[i] = sources[newTrackIndex];
                StartCoroutine(FadeIn(activeTracks[i]));
                Debug.Log($"Added track {newTrackIndex} to active tracks and started fading in.");
                break;
            }
        }
    }

    // Remove a random active track and fade it out
    void RemoveRandomTrack()
    {
        int trackIndex = Random.Range(0, activeTracks.Length);
        while (activeTracks[trackIndex] == null)
        {
            trackIndex = Random.Range(0, activeTracks.Length);
        }

        Debug.Log($"Removing track {trackIndex} and starting fade out.");
        StartCoroutine(FadeOut(activeTracks[trackIndex]));
        activeTracks[trackIndex] = null;
    }

    // Check if a specific track is already active
    bool IsTrackActive(AudioSource track)
    {
        foreach (var activeTrack in activeTracks)
        {
            if (activeTrack == track)
            {
                return true;
            }
        }
        return false;
    }

    // Get the number of currently active tracks
    int GetActiveTrackCount()
    {
        int count = 0;
        foreach (var track in activeTracks)
        {
            if (track != null)
            {
                count++;
            }
        }
        return count;
    }

    // Fade in the selected track
    IEnumerator FadeIn(AudioSource audioSource)
    {
        if (audioSource == null)
        {
            yield break;
        }

        audioSource.volume = 0f;
        audioSource.Play();
        Debug.Log($"Started fading in track {audioSource.name}.");

        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 1f;
        Debug.Log($"Finished fading in track {audioSource.name}. Volume is now 1.");
    }

    // Fade out the selected track
    IEnumerator FadeOut(AudioSource audioSource)
    {
        if (audioSource == null)
        {
            yield break;
        }

        float startVolume = audioSource.volume;
        Debug.Log($"Started fading out track {audioSource.name}.");

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        Debug.Log($"Finished fading out track {audioSource.name}. Track stopped.");
    }

    // Method to transition from Main Menu music to Lore music
    public void TransitionToLoreMusic()
    {
        if (isTransitioning)
        {
            Debug.Log("Already transitioning. Ignoring request to transition to Lore music.");
            return;  // Prevent multiple transitions at once
        }

        Debug.Log("Transitioning to Lore Music...");
        isTransitioning = true;

        // Fade out all active tracks of the current Main Menu music
        foreach (var track in activeTracks)
        {
            if (track != null)
            {
                Debug.Log($"Fading out track {track.name}.");
                StartCoroutine(FadeOut(track));
            }
        }

        // Start the dynamic track management for Lore music after a delay to allow for fading
        StartCoroutine(TransitionToNewMusic(loreAudioSources, mainMenuAudioSources));
    }

    // Coroutine to handle the transition to the new music (Lore)
    IEnumerator TransitionToNewMusic(AudioSource[] newMusicSources, AudioSource[] oldMusicSources)
    {
        // Wait for the fade-out to finish
        yield return new WaitForSeconds(fadeDuration);

        // Reset active tracks
        activeTracks = new AudioSource[maxActiveTracks];

        // Start the dynamic track manager for the new music (Lore)
        StartCoroutine(DynamicTrackManager(newMusicSources));

        // Make sure Lore music starts playing
        foreach (var source in newMusicSources)
        {
            source.Play();
            Debug.Log($"Started playing Lore track {source.name}.");
        }
    }

    // Method to transition back from Lore music to Main Menu music
    public void TransitionToMainMenuMusic()
    {
        if (isTransitioning)
        {
            Debug.Log("Already transitioning. Ignoring request to transition to Main Menu music.");
            return;  // Prevent multiple transitions at once
        }

        Debug.Log("Transitioning to Main Menu Music...");
        isTransitioning = true;

        // Fade out all active tracks of the current Lore music
        foreach (var track in activeTracks)
        {
            if (track != null)
            {
                Debug.Log($"Fading out track {track.name}.");
                StartCoroutine(FadeOut(track));
            }
        }

        // Start the dynamic track management for Main Menu music after a delay to allow for fading
        StartCoroutine(TransitionToNewMusic(mainMenuAudioSources, loreAudioSources));
    }
}

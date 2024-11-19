using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class DynamicMusicController : MonoBehaviour
{
    private List<AudioSource> activeSongs = new List<AudioSource>();  // Tracks that are currently playing
    private List<AudioSource> loadedSongs = new List<AudioSource>();  // Tracks available to be loaded based on the current screen
    private bool isTransitioning = false;  // Flag to check if we're transitioning between screens
    public float fadeDuration = 2f;  // Duration of the fade effect
    public float minInterval = 3f;   // Minimum interval time to wait before adding/removing tracks
    public float maxInterval = 7f;   // Maximum interval time to wait before adding/removing tracks
    public int maxActiveTracks = 3;  // Maximum number of tracks that can play at the same time

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

        // Start with the main menu music loaded and activate it
        LoadMainMenuMusic();
        SetMusicObjectActive(mainMenuMusicObject, true);
        SetMusicObjectActive(loreMusicObject, false);

        // Start the dynamic track manager for Main Menu music
        StartCoroutine(DynamicTrackManager());
        Debug.Log("Main Menu music started dynamically.");
    }

    // Coroutine that manages adding and removing tracks at random intervals
    IEnumerator DynamicTrackManager()
    {
        Debug.Log("Started DynamicTrackManager for music.");

        while (!isTransitioning)
        {
            // Wait for a random interval before adding/removing a track
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Randomly decide whether to add or remove a track
            if (Random.value > 0.5f && activeSongs.Count < maxActiveTracks)
            {
                AddRandomTrack();
            }
            else
            {
                if (activeSongs.Count > 1)  // Ensure we always have at least one track active
                {
                    RemoveRandomTrack();
                }
                else if (activeSongs.Count == 1)
                {
                    // If only one track is active, don't remove it, just add another to keep at least one active
                    AddRandomTrack();
                }
            }

            // Debug the current state of active and loaded tracks
            DebugActiveAndLoadedSongs();
        }
    }

    // Add a random track to the active list and fade it in
    void AddRandomTrack()
    {
        if (loadedSongs.Count == 0) return;

        // Pick a random track from the loaded songs
        int randomIndex = Random.Range(0, loadedSongs.Count);
        AudioSource track = loadedSongs[randomIndex];

        if (activeSongs.Contains(track)) return; // Don't add a track that's already active

        activeSongs.Add(track);
        StartCoroutine(FadeIn(track));
        Debug.Log($"Added track {track.name} to active songs and started fading in.");
    }

    // Remove a random active track and fade it out
    void RemoveRandomTrack()
    {
        if (activeSongs.Count == 0) return;

        // Pick a random track from the active songs
        int randomIndex = Random.Range(0, activeSongs.Count);
        AudioSource track = activeSongs[randomIndex];

        activeSongs.Remove(track);
        StartCoroutine(FadeOut(track));
        Debug.Log($"Removed track {track.name} from active songs and started fading out.");
    }

    // Fade in the selected track
    IEnumerator FadeIn(AudioSource audioSource)
    {
        if (audioSource == null) yield break;

        audioSource.volume = 0f;
        audioSource.Play();
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
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        Debug.Log($"Finished fading out track {audioSource.name}. Track stopped.");
    }

    // Load tracks based on the current screen (Main Menu or Lore)
    void LoadMainMenuMusic()
    {
        loadedSongs.Clear();
        foreach (var track in mainMenuAudioSources)
        {
            loadedSongs.Add(track);
        }
        Debug.Log("Main Menu tracks loaded.");
    }

    void LoadLoreMusic()
    {
        loadedSongs.Clear();
        foreach (var track in loreAudioSources)
        {
            loadedSongs.Add(track);
        }
        Debug.Log("Lore tracks loaded.");
    }

    // Set the active state of a music object (main menu or lore music)
    void SetMusicObjectActive(GameObject musicObject, bool isActive)
    {
        musicObject.SetActive(isActive);
    }

    // Transition from Main Menu music to Lore music
    public void TransitionToLoreMusic()
    {
        if (isTransitioning) return;

        Debug.Log("Transitioning to Lore Music...");
        isTransitioning = true;

        // Fade out all active tracks from Main Menu music
        foreach (var track in activeSongs)
        {
            StartCoroutine(FadeOut(track));
        }

        // Load the Lore music after fading out the Main Menu music
        StartCoroutine(TransitionToNewMusic(LoadLoreMusic, loreMusicObject, mainMenuMusicObject));
    }

    // Transition from Lore music to Main Menu music
    public void TransitionToMainMenuMusic()
    {
        if (isTransitioning) return;

        Debug.Log("Transitioning to Main Menu Music...");
        isTransitioning = true;

        // Fade out all active tracks from Lore music
        foreach (var track in activeSongs)
        {
            StartCoroutine(FadeOut(track));
        }

        // Load the Main Menu music after fading out the Lore music
        StartCoroutine(TransitionToNewMusic(LoadMainMenuMusic, mainMenuMusicObject, loreMusicObject));
    }

    // Coroutine to handle transition logic after fading out tracks
    IEnumerator TransitionToNewMusic(System.Action loadMusicAction, GameObject musicObjectToActivate, GameObject musicObjectToDeactivate)
    {
        // Wait for fade-out to complete
        yield return new WaitForSeconds(fadeDuration);

        // Clear the current active songs
        activeSongs.Clear();

        // Load the appropriate music for the new screen
        loadMusicAction();

        // Start adding tracks to the active list from the loaded songs
        if (loadedSongs.Count > 0)
        {
            AddRandomTrack();  // Always add at least one track after transition
        }

        // Set the music objects active/inactive
        SetMusicObjectActive(musicObjectToActivate, true);
        SetMusicObjectActive(musicObjectToDeactivate, false);

        // Restart the dynamic track management for the new music
        StartCoroutine(DynamicTrackManager());
        isTransitioning = false;
    }

    // Debugging function to show both active and loaded tracks
    void DebugActiveAndLoadedSongs()
    {
        string activeSongsNames = string.Join(", ", activeSongs.ConvertAll(track => track.name));
        string loadedSongsNames = string.Join(", ", loadedSongs.ConvertAll(track => track.name));

        Debug.Log($"Active Tracks: {activeSongsNames}");
        Debug.Log($"Loaded Tracks: {loadedSongsNames}");
    }
}

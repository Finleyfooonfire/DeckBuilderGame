using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GeneralMusicController : MonoBehaviour
{
    private List<AudioSource> activeSongs = new List<AudioSource>();
    private List<AudioSource> loadedSongs = new List<AudioSource>();
    private bool isTransitioning = false;

    public float fadeDuration = 2f; // Duration for fade effects
    public float minInterval = 3f;  // Minimum interval between track changes
    public float maxInterval = 7f;  // Maximum interval between track changes
    public int maxActiveTracks = 3; // Maximum number of concurrent tracks

    public List<AudioClip> mainMenuTracks; // Default tracks for main menu or starting scene
    public List<AudioClip> gameSceneTracks; // Tracks for the game scene

    private AudioSource audioSourcePrefab; // Prefab for creating AudioSources
    private string currentScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Initialize AudioSource prefab
        audioSourcePrefab = new GameObject("AudioSourcePrefab").AddComponent<AudioSource>();
        DontDestroyOnLoad(audioSourcePrefab.gameObject);
        audioSourcePrefab.playOnAwake = false;
        audioSourcePrefab.loop = true;

        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;

        currentScene = SceneManager.GetActiveScene().name;

        // Load default tracks (main menu)
        LoadMusicForScene("MainMenu");
        StartPlayback();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != currentScene)
        {
            currentScene = scene.name;
            TransitionToSceneMusic(scene.name);
        }
    }

    private void LoadMusicForScene(string sceneName)
    {
        loadedSongs.Clear();

        List<AudioClip> sceneTracks = sceneName switch
        {
            "MainMenu" => mainMenuTracks,
            "GameScene" => gameSceneTracks,
            _ => new List<AudioClip>(),
        };

        foreach (var track in sceneTracks)
        {
            AudioSource newSource = Instantiate(audioSourcePrefab, transform);
            newSource.clip = track;
            newSource.Play();
            newSource.volume = 0f; // Start muted
            loadedSongs.Add(newSource);
        }
    }

    private void StartPlayback()
    {
        if (loadedSongs.Count > 0)
        {
            AddRandomTrack();
        }

        StartCoroutine(DynamicTrackManager());
    }

    private void TransitionToSceneMusic(string sceneName)
    {
        if (isTransitioning) return;

        isTransitioning = true;
        StartCoroutine(TransitionMusicCoroutine(sceneName));
    }

    private IEnumerator TransitionMusicCoroutine(string sceneName)
    {
        foreach (var track in activeSongs)
        {
            StartCoroutine(FadeOut(track));
        }

        yield return new WaitForSeconds(fadeDuration);

        activeSongs.Clear();
        LoadMusicForScene(sceneName);

        StartPlayback();
        isTransitioning = false;
    }

    private IEnumerator DynamicTrackManager()
    {
        while (!isTransitioning)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            if (Random.value > 0.5f && activeSongs.Count < maxActiveTracks)
            {
                AddRandomTrack();
            }
            else if (activeSongs.Count > 1)
            {
                RemoveRandomTrack();
            }
        }
    }

    private void AddRandomTrack()
    {
        if (loadedSongs.Count == 0) return;

        int randomIndex = Random.Range(0, loadedSongs.Count);
        AudioSource track = loadedSongs[randomIndex];

        if (activeSongs.Contains(track)) return;

        activeSongs.Add(track);
        StartCoroutine(FadeIn(track));
        DebugTracksState();
    }

    private void RemoveRandomTrack()
    {
        if (activeSongs.Count == 0) return;

        int randomIndex = Random.Range(0, activeSongs.Count);
        AudioSource track = activeSongs[randomIndex];

        activeSongs.Remove(track);
        StartCoroutine(FadeOut(track));
        DebugTracksState();
    }

    private IEnumerator FadeIn(AudioSource audioSource)
    {
        if (audioSource == null) yield break;

        float targetVolume = 1f;
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource audioSource)
    {
        if (audioSource == null) yield break;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 0f;
    }

    private void DebugTracksState()
    {
        string activeTracks = string.Join(", ", activeSongs.ConvertAll(track => track.clip?.name ?? "Unnamed"));
        string inactiveTracks = string.Join(", ", loadedSongs.FindAll(track => !activeSongs.Contains(track))
                                                                  .ConvertAll(track => track.clip?.name ?? "Unnamed"));

        Debug.Log($"[Music Debug] Active Tracks: {activeTracks}");
        Debug.Log($"[Music Debug] Inactive Tracks: {inactiveTracks}");
    }
}

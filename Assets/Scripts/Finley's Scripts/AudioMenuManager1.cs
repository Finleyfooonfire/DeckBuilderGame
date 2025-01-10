using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GeneralMusicController : MonoBehaviour
{
    private List<AudioSource> activetracks = new List<AudioSource>();
    private List<AudioSource> inactivetracks = new List<AudioSource>();
    private bool isTransitioning = false;
    public float fadetime = 2f;
    public float minrandomtime = 3f; 
    public float maxrandomtime = 7f;  
    public int maxtracks = 3; 
    public List<AudioClip> maintracks; 
    public List<AudioClip> secondarytracks; 
    private AudioSource audioprefabspawner; 
    private string currentScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioprefabspawner = new GameObject("Audiosources").AddComponent<AudioSource>();
        DontDestroyOnLoad(audioprefabspawner.gameObject);
        audioprefabspawner.playOnAwake = false;
        audioprefabspawner.loop = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        currentScene = SceneManager.GetActiveScene().name;
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
        inactivetracks.Clear();
        List<AudioClip> sceneTracks = sceneName switch
        {
            "MainMenu" => maintracks,
            "GameScene" => secondarytracks,
            _ => new List<AudioClip>(),
        };
        foreach (var track in sceneTracks)
        {
            AudioSource newSource = Instantiate(audioprefabspawner, transform);
            newSource.clip = track;
            newSource.Play();
            newSource.volume = 0f;
            inactivetracks.Add(newSource);
        }
    }
    private void StartPlayback()
    {
        if (inactivetracks.Count > 0)
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
        foreach (var track in activetracks)
        {
            StartCoroutine(FadeOut(track));
        }
        yield return new WaitForSeconds(fadetime);
        activetracks.Clear();
        LoadMusicForScene(sceneName);
        StartPlayback();
        isTransitioning = false;
    }
    private IEnumerator DynamicTrackManager()
    {
        while (!isTransitioning)
        {
            float waitTime = Random.Range(minrandomtime, maxrandomtime);
            yield return new WaitForSeconds(waitTime);

            if (Random.value > 0.5f && activetracks.Count < maxtracks)
            {
                AddRandomTrack();
            }
            else if (activetracks.Count > 1)
            {
                RemoveRandomTrack();
            }
        }
    }

    private void AddRandomTrack()
    {
        if (inactivetracks.Count == 0) return;
        int randomIndex = Random.Range(0, inactivetracks.Count);
        AudioSource track = inactivetracks[randomIndex];
        if (activetracks.Contains(track)) return;
        activetracks.Add(track);
        StartCoroutine(FadeIn(track));
        DebugTracksState();
    }

    private void RemoveRandomTrack()
    {
        if (activetracks.Count == 0) return;
        int randomIndex = Random.Range(0, activetracks.Count);
        AudioSource track = activetracks[randomIndex];
        activetracks.Remove(track);
        StartCoroutine(FadeOut(track));
        DebugTracksState();
    }

    private IEnumerator FadeIn(AudioSource audioSource)
    {
        if (audioSource == null) yield break;
        float targetVolume = 1f;
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / fadetime;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource audioSource)
    {
        if (audioSource == null) yield break;
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadetime;
            yield return null;
        }
        audioSource.volume = 0f;
    }

    private void DebugTracksState()
    {
        Debug.Log($"[Music Debug] Active Tracks: ");
        Debug.Log($"[Music Debug] Inactive Tracks:");
    }
}

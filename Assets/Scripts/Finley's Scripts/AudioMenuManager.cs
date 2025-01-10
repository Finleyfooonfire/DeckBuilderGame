using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class DynamicMusicController : MonoBehaviour
{
    private List<AudioSource> activeSongs = new List<AudioSource>();  
    private List<AudioSource> inactivesongs = new List<AudioSource>();  
    private bool isTransitioning = false; 
    public float fade = 2f;  
    public float minInterval = 3f;  
    public float maxInterval = 7f;   
    public int maxtracks = 3;  
    public GameObject mainobject;
    public GameObject loreobject;
    private AudioSource[] mainsongs;
    private AudioSource[] loresongs;

    void Start()
    {
        mainsongs = mainobject.GetComponentsInChildren<AudioSource>();
        loresongs = loreobject.GetComponentsInChildren<AudioSource>();

        if (mainsongs.Length == 0)
        {
            Debug.LogError("No AudioSource components found for Main Menu Music! Please assign them in the Inspector.");
            return;
        }

        if (loresongs.Length == 0)
        {
            Debug.LogError("No AudioSource components found for Lore Music! Please assign them in the Inspector.");
            return;
        }
        LoadMainMenuMusic();
        SetMusicObjectActive(mainobject, true);
        SetMusicObjectActive(loreobject, false);
        StartCoroutine(DynamicTrackManager());
        //Debug.Log("Main Menu music started dynamically.");
    }
    IEnumerator DynamicTrackManager()
    {
        //Debug.Log("Started DynamicTrackManager for music.");
        while (!isTransitioning)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);
            if (Random.value > 0.5f && activeSongs.Count < maxtracks)
            {
                AddRandomTrack();
            }
            else
            {
                if (activeSongs.Count > 1)  
                {
                    RemoveRandomTrack();
                }
                else if (activeSongs.Count == 1)
                {
                    AddRandomTrack();
                }
            }
            DebugActiveAndLoadedSongs();
        }
    }

    void AddRandomTrack()
    {
        if (inactivesongs.Count == 0) return;
        int randomIndex = Random.Range(0, inactivesongs.Count);
        AudioSource track = inactivesongs[randomIndex];

        if (activeSongs.Contains(track)) return; 

        activeSongs.Add(track);
        StartCoroutine(FadeIn(track));
        //Debug.Log($"Added track {track.name} to active songs and started fading in.");
    }

    void RemoveRandomTrack()
    {
        if (activeSongs.Count == 0) return;
        int randomIndex = Random.Range(0, activeSongs.Count);
        AudioSource track = activeSongs[randomIndex];
        activeSongs.Remove(track);
        StartCoroutine(FadeOut(track));
        //Debug.Log($"Removed track {track.name} from active songs and started fading out.");
    }

    IEnumerator FadeIn(AudioSource audioSource)
    {
        if (audioSource == null) yield break;
        audioSource.volume = 0f;
        audioSource.Play();
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fade;
            yield return null;
        }
        audioSource.volume = 1f;
        //Debug.Log($"Finished fading in track {audioSource.name}. Volume is now 1.");
    }
    IEnumerator FadeOut(AudioSource audioSource)
    {
        if (audioSource == null) yield break;
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fade;
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
    void LoadMainMenuMusic()
    {
        inactivesongs.Clear();
        foreach (var track in mainsongs)
        {
            inactivesongs.Add(track);
        }
        
    }

    void LoadLoreMusic()
    {
        inactivesongs.Clear();
        foreach (var track in loresongs)
        {
            inactivesongs.Add(track);
        }
    }

    void SetMusicObjectActive(GameObject musicObject, bool isActive)
    {
        musicObject.SetActive(isActive);
    }

    public void TransitionToLoreMusic()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        foreach (var track in activeSongs)
        {
            StartCoroutine(FadeOut(track));
        }
        StartCoroutine(TransitionToNewMusic(LoadLoreMusic, loreobject, mainobject));
    }

    public void TransitionToMainMenuMusic()
    {
        if (isTransitioning) return;
        //Debug.Log("Transitioning to Main Menu Music...");
        isTransitioning = true;
        foreach (var track in activeSongs)
        {
            StartCoroutine(FadeOut(track));
        }
        StartCoroutine(TransitionToNewMusic(LoadMainMenuMusic, mainobject, loreobject));
    }

    IEnumerator TransitionToNewMusic(System.Action loadMusicAction, GameObject musicObjectToActivate, GameObject musicObjectToDeactivate)
    {
        yield return new WaitForSeconds(fade);
        activeSongs.Clear();
        loadMusicAction();
        if (inactivesongs.Count > 0)
        {
            AddRandomTrack(); 
        }
        SetMusicObjectActive(musicObjectToActivate, true);
        SetMusicObjectActive(musicObjectToDeactivate, false);
        StartCoroutine(DynamicTrackManager());
        isTransitioning = false;
    }

   void DebugActiveAndLoadedSongs()
    {

        //Debug.Log($"Active Tracks: {activeSongsNames}");
        //Debug.Log($"Loaded Tracks: {inactivesongsNames}");
    }
}

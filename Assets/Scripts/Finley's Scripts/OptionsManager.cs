using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public Slider volume; 
    public Toggle togglebutton; 
    public TMP_Dropdown qualitydropdown; 
    public TMP_Dropdown resolutionoptions; 
    public AudioMixer mainaudiomonitor; 
    public Button mainmenubutton; 

    private void Start()
    {
        LoadSettings();
        volume.onValueChanged.AddListener(SetVolume);
        PopulateResolutionDropdown();
        mainmenubutton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!optionsMenu.activeSelf)
            {
                OpenOptionsMenu();
            }
            else
            {
                CloseOptionsMenu();
            }
        }
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        mainaudiomonitor.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); 
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", volume.value);
        PlayerPrefs.SetInt("Fullscreen", togglebutton.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Resolution", resolutionoptions.value); 
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            volume.value = savedVolume; 
            SetVolume(savedVolume); 
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            togglebutton.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        }

        if (PlayerPrefs.HasKey("Resolution"))
        {
            resolutionoptions.value = PlayerPrefs.GetInt("Resolution");
        }
    }

    private void PopulateResolutionDropdown()
    {
        resolutionoptions.ClearOptions();
        Resolution[] resolutions = Screen.resolutions;
        var resolutionOptions = new System.Collections.Generic.List<string>();

        foreach (var resolution in resolutions)
        {
            resolutionOptions.Add(resolution.width + "x" + resolution.height);
        }
        resolutionoptions.AddOptions(resolutionOptions);
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                resolutionoptions.value = i;
                break;
            }
        }
        resolutionoptions.onValueChanged.AddListener(SetResolution);
    }
    public void ReturnToMainMenu()
    {
        SaveSettings();
        SceneManager.LoadScene("MainMenu");
    }
}

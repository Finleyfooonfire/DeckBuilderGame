using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Required for Audio Mixer functionality

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu; // The options menu GameObject
    public Slider volumeSlider; // The slider for volume control
    public Toggle fullscreenToggle; // The toggle for fullscreen
    public Dropdown qualityDropdown; // The dropdown for quality settings
    public AudioMixer audioMixer; // Reference to your AudioMixer

    private void Start()
    {
        // Load saved settings on start
        LoadSettings();
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }

    // Method to change the volume based on slider value
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume); // Adjust the volume using an Audio Mixer parameter
    }

    // Method to toggle fullscreen mode
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // Method to change graphics quality
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Save the settings (you could use PlayerPrefs or a file)
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Quality", qualityDropdown.value);
        PlayerPrefs.Save();
    }

    // Load settings when the menu is opened
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        }
        if (PlayerPrefs.HasKey("Quality"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("Quality");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Required for Audio Mixer functionality
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu; // The options menu GameObject
    public Slider volumeSlider; // The slider for volume control
    public Toggle fullscreenToggle; // The toggle for fullscreen
    public TMP_Dropdown qualityDropdown; // The dropdown for quality settings
    public AudioMixer audioMixer; // Reference to your AudioMixer

    private void Start()
    {
        // Load saved settings on start
        LoadSettings();

        // Initialize the volume slider with the saved volume
        volumeSlider.onValueChanged.AddListener(SetVolume); // Ensure volume is updated when the slider value changes
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
        // Adjust the volume using the AudioMixer parameter "MasterVolume"
        // Map the slider value (0 to 1) to a suitable dB value (-80 to 0)
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); // Logarithmic scale for smooth volume changes
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
        PlayerPrefs.SetFloat("Volume", volumeSlider.value); // Save volume slider value
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Quality", qualityDropdown.value);
        PlayerPrefs.Save();
    }

    // Load settings when the menu is opened
    private void LoadSettings()
    {
        // Load the volume setting from PlayerPrefs
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = savedVolume; // Set the slider value to the saved volume
            SetVolume(savedVolume); // Adjust the AudioMixer's volume accordingly
        }

        // Load fullscreen toggle setting
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        }

        // Load graphics quality setting
        if (PlayerPrefs.HasKey("Quality"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("Quality");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;  // Add this for scene management

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu; // The options menu GameObject
    public Slider volumeSlider; // The slider for volume control
    public Toggle fullscreenToggle; // The toggle for fullscreen
    public TMP_Dropdown qualityDropdown; // The TMP dropdown for quality settings
    public TMP_Dropdown resolutionDropdown; // The TMP dropdown for resolution settings
    public AudioMixer audioMixer; // Reference to your AudioMixer
    public Button returnToMainMenuButton; // The button for returning to the main menu

    private void Start()
    {
        // Load saved settings on start
        LoadSettings();

        // Initialize the volume slider with the saved volume
        volumeSlider.onValueChanged.AddListener(SetVolume); // Ensure volume is updated when the slider value changes

        // Populate the resolution dropdown with available screen resolutions
        PopulateResolutionDropdown();

        // Add listener for the return to main menu button
        returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        // Listen for the Escape key (ESC) to toggle the options menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the options menu is not already active, open it. Otherwise, close it.
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

    // Method to change the volume based on slider value
    public void SetVolume(float volume)
    {
        // Adjust the volume using the AudioMixer parameter "MasterVolume"
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

    // Method to change screen resolution
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Save the settings (you could use PlayerPrefs or a file)
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value); // Save volume slider value
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value); // Save resolution dropdown value
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

        // Load resolution setting
        if (PlayerPrefs.HasKey("Resolution"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        }
    }

    // Populate the resolution dropdown with available screen resolutions
    private void PopulateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions(); // Clear existing options

        // Get all available screen resolutions
        Resolution[] resolutions = Screen.resolutions;

        // Create a list of resolution strings
        var resolutionOptions = new System.Collections.Generic.List<string>();

        foreach (var resolution in resolutions)
        {
            resolutionOptions.Add(resolution.width + "x" + resolution.height);
        }

        // Add options to the dropdown
        resolutionDropdown.AddOptions(resolutionOptions);

        // Set the default value in the dropdown to the current resolution
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        // Add listener to handle selection change
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // New method to return to the main menu
    public void ReturnToMainMenu()
    {
        // Optionally, save settings before returning
        SaveSettings();

        // Load the main menu scene (replace "MainMenu" with the actual name of your main menu scene)
        SceneManager.LoadScene("MainMenu");
    }
}

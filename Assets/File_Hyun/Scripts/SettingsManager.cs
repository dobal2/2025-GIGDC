using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Text volumeLabel;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;

        AudioListener.volume = savedVolume;
        SetVolume(1);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameObject.activeSelf)
        {
            CloseSettings();
        }
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        int percent = Mathf.RoundToInt(value * 100);
        volumeLabel.text = $"À½·®: {percent}%";
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider vfxSlider;
    public TMP_Text bgmLabel;
    public TMP_Text vfxLabel;

    void Start()
    {
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float savedVFX = PlayerPrefs.GetFloat("VFXVolume", 1f);

        bgmSlider.value = savedBGM;
        vfxSlider.value = savedVFX;

        SetBGMVolume(savedBGM);
        SetVFXVolume(savedVFX);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        vfxSlider.onValueChanged.AddListener(SetVFXVolume);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }

    public void SetBGMVolume(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
        PlayerPrefs.SetFloat("BGMVolume", value);
        int percent = Mathf.RoundToInt(value * 100);
        bgmLabel.text = $"BGM : {percent}%";
    }

    public void SetVFXVolume(float value)
    {
        AudioManager.Instance.SetVFXVolume(value);
        PlayerPrefs.SetFloat("VFXVolume", value);
        int percent = Mathf.RoundToInt(value * 100);
        vfxLabel.text = $"SFX : {percent}%";
    }
}
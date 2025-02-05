using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        SetVolume(masterVolumeSlider, "MasterVolume", "MasterVolumeValue", PlayerPrefs.GetFloat("MasterVolumeValue"));
        SetVolume(musicVolumeSlider, "MusicVolume", "MusicVolumeValue", PlayerPrefs.GetFloat("MusicVolumeValue"));
        SetVolume(sfxVolumeSlider, "SFXVolume", "SFXVolumeValue", PlayerPrefs.GetFloat("SFXVolumeValue"));
    }

    public void SetMasterVolumeFromSlider()
    {
        SetVolume(masterVolumeSlider, "MasterVolume", "MasterVolumeValue", masterVolumeSlider.value);
    }

    public void SetMusicVolumeFromSlider()
    {
        SetVolume(musicVolumeSlider, "MusicVolume", "MusicVolumeValue", musicVolumeSlider.value);
    }

    public void SetSFXVolumeFromSlider()
    {
        SetVolume(sfxVolumeSlider, "SFXVolume", "SFXVolumeValue", sfxVolumeSlider.value);
    }

    private void SetVolume(Slider volumeSlider, string mixerParam, string volumeParam, float value)
    {
        if (value < 0.5f)
        {
            value = 0.01f;
        }

        volumeSlider.value = value;
        PlayerPrefs.SetFloat(volumeParam, value);
        audioMixer.SetFloat(mixerParam, Mathf.Log10(value / 100) * 20f);
    }
}

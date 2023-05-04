using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Этот скрипт писал не я
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource ButtonSource;
    public AudioMixer Mixer;

    public static void PlayButtonSound(AudioClip Clip)
    {
        if (SoundManager.Instance.ButtonSource) SoundManager.Instance.ButtonSource.PlayOneShot(Clip);
    }

    private void Awake()
    {
        Instance = this;
    }

    public static void SetSettings(float MusicLevel, float SoundsLevel)
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.UpdateSettings(MusicLevel, SoundsLevel);
        }
    }

    void UpdateSettings(float MusicLevel, float SoundsLevel)
    {
        Mixer.SetFloat("MusicVolume", Mathf.Log10(MusicLevel) * 20);
        Mixer.SetFloat("SoundsVolume", Mathf.Log10(SoundsLevel) * 20);
    }


}

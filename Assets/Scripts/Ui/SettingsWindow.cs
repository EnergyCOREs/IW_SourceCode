using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Этот скрипт писал не я
/// </summary>
public class SettingsWindow : MonoBehaviour
{
    public static SettingsWindow instance;

    public Toggle BetterGraphic;
    public Toggle BetterParticles;

    public Slider Volume_Music;
    public Slider Volume_Sounds;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Загрузить настройки
        //LoadSettings();

        //Подписаться на все кнопки
        BetterGraphic.onValueChanged.AddListener(ChangeToggle);
        BetterParticles.onValueChanged.AddListener(ChangeToggle);
        Volume_Music.onValueChanged.AddListener(ChangeSlider);
        Volume_Sounds.onValueChanged.AddListener(ChangeSlider);
        MapGlobals.Instance.UserBusinessLogic.SettingsLoaded += LoadSettings;
        
    }

    void ChangeToggle(bool Value)
    {
        OnSettingsChange();
        SaveSettings();
    }
    void ChangeSlider(float Value)
    {
        OnSettingsChange();
        SaveSettings();
    }

    void OnSettingsChange()
    {
        SoundManager.SetSettings(Volume_Music.value, Volume_Sounds.value);
        GraphicsManager.SetSettings(BetterGraphic.isOn, BetterParticles.isOn);
    }



    void LoadSettings()
    {
        BetterGraphic.isOn = MapGlobals.Instance.UserBusinessLogic.SaveObject.ExtendedGraphics;
        BetterParticles.isOn = MapGlobals.Instance.UserBusinessLogic.SaveObject.ExtendedParticles;
        Volume_Music.value = MapGlobals.Instance.UserBusinessLogic.SaveObject.MusicLevel;
        Volume_Sounds.value = MapGlobals.Instance.UserBusinessLogic.SaveObject.SoundLevel;
        OnSettingsChange();
    }

    void SaveSettings()
    {
        MapGlobals.Instance.UserBusinessLogic.SaveObject.ExtendedGraphics = BetterGraphic.isOn;
        MapGlobals.Instance.UserBusinessLogic.SaveObject.ExtendedParticles = BetterParticles.isOn;
        MapGlobals.Instance.UserBusinessLogic.SaveObject.MusicLevel = Volume_Music.value;
        MapGlobals.Instance.UserBusinessLogic.SaveObject.SoundLevel = Volume_Sounds.value;
    }

}

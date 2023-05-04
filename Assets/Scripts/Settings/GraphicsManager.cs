using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Этот скрипт писал не я
/// </summary>
public class GraphicsManager : MonoBehaviour
{
    public static GraphicsManager instance;

    public Color UnderwaterFogColor = Color.cyan;
    public Camera PlayerCamera;


    public bool HighQualityGraphics = true;
    public bool HighQualityParticles = true;
    public Light SunLight;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateFogValues();
        ChangeSettings(HighQualityGraphics, HighQualityParticles);
    }

    private void Update()
    {
        UpdateFogValues();
    }



    void UpdateFogValues()
    {
        Shader.SetGlobalVector("_FogColor", UnderwaterFogColor);
        if (PlayerCamera)
        {
            PlayerCamera.backgroundColor = UnderwaterFogColor;
        }
    }

    public static void SetSettings(bool HQG, bool HQO)
    {
        if (GraphicsManager.instance)
        {
            GraphicsManager.instance.ChangeSettings(HQG, HQO);
        }
    }


    public void ChangeSettings(bool HQG, bool HQO)
    {
        HighQualityGraphics = HQG;
        HighQualityParticles = HQO;

        if (SunLight)
        {
            if (HighQualityGraphics)
            {
                SunLight.shadows = LightShadows.Hard;
            }
            else
            {
                SunLight.shadows = LightShadows.None;
            }
        }


        //Выключаем обьекты красивостей
        QualityRenderSwitch[] myItems = FindObjectsOfType(typeof(QualityRenderSwitch)) as QualityRenderSwitch[];
        foreach (QualityRenderSwitch item in myItems)
        {
            item.SetChildState(HighQualityParticles);
        }
        if (Screen.currentResolution.refreshRate > 60)
        {
            Application.targetFrameRate = 144;
        }
        else
        {
            Application.targetFrameRate = 60;
        }

        //Переключаем шейдерные приколы в режим бомжа
        if (HighQualityGraphics)
        {
            Shader.EnableKeyword("LowMode_OFF");
            Shader.DisableKeyword("LowMode_ON");
        }
        else
        {
            Shader.EnableKeyword("LowMode_ON");
            Shader.DisableKeyword("LowMode_OFF");
        }
    }
}

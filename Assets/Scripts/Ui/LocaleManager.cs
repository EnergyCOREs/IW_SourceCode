using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleLocalization;

/// <summary>
/// Этот скрипт писал не я
/// </summary>
public class LocaleManager : MonoBehaviour
{
    private void Awake()
    {
        LocalizationManager.Read();


        switch (Application.systemLanguage)
        {
            case SystemLanguage.Russian:
                LocalizationManager.Language = "Russian";
                break;
            default:
                LocalizationManager.Language = "English";
                break;
        }


    }

    public void SetLocalization(string localization)
    {
        LocalizationManager.Language = localization;
    }

}

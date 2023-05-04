using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Этот скрипт писал не я
/// </summary>
public class SliderLevelSound : MonoBehaviour
{

    Slider targetSlider;
    AudioSource AudioClick;
    double lastValue, newValue;

    private void Awake()
    {
        targetSlider = this.GetComponent<Slider>();
        AudioClick = this.GetComponent<AudioSource>();

        if (targetSlider)
        {
            if (AudioClick)
            {
                lastValue = System.Math.Round(targetSlider.value, 1);
                targetSlider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }
    }

    void OnSliderValueChanged(float Value)
    {
        newValue = System.Math.Round(Value, 1);
        if (lastValue != newValue)
        {
            lastValue = newValue;
            AudioClick.Play();
        }
    }

}

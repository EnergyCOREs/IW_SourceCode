using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTimer : MonoBehaviour
{
    private float _timer, _startTimer;
    [SerializeField] private Image _filledImage;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = MapGlobals.Instance.WorldCameraComponent;
        UpgradeVisual(0f);
    }

    public void SetTime(float time)
    {
        _timer = _startTimer = time;
        UpgradeVisual(1f);
    }

    private void Update()
    {
        if (!Mathf.Approximately(0, _timer))
        {
            _timer -= Time.deltaTime;
            _timer = Mathf.Max(_timer, 0);
            UpgradeVisual(_timer / _startTimer);
        }
    }

    private void UpgradeVisual(float value)
    {
        value = Mathf.Clamp01(1 - value);
        _filledImage.fillAmount = Mathf.Round(value*10)/10;
    }
}

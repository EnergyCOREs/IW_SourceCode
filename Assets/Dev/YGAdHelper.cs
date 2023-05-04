using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class YGAdHelper
{
    private int _id = 0;
    private int _temporalId = 0;

    public Action<int> Rewarded; 
    public Action<int> NoRewarded;

    public YGAdHelper()
    {
        YandexGame.RewardVideoEvent += OnRewarded;
        YandexGame.ErrorVideoEvent += OnNoRewarded;
        YandexGame.CloseVideoEvent += OnNoRewarded;
        Debug.Log("YG Helper initialized");
    }

    public void ShowRewardAD(int id)
    {
        _temporalId = id;
        YandexGame.RewVideoShow(id);
    }

    public void OnNoRewarded()
    {
        NoRewarded?.Invoke(_temporalId);
    }

    public void OnRewarded(int id)
    {
        Rewarded?.Invoke(id);
    }

    public int GetFreeId()
    {
        return _id++;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraderLine : MonoBehaviour
{
    [SerializeField] private TMP_Text _rewardCount, _doubleRewardCount;
    [SerializeField] private Image _resourceImage;
    private int _maxCount;
    private TraderVisual _traderVisual;
    private ResourceObject _item;
    private TradeLot _lot;
    private int _rewardTemp;

    public int CalculatedReward => _rewardTemp;
    internal void Init(ResourceObject item, TradeLot lot, TraderVisual traderVisual)
    {
        _traderVisual = traderVisual;
        _item = item;
        _lot = lot;
        UpdateVisual();
    }

    private void UpdateVisual()
    {

        _resourceImage.sprite = _lot.Price[0].ResourceType.Icon;
        _maxCount = MapGlobals.Instance.Player.ResourceObjects / _lot;
        //_count.text = $"{_item.Count * _maxCount}";

        _rewardTemp = GetMaximum();
        gameObject.SetActive(_rewardTemp != 0);

        _rewardCount.text = $"{_rewardTemp}";
        _doubleRewardCount.text = $"{_rewardTemp * 2}";
        _traderVisual.UpdateVisual();
    }

    public void Sell()
    {
        TradeLot.Trade(MapGlobals.Instance.Player, MapGlobals.Instance.Player, _lot, _maxCount);
        UpdateVisual();
    }

    public void SellDouble()
    {
        MapGlobals.Instance.UserBusinessLogic.ShowDoubleRewardVideo(SellDoubleCallback);
    }

    public void SellDoubleCallback(int id)
    {
        if (id != 2)
            return;

        var reward = GetMaximum();
        TradeLot.Trade(MapGlobals.Instance.Player, MapGlobals.Instance.Player, _lot, _maxCount);
        TradeLot.AddResource(MapGlobals.Instance.DefaultOneRubyLot.Price[0].ResourceType, MapGlobals.Instance.Player, reward);
        UpdateVisual();

        MapGlobals.Instance.UserBusinessLogic.ResetHandler();
    }

    internal int GetMaximum()
    {
        return _lot.Reward[0].Count * _maxCount;
    }
}

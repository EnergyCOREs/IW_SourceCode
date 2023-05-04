using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TraderVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text _normalSellText, _videoSellText;
    [SerializeField] private TraderLine _traderLine;
    [SerializeField] private Transform _container;
    private List<TraderLine> _lines = new List<TraderLine>();

    internal void Reinit()
    {
        _lines.Clear();

        for (int i = 0; i < _container.childCount; i++)
        {
            Destroy(_container.GetChild(i).gameObject);
        }

        foreach (var item in MapGlobals.Instance.Player.ResourceObjects)
        {
            var lot = MapGlobals.Instance.AvialableLots.Find(a => a.Price[0].ResourceType == item.ResourceType);
            if (lot != null)
            {
                var line = Instantiate(_traderLine, _container);
                line.Init(item, lot, this);
                _lines.Add(line);
            }
        }

        //GetMaximumReward();
        UpdateVisual();
    }

    public int GetMaximumReward()
    {
        int maximum = 0;
        foreach (var item in _lines)
        {
            maximum += item.CalculatedReward;
        }
        return maximum;
    }

    public void SellAll()
    {
        foreach (var item in _lines)
        {
            item.Sell();
        }
    }

    public void SellAllDouble()
    {
        MapGlobals.Instance.UserBusinessLogic.ShowDoubleRewardVideo(SellAllDoubleCallback);
    }

    public void SellAllDoubleCallback(int id)
    {
        if (id != 2)
            return;

        var reward = GetMaximumReward();
        SellAll();
        TradeLot.AddResource(MapGlobals.Instance.DefaultOneRubyLot.Price[0].ResourceType, MapGlobals.Instance.Player, reward);
        MapGlobals.Instance.UserBusinessLogic.ResetHandler();
    }

    public void UpdateVisual()
    {
        _normalSellText.text = $"{GetMaximumReward()}" ;
        _videoSellText.text = $"{GetMaximumReward()*2}" ;
    }
}

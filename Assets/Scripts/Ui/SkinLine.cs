using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkinLine : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private TMP_Text _cost;

    [SerializeField] private GameObject _useButtonVisual;
    [SerializeField] private GameObject _buyButtonVisual;
    [SerializeField] private GameObject _rewardButtonVisual;

    private UserBusinessLogic _businessLogic;
    private SkinListing _currentSkin;

    public void Init(SkinListing skin, UserBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
        _currentSkin = skin;
        _text.text = skin.Name;
        _cost.text = skin.CostRuby.ToString();
        Rect rect = new Rect(skin.Skintexture.width / 8, skin.Skintexture.height / 8, skin.Skintexture.width / 8, skin.Skintexture.height / 8);
        _icon.sprite = Sprite.Create(skin.Skintexture as Texture2D, rect, new Vector2(0.5f, 0.5f));
        _businessLogic.SkinBuyCompleted += OnSkinBought;

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        _useButtonVisual.SetActive(false);
        _rewardButtonVisual.SetActive(false);
        _buyButtonVisual.SetActive(false);

        if ((_currentSkin.CostRuby < 0)||(_businessLogic.CheckSkinAvialable(_currentSkin.HashKey)))
        {
            _useButtonVisual.SetActive(true);
            return;
        }

        if (_currentSkin.CostRuby == 0)
            _rewardButtonVisual.SetActive(true);

        if (_currentSkin.CostRuby > 0)
            _buyButtonVisual.SetActive(true);

    }

    public void OnSkinBought(string key)
    {
        if (key == _currentSkin.HashKey)
            UpdateButtons();
    }

    public void BuyByVideo()
    {
        _businessLogic.BuySkinVideo(_currentSkin.HashKey);

    }

    public void BuyByRyby()
    {
        Debug.Log("BuyByRuby");
        _businessLogic.BuySkinRuby(_currentSkin.HashKey, _currentSkin.CostRuby);
    }

    public void UseSkin()
    {
        MapGlobals.Instance.SkinSelector.SetSkin(_currentSkin.HashKey);
    }
}

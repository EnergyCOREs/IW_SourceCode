using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UpgradeVisual : MonoBehaviour
{
    private UserBusinessLogic _businessLogic;
    private Building _building;

    [SerializeField] private TMP_Text _textName;
    [SerializeField] private TMP_Text _textLevel;
    [SerializeField] private TMP_Text _textCost;
    [SerializeField] private TMP_Text _textCurrentNpc, _textNextNPC;
    [SerializeField] private TMP_Text _textSpeed;
    [SerializeField] private TMP_Text _textSpeedFabrication;

    [SerializeField] private GameObject _npcPanel, _speedPanel, _fabricationSpeedPanel, _maxLevelPanel;
    [SerializeField] private GameObject[] _buttons;

    private void Start()
    {
        _businessLogic = MapGlobals.Instance.UserBusinessLogic;
        MapGlobals.Instance.UserBusinessLogic.BuildingBuyCompleted += OnComplete;
    }

    public void Reinit(Building building)
    {

        _building = building;
        if (building == null)
            return;

        Debug.Log("Reinit");

        OnUpgradeVisual();
    }

    private void OnUpgradeVisual()
    {
        Debug.Log("Upgrade visual");


        _textName.text = _building.BuildingName;
        _textLevel.text = _building._currentGrade.ToString();
        if (_building.IsMaximumLevel)
        {
            HideAll();
            _maxLevelPanel.SetActive(true);
            return;
        }
        ButtonsSetActive(true);

        var buildingUpgradeCost = _building.CalculateUpgradePrice(_building._currentGrade + 1);

        _textCost.text = $"{TradeLot.ScoreFormat(buildingUpgradeCost)}";
        _textCost.color = (MapGlobals.Instance.GetPlayerRubyCount() >= buildingUpgradeCost) ? Color.white : Color.red;
        _maxLevelPanel.SetActive(false);

        int speed = 0;

        if (_building.CanSpawnNPC)
        {
            var buildingNPCCount = _building.CalculateNPCCount(_building._currentGrade);
            var buildingNPCCountNext = _building.CalculateNPCCount(_building._currentGrade + 1);
            var buildingNPCSpeed = _building.CalculateNPCSpeed(_building._currentGrade);
            var buildingNPCSpeedNext = _building.CalculateNPCSpeed(_building._currentGrade + 1);

            _npcPanel.SetActive(buildingNPCCountNext > 0);
            _speedPanel.SetActive(speed > 1);
            _fabricationSpeedPanel.SetActive(false);

            _textCurrentNpc.text = buildingNPCCount.ToString();
            _textNextNPC.text = buildingNPCCountNext.ToString();
            speed = Mathf.CeilToInt(100f - (100f / (buildingNPCSpeed * buildingNPCCount)) * (buildingNPCSpeedNext * buildingNPCCountNext));
            _textSpeed.text = $"+{speed} %";
        }
        else
        {
            _npcPanel.SetActive(false);
            _speedPanel.SetActive(false);
            _fabricationSpeedPanel.SetActive(true);
            speed = _building.GetFabricationSpeedOnUpgrade();
            _textSpeedFabrication.text = $"+{speed} %";
        }
    }

    public void OnComplete(Building building)
    {
        if (building != _building)
            return;

        OnUpgradeVisual();
    }

    private void ButtonsSetActive(bool status)
    {
        foreach (var item in _buttons)
        {
            item.SetActive(status);
        }
    }

    private void HideAll()
    {
        _npcPanel.SetActive(false);
        _speedPanel.SetActive(false);
        _fabricationSpeedPanel.SetActive(false);
        _textCost.text = "0";
        ButtonsSetActive(false);
        // _maxLevelPanel.SetActive(false);
    }

    public void UpgradeByRuby()
    {
        _businessLogic.BuyBuildingGradeRuby(_building);
    }

    public void UpgradeByVideo()
    {
        _businessLogic.DropCostBuildingGradeVideo(_building);
    }
}

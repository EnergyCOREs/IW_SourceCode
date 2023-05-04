using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

// 0 - апгрейд
// 1 - скин
// 2 - умножение
// потом в энум переведу

[Serializable]
public class UserBusinessLogic
{
    public UserSaves SaveObject = new UserSaves();

    public delegate void Operation(int id);
    public Operation OperationHandler;

    public Action<string> SkinBuyCompleted;
    public Action<Building> BuildingBuyCompleted;
    public Action SettingsLoaded;

    private Building _storedBuilding;
    private string _storedHashKey;
    private string _jsonString;
    private float _timer;

    public const int SAVE_VERSION = 1;
    public void ShowDoubleRewardVideo(Operation callbackMethod)
    {
        OperationHandler = callbackMethod;
        YandexGame.RewVideoShow(2);
    }

    public void ResetHandler()
    {
        OperationHandler = Void;
    }

    public void ForceLoad()
    {

        YandexGame.GetDataEvent += OnLoadedFromYG;
        YandexGame.RewardVideoEvent += RewardVideoEvent;
        YandexGame.LoadProgress();
        Debug.Log("YG Force Load");
    }

    private void RewardVideoEvent(int id)
    {
        OperationHandler(id);
    }

    private void OnLoadedFromYG()
    {
        _jsonString = YandexGame.savesData.jsonString;
        Debug.Log($"YG load DataEvent - {_jsonString}");
        LoadSettings();
    }

    public void Void(int id)
    {

    }

    internal void Tick(float delta)
    {
        if (_timer > 0)
        {
            _timer -= delta;
            if (_timer < 0)
            {
                SaveSettings();
            }
        }
    }

    internal void SaveTimer(float time)
    {
        _timer = Mathf.Max(time, _timer);
    }


    public void SaveSettings()
    {
        // ReconnectToMap();
        SaveObject.SaveData = MapGlobals.Instance.SaveSystem.GetJson();
        SaveObject.SaveVersion = SAVE_VERSION;
        string jsonString = JsonUtility.ToJson(SaveObject);
        YandexGame.savesData.jsonString = jsonString;
        // Debug.Log(jsonString);
        YandexGame.SaveProgress();
        // PlayerPrefs.SetString("Save", jsonString);

    }

    public void LoadSettings()
    {
        //ReconnectToMap();
        // string jsonString = PlayerPrefs.GetString("Save");

        string jsonString = "";
        if (_jsonString != null)
            jsonString = _jsonString;



        if (jsonString.Length < 10f)
        {
            SaveSettings();
            return;
        }

        JsonUtility.FromJsonOverwrite(jsonString, SaveObject);

        if (SaveObject.SaveVersion != SAVE_VERSION)
            return;

        MapGlobals.Instance.SaveSystem.SetJson(SaveObject.SaveData);
        SettingsLoaded?.Invoke();
    }

    public void BuySkinVideo(string hashKey)
    {
        _storedHashKey = hashKey;
        OperationHandler = BuySkinVideoCallback;
        YandexGame.RewVideoShow(1);
    }

    private void BuySkinVideoCallback(int id)
    {
        if (id != 1)
            return;

        if (_storedHashKey == "")
            return;

        if (!SaveObject.AvialableSkins.Contains(_storedHashKey))
            SaveObject.AvialableSkins.Add(_storedHashKey);

        SkinBuyCompleted?.Invoke(_storedHashKey);

        _storedHashKey = "";
        OperationHandler = Void;
        MapGlobals.Instance.Save();
    }

    public bool CheckSkinAvialable(string hashKey)
    {
        return SaveObject.AvialableSkins.Contains(hashKey);
    }

    internal void BuySkinRuby(string hashKey, int costRuby)
    {
        if (TradeLot.Pay(MapGlobals.Instance.Player, MapGlobals.Instance.DefaultOneRubyLot, costRuby))
        {
            if (!SaveObject.AvialableSkins.Contains(hashKey))
                SaveObject.AvialableSkins.Add(hashKey);
            SkinBuyCompleted?.Invoke(hashKey);
            MapGlobals.Instance.Save();
        }
        else
            ShowNoRubyPanel();
    }

    public void ShowNoRubyPanel()
    {
        Debug.Log("No ruby");
        throw new Exception("Not implemented Ruby buy panel");
    }

    internal void BuyBuildingGradeRuby(Building building)
    {
        if (building.IsMaximumLevel)
            return;

        if (TradeLot.Pay(MapGlobals.Instance.Player, MapGlobals.Instance.DefaultOneRubyLot, building.CalculateUpgradePrice(building._currentGrade + 1)))
        {
            building.FreeUpgrade();
            OperationHandler = Void;
            BuildingBuyCompleted?.Invoke(building);
        }
        else
            ShowNoRubyPanel();

        MapGlobals.Instance.Save();
    }

    internal void DropCostBuildingGradeVideo(Building building)
    {
        if (building.IsMaximumLevel)
            return;

        _storedBuilding = building;
        OperationHandler = BuyBuildingGradeVideoCallback;
        YandexGame.RewVideoShow(0);
    }

    internal void BuyBuildingGradeVideoCallback(int id)
    {
        if (id != 0)
            return;


        if (_storedBuilding == null)
            return;

        _storedBuilding.DropCost();
        BuildingBuyCompleted?.Invoke(_storedBuilding);
        OperationHandler = Void;
        _storedBuilding = null;

        MapGlobals.Instance.Save();
    }
}

[System.Serializable]
public class UserSaves
{
    public int SaveVersion = 1;
    public List<string> AvialableSkins;
    public float MusicLevel = 0.95f, SoundLevel = 0.95f;
    public bool ExtendedGraphics = true, ExtendedParticles = true;

    public string SaveData;
}

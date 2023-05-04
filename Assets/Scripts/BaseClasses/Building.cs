using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Building : ResourceContainer, ISavable
{
    [SerializeField] private  int _nextUpgradeCostOverride = -1;
    [SerializeField] private string _buildingName;
    [SerializeField] internal int _currentGrade = 0;
    [SerializeField] private Transform _suckerEndPoint;
    [SerializeField] private BuildingTimer _buildingTimer;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _popupPoint;
    [SerializeField] private GameObject _visual;
    [SerializeField] private GameObject _objectsForDestroy;


    [Header("GradeSettings")]

    [SerializeField] private TradeLot _buildLot;
    [SerializeField] private int _maxGradeLevel;
    [SerializeField] private AnimationCurve _gradeCostCurve;

    [Header("Fabricator")]
    [SerializeField] private bool _canFabricate = false;
    [SerializeField] private TradeLot _fabricatedLot;
    [SerializeField] private AnimationCurve _fabricatorTimeCurve;
    [SerializeField] private AnimationCurve _fabricatorMultiplierCurve;

    [Header("Miner")]
    [SerializeField] private bool _canMine = false;
    [SerializeField] private AnimationCurve _mineTimeCurve;
    [SerializeField] private List<ResourceObject> _miningResources;
    [SerializeField] private AnimationCurve _miningResourcesMultiplier;

    [Header("NPC House")]
    [SerializeField] private bool _canSpawnNPC = false;
    [SerializeField] private Npc _nPC;
    [SerializeField] private string _breakableType = "Tree";
    [SerializeField] private AnimationCurve _npcCountCurve;
    [SerializeField] private AnimationCurve _npcSpeedCurve;


    internal List<Npc> Npcs = new List<Npc>();

    public UnityEvent<int> BuildingUpgraded;
    public UnityEvent BuildingBuilded;
    public UnityEvent BuildingMineActionCompleted;

    public string BuildingName => _buildingName;
    public bool CanSpawnNPC => _canSpawnNPC;

    Vector3 PopupStartPosition => _popupPoint != null ? _popupPoint.position : GetWorldPosition() + Vector3.up * 2f;
    public bool IsMaximumLevel => (_currentGrade >= _maxGradeLevel);
    //=======================================================================================================================================
    //=======================================================================================================================================
    //=======================================================================================================================================

    public int CalculateFabricatedResourceMultiplier(int level)
    {
        if (level == 0)
            return 0;
        return Mathf.RoundToInt(_fabricatorMultiplierCurve.Evaluate(level));
    }

    public int CalculateMinerResourceMultiplier(int level)
    {
        if (level == 0)
            return 0;
        return Mathf.RoundToInt(_miningResourcesMultiplier.Evaluate(level));
    }

    public float CalculateFabricationTime(int level)
    {
        if (level == 0)
            return 0;

        return _fabricatorTimeCurve.Evaluate(level);
    }

    public int CalculateUpgradePrice(int level)
    {
        if (level == 0)
            return 0;

        if (_nextUpgradeCostOverride > 0)
            return _nextUpgradeCostOverride;

        return Mathf.RoundToInt(_gradeCostCurve.Evaluate(level)) * 10;
    }

    public float CalculateMineTime(int level)
    {
        if (level == 0)
            return 0;

        return _mineTimeCurve.Evaluate(level);
    }


    public int CalculateNPCCount(int level)
    {
        if (level == 0)
            return 0;

        return Mathf.RoundToInt(_npcCountCurve.Evaluate(level));
    }

    public float CalculateNPCSpeed(int level)
    {
        if (level == 0)
            return 0;

        return _npcCountCurve.Evaluate(level);

    }


    //=======================================================================================================================================
    //=======================================================================================================================================
    //=======================================================================================================================================

    public void SetID(int id)
    {
        SaverID = id;
    }

    private void Update()
    {
        if (_canSpawnNPC)
        {
            var speed = CalculateNPCSpeed(_currentGrade);
            foreach (var item in Npcs)
            {
                item.SetSpeed(speed);
                RefreshTarget(item);
            }
        }

        if (_currentGrade != 0)
            return;
        TryToBuild();
    }


    public virtual void Start()
    {
        _visual.SetActive(false);
        AddPlaceholdersForNeedResources();
    }

    public void AddPlaceholdersForNeedResources()
    {

        if (_buildLot.Price != null)
        {
            if (_buildLot.Price.Count > 0)
            {
                foreach (var item in _buildLot.Price)
                {
                    TradeLot.AddResource(item.ResourceType, this, 0);
                }
            }
            TradeLot.AddResource(MapGlobals.Instance.DefaultOneRubyLot.Price[0].ResourceType, this, 0);
        }
    }

    public override Vector3 GetWorldPosition()
    {
        if (_suckerEndPoint)
        {
            return _suckerEndPoint.position;
        }
        else
        {
            return base.GetWorldPosition();
        }
    }

    public List<ResourceObject> NeedResources
    {
        get
        {
            if (IsMaximumLevel)
                return null;

            if (_currentGrade == 0)
            {
                return _buildLot.Price;
            }
            else
            {
                return TradeLot.Multiply(MapGlobals.Instance.DefaultOneRubyLot.Price, CalculateUpgradePrice(_currentGrade + 1));
            }

            return null;
        }
    }


    public virtual void OnUpgrade(bool silent = false)
    {
        if (!silent)
        {
            SoundMaker.Instance.PlaySound(SoundMaker.SoundType.Upgrade, GetWorldPosition());

            if (_currentGrade == 1)
            {
                BuildingBuilded?.Invoke();
            }
            else
            {
                BuildingUpgraded?.Invoke(_currentGrade);
            }
        }


        MapGlobals.SaveTimered();

        if (_currentGrade != 0)
        {
            _visual.SetActive(true);
            if (_objectsForDestroy != null)
                Destroy(_objectsForDestroy);
        }

        StopAllCoroutines();

        if (_canSpawnNPC)
            SpawnNpcs(CalculateNPCCount(_currentGrade));

        if (_canFabricate)
            StartCoroutine(FabricationRoutine());

        if (_canMine)
            StartCoroutine(MiningRoutine());
    }


    private IEnumerator FabricationRoutine()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            while (_fabricatedLot == null)
                yield return new WaitForSeconds(0.5f);

            var fabricationTime = CalculateFabricationTime(_currentGrade);
            
            if (fabricationTime > 0.05f)
            {
                if (TradeLot.CheckExist(_fabricatedLot.Price, MapGlobals.Instance.Player.ResourceObjects, 1))
                {
                    if (_buildingTimer != null)
                        _buildingTimer.SetTime(fabricationTime);
                    yield return new WaitForSeconds(fabricationTime);
                    FabricateResource(MapGlobals.Instance.Player, _fabricatedLot, Mathf.Min(MapGlobals.Instance.Player.ResourceObjects / _fabricatedLot, CalculateFabricatedResourceMultiplier(_currentGrade)));
                    BuildingMineActionCompleted?.Invoke();
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

        }
    }


    private IEnumerator MiningRoutine()
    {
        while (true)
        {
            var timeToMine = CalculateMineTime(_currentGrade);
            if (timeToMine > 0.05f)
            {
                if (_buildingTimer != null)
                    _buildingTimer.SetTime(timeToMine);
                yield return new WaitForSeconds(timeToMine);
                MineResources();
                BuildingMineActionCompleted?.Invoke();
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

        }
    }

    public void TryToBuild()
    {
        if (NeedResources == null)
            return;

        if (TradeLot.CheckExist(NeedResources, ResourceObjects, 1))
        {
            TradeLot.Pay(this, _buildLot, 1);
            _currentGrade++;
            OnUpgrade();
        }
    }

    internal void Reinit()
    {
        AddPlaceholdersForNeedResources();
        if (_currentGrade > 0)
            OnUpgrade();
    }

    internal virtual int GetFabricationSpeedOnUpgrade()
    {
        if (_currentGrade >= _maxGradeLevel)
            return 0;
        float speed = CalculateFabricationTime(_currentGrade) / CalculateFabricationTime(_currentGrade + 1);
        return Mathf.CeilToInt((speed - 1) * 100f);
    }

    internal void DropCost()
    {
        if (IsMaximumLevel)
            return;

        var cost = (float)CalculateUpgradePrice(_currentGrade + 1);

        cost *= 0.5f;

        _nextUpgradeCostOverride = Mathf.CeilToInt(cost);
    }




    [ContextMenu("Upgrade Building")]
    public void FreeUpgrade()
    {
        if (IsMaximumLevel)
            return;

        _currentGrade++;
        OnUpgrade();
        _nextUpgradeCostOverride = -1;
    }

    public void RefreshTarget(Npc npc)
    {
        if (!npc.NeedTarget)
            return;

        var targets = MapGlobals.Instance.GetAll<OnMapResource>(npc.GetWorldPosition(), true);

        foreach (var item in targets)
        {
            if (item.BreakableType != _breakableType)
            {
                Debug.Log($"Target not Finded {item.BreakableType} <=> {_breakableType}");
                continue;
            }

            Debug.Log("TargetFinded");

            if (!NpcContainsTarget(item as ResourceContainer))
            {
                npc.SetDistanation(item as ResourceContainer);
            }
        }
    }



    public bool NpcContainsTarget(ResourceContainer container)
    {
        foreach (var item in Npcs)
        {
            if (item != null)
                if (item.Distanation == container)
                    return true;
        }
        return false;
    }

    public void SpawnNpcs(int count)
    {
        while (Npcs.Count < count)
        {
            var pidor = Instantiate(_nPC, _spawnPoint.position, _nPC.transform.rotation);
            Npcs.Add(pidor);
            pidor.Owner = this;
        }
    }

    public virtual void FabricateResource(ResourceContainer container, TradeLot lot, int multiplier)
    {
        if (lot == null)
            return;

        Debug.Log($"Fabricator make with multiplier {multiplier}");

        if (TradeLot.Trade(container, container, lot, multiplier))
            FabricatePopup(lot, multiplier);

        //   MapGlobals.SaveTimered();
    }


    public virtual void MineResources()
    {
        TradeLot.AddResources(_miningResources, MapGlobals.Instance.Player, CalculateMinerResourceMultiplier(_currentGrade));
        MinePopup(_miningResources, CalculateMinerResourceMultiplier(_currentGrade));
    }



    private void MinePopup(List<ResourceObject> resources, int multiplier)
    {
        int x = 0;
        foreach (var item in resources)
        {
            PopupVisualPool.Instance.Pull(PopupStartPosition + Vector3.right * x * 2, item.ResourceType.Icon, item.Count * multiplier);
            x++;
        }
        return;
    }

    private void FabricatePopup(TradeLot lot, int multiplier)
    {
        int x = 0;
        foreach (var item in lot.Price)
        {
            PopupVisualPool.Instance.Pull(PopupStartPosition + Vector3.right * x * 2, item.ResourceType.Icon, -item.Count * multiplier, Color.red);
            x++;
        }

        foreach (var item in lot.Reward)
        {
            PopupVisualPool.Instance.Pull(PopupStartPosition + Vector3.right * x * 2, item.ResourceType.Icon, item.Count * multiplier, Color.green);
            x++;
        }
        return;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetWorldPosition(), 0.2f);
        Gizmos.color = Color.red;
        if (_spawnPoint) Gizmos.DrawWireSphere(_spawnPoint.position, 0.2f);
    }

    public bool IsDynamic => false;
    public int ID { get => SaverID; set => SetID(value); }
    public bool CanSpawnNPC1 { get => _canSpawnNPC; set => _canSpawnNPC = value; }

    public int SaverID;


    public string GetJson()
    {
        SaveData data = new SaveData();
        //==========================
        data.Level = _currentGrade;
        data.ResourceCounts = new List<int>();
        data.ResourceTypes = new List<ResourceType>();
        for (int i = 0; i < ResourceObjects.Count; i++)
        {
            data.ResourceTypes.Add(ResourceObjects[i].ResourceType);
            data.ResourceCounts.Add(ResourceObjects[i].Count);
        }

        data.NextUpgradeCost = _nextUpgradeCostOverride;
        //==========================
        return JsonUtility.ToJson(data);

    }

    public void SetJson(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        //==========================
        _currentGrade = data.Level;
        if (_currentGrade != 0)
            OnUpgrade(true);

        for (int i = 0; i < data.ResourceTypes.Count; i++)
        {
            TradeLot.AddResource(data.ResourceTypes[i], this, data.ResourceCounts[i]);
        }

        _nextUpgradeCostOverride = data.NextUpgradeCost;
        //==========================
    }

    public struct SaveData
    {
        public int Level;
        public List<ResourceType> ResourceTypes;
        public List<int> ResourceCounts;
        public int NextUpgradeCost;
    }
}


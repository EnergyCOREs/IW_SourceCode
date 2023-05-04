using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGlobals : MonoBehaviour
{
    public static MapGlobals Instance { get; private set; }

    [SerializeField] private UpgradeVisual _upgradeVisual;
    [SerializeField] private TraderVisual _traderVisual;
    [SerializeField] private TradeLot defaultOneRubyLot;
    [SerializeField] private SkinSelector _skinSelector;
    [SerializeField] private Camera _worldCameraComponent;

    private SimpleCharacterController _player;
    private Transform _worldCamera;

    public UserBusinessLogic UserBusinessLogic { get; internal set; } = new UserBusinessLogic();
    public SaveSystem SaveSystem { get; internal set; } = new SaveSystem();
    public YGAdHelper YGAdHelper { get; internal set; } = new YGAdHelper();
    public List<TradeLot> AvialableLots = new List<TradeLot>();
    public SimpleCharacterController Player { get => _player; internal set => _player = value; }
    public Transform WorldCamera { get => _worldCamera; internal set => _worldCamera = value; }
    public Camera WorldCameraComponent { get => _worldCameraComponent; internal set => _worldCameraComponent = value; }
    public AnimatedTabsSys AnimatedTabsSys { get; internal set; }
    public UpgradeVisual UpgradeVisual { get => _upgradeVisual; internal set => _upgradeVisual = value; }
    public TraderVisual TraderVisual { get => _traderVisual; internal set => _traderVisual = value; }
    public SkinSelector SkinSelector { get => _skinSelector; internal set => _skinSelector = value; }
    public TradeLot DefaultOneRubyLot { get => defaultOneRubyLot; internal set => defaultOneRubyLot = value; }

    public List<IPlayerTarget> PlayerTargets = new List<IPlayerTarget>();



    private void ClearUnused() => PlayerTargets.RemoveAll(item => IsNullOrDestroyed(item));

    [ContextMenu("Save Settings")]
    public void Save() => UserBusinessLogic.SaveSettings();

    [ContextMenu("PrepareForSave")]
    public void PrepareForSave() => SaveSystem.PrepareIDs();

    public static void SaveTimered()
    {
        if (Instance != null)
            Instance.UserBusinessLogic.SaveTimer(5f);
    }

    private void Update()
    {
        UserBusinessLogic.Tick(Time.deltaTime);
    }

    public int GetPlayerRubyCount()
    {
        var finded = TradeLot.FindResource(Player.ResourceObjects, DefaultOneRubyLot.Price[0].ResourceType);
        if (finded == null)
            return 0;

        return finded.Count;
    }

    private void Awake()
    {
        Instance = this;
        Player = FindObjectOfType<SimpleCharacterController>();
        WorldCameraComponent = FindObjectOfType<Camera>();
        WorldCamera = WorldCameraComponent.transform;
        StartCoroutine(LateLoad());
        StartCoroutine(AutoSave());
    }

    IEnumerator LateLoad()
    {
        yield return new WaitForSeconds(0.5f);
        UserBusinessLogic.ForceLoad();
    }

    IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);
            UserBusinessLogic.SaveSettings();
        }
    }

    internal void AddToList(IPlayerTarget targetObject)
    {
        PlayerTargets.Add(targetObject);
    }

    public IPlayerTarget GetNearest(Vector3 position, float searchDistance)
    {
        ClearUnused();
        var type = typeof(IPlayerTarget);
        var maxDistance = float.MaxValue;
        IPlayerTarget candidate = null;
        foreach (var item in PlayerTargets)
        {
            //  Debug.Log("Fuck!");
            var magnitude = (item.GetWorldPosition() - position).sqrMagnitude;
            if (magnitude < maxDistance)
            {
                maxDistance = magnitude;
                candidate = item;
            }

        }

        if (candidate == null)
            return null;

        if (maxDistance > searchDistance * searchDistance)
            return null;

        return candidate;
    }

    public static bool IsNullOrDestroyed(System.Object obj)
    {

        if (object.ReferenceEquals(obj, null)) return true;

        if (obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;

        return false;
    }


    public T GetNearest<T>(Vector3 position, float searchDistance) where T : IPlayerTarget
    {
        ClearUnused();
        var type = typeof(T);
        var maxDistance = float.MaxValue;
        T candidate = default(T);
        foreach (var item in PlayerTargets)
        {

            if (item.GetType() != type)
                continue;

            var magnitude = (item.GetWorldPosition() - position).sqrMagnitude;
            if (magnitude < maxDistance)
            {
                maxDistance = magnitude;
                candidate = (T)item;
            }

        }

        if (candidate == null)
            return default(T);
        if (maxDistance > searchDistance * searchDistance)
            return default(T);
        return candidate;
    }

    public List<T> GetAll<T>(Vector3 position, bool sortByDistance) where T : IPlayerTarget
    {
        ClearUnused();
        var type = typeof(T);
        List<T> list = new List<T>();
        foreach (var item in PlayerTargets)
        {
            if (item.GetType() != type)
                continue;

            if (item.IsDecorative == true)
                continue;

            list.Add((T)item);
        }

        if (sortByDistance)
        {
            list = list.OrderBy(x => (x.GetWorldPosition() - position).sqrMagnitude * -1f).ToList();
        }

        return list;
    }

    public List<IPlayerTarget> GetAll(Vector3 position, bool sortByDistance, Type type)
    {
        ClearUnused();
        List<IPlayerTarget> list = new List<IPlayerTarget>();
        foreach (var item in PlayerTargets)
        {
            if (item.GetType() != type)
                continue;

            list.Add(item);
        }

        if (sortByDistance)
        {
            list = list.OrderBy(x => (x.GetWorldPosition() - position).sqrMagnitude * -1f).ToList();
        }

        return list;
    }

}


public interface IPlayerTarget
{
    public System.Type Typeof { get; }
    public string BreakableType { get; }

    public TargetType UsedWeapon { get; }
    public bool IsDecorative { get; }

    public virtual Vector3 GetWorldPosition() { return Vector3.zero; }
    public void AddToTargetList();
    public enum TargetType
    {
        Sword,
        Axe,
        PickAxe,
        Collectable
    }
}
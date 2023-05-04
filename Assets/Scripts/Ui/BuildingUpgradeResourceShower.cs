
using UnityEngine;

public class BuildingUpgradeResourceShower : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private BuildingUpgradeResourceCell _cell;
    [SerializeField] private Building _building;

    private void Start()
    {
        foreach (var item in _building.NeedResources)
        {
            Instantiate(_cell, _container).Initialize(item.ResourceType, item.Count, _building);
        }
    }
}

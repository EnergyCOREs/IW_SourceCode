using System.Collections.Generic;
using UnityEngine;

public class InventoryShower : MonoBehaviour
{
    [SerializeField] private InventoryVisual _visualPrefab;
    [SerializeField] private ResourceContainer _container;
    [SerializeField] private Transform _rootObject;

    private int _count;
    private List<InventoryVisual> _visuals = new List<InventoryVisual>();

    public ResourceContainer Container
    {
        get => _container;
        set
        {
            _container = value;
            Regenerate();
        }
    }

    private void Start()
    {
        Regenerate();
        MapGlobals.Instance.UserBusinessLogic.SettingsLoaded += Regenerate;
    }

    private void Update()
    {
        if (_count != _container.ResourceObjects.Count)
        {
            Regenerate();
        }

        UpdateVisibility();
    }

    public void Regenerate()
    {
        Debug.Log("Regenerate");
        _count = _container.ResourceObjects.Count;

        for (int i = _rootObject.childCount-1; i >= 0; i--)
        {
            Destroy(_rootObject.GetChild(i).gameObject);
        }

        _visuals.Clear();

        if (_container == null)
            return;

        for (int i = 0; i < _container.ResourceObjects.Count; i++)
        {
            var visual = Instantiate(_visualPrefab, _rootObject).Init(_container.ResourceObjects[i]);
            _visuals.Add(visual);
            if (_container.ResourceObjects[i].ResourceType.Name == "Ruby")
                visual.transform.SetAsFirstSibling();

        }

        UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        foreach (var item in _visuals)
        {
            
            item.UpdateVisibility();
        }
    }

}

using UnityEngine;
using UnityEngine.UI;
public class BuildingUpgradeResourceCell : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Text _countText;
    private int _needed;
    private ResourceObject _resource;
    private Building _building;

    private void Update()
    {
        UpdateText((_resource != null) ? _resource.Count : 0, _needed);
    }

    public void UpdateIcon(Sprite sprite)
    {
        _icon.sprite = sprite;
    }

    public void UpdateText(int exist, int needed)
    {
        string color = exist >= needed ? "ffffff" : "ff0000";
        _countText.text = $"<color=#{color}ff>{TradeLot.ScoreFormat(exist)}</color>/{TradeLot.ScoreFormat(needed)}";
    }

    internal void Initialize(ResourceType resourceType, int count, Building building)
    {
        UpdateIcon(resourceType.Icon);
        _needed = count;
        _building = building;

        _resource = TradeLot.FindResource(_building.ResourceObjects, resourceType);

        UpdateText((_resource != null) ? _resource.Count : 0, _needed);
    }

}

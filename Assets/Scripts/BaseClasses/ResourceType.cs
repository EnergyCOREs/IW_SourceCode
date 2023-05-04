using UnityEngine;

[CreateAssetMenu(fileName = "ResourceType", menuName = "ScriptableObjects/ResourceType", order = 1)]
public class ResourceType : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameObject Prefab;
    public int Cost;
}


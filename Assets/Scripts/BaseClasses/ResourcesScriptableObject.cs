using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesScriptableObject", menuName = "ScriptableObjects/ResourcesScriptableObject", order = 1)]
public class ResourcesScriptableObject : ScriptableObject
{
    public ResourceType[] Resources;
}

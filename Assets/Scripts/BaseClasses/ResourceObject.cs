using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceObject
{
    public ResourceType ResourceType;
    public int Count;

    public ResourceObject(ResourceType resourceType, int count)
    {
        ResourceType = resourceType;
        Count = count;
    }
}

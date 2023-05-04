using System.Collections.Generic;
using UnityEngine;

public class ResourceContainer : MonoBehaviour
{
    public List<ResourceObject> ResourceObjects = new List<ResourceObject>();
    internal Transform _transform;

    public Transform MyTransform 
    {
        get
        {
            if (_transform == null)
                _transform = transform;
            return _transform;
        }
    }

    public virtual Vector3 GetWorldPosition()
    {
        return MyTransform.position;
    }
}

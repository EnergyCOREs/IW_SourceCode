using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSuckerPool : MonoBehaviour
{
    private static ResourceSuckerPool _instance;
    private Queue<ResourceSuckerVisual> poolObjects = new Queue<ResourceSuckerVisual>();
    public ResourceSuckerVisual Prefab;
    public static ResourceSuckerPool Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ResourceSuckerPool>();
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        gameObject.SetActive(false);
    }

    public void Pull(Vector3 startPoint, Vector3 endpoint, Sprite icon)
    {
        if (poolObjects.Count == 0)
            Push(Instantiate(Prefab).Init(transform, MapGlobals.Instance.WorldCameraComponent));

        ResourceSuckerVisual visual = poolObjects.Dequeue();
        visual.Fly(startPoint, endpoint, icon);
    }

    public void Push(ResourceSuckerVisual visual)
    {
        poolObjects.Enqueue(visual);
    }
}

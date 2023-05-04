using System.Collections.Generic;
using UnityEngine;

public class PopupVisualPool : MonoBehaviour
{
    [SerializeField] private PopupVisual _prefab;

    private static PopupVisualPool _instance;
    private Transform _transform;
    private Queue<PopupVisual> poolObjects = new Queue<PopupVisual>();

    public static PopupVisualPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PopupVisualPool>();
                if (_instance == null)
                    Debug.LogError("3AE6AJI! TTOCTABb HA CLLEHU PopupVisualPool U HACTPOU HAXYU!");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        _transform = transform;
        gameObject.SetActive(false);
    }

    public void Pull(Vector3 startPoint, Sprite icon, int count, Color color)
    {
        if (poolObjects.Count == 0)
            Push(Instantiate(_prefab).Init(transform, MapGlobals.Instance.WorldCameraComponent));

        PopupVisual visual = poolObjects.Dequeue();
        visual.Fly(startPoint, count, icon, color);
        if (count > 0)
            SoundMaker.Instance.PlaySound(SoundMaker.SoundType.Pop, startPoint);
    }

    public void Pull(Vector3 startPoint, Sprite icon, int count)
    {
        Pull(startPoint, icon, count, Color.green);
    }

    public void Push(PopupVisual visual)
    {
        poolObjects.Enqueue(visual);
    }
}

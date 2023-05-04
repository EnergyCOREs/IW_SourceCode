using UnityEngine;
using UnityEngine.UI;

public class ResourceSuckerVisual : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private float _height = 1;

    private Vector3 _startPoint;
    private Vector3 _endPoint;

    private float _timer = -1;
    private Transform _owner;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public ResourceSuckerVisual Init(Transform owner, Camera camera)
    {
        _owner = owner;
        _canvas.worldCamera = camera;
        return this;
    }

    public void Fly(Vector3 startPoint, Vector3 endPoint, Sprite icon)
    {
        if (_transform == null)
            Awake();

        _icon.sprite = icon;
        _transform.SetParent(null);
        _startPoint = startPoint;
        _endPoint = endPoint;
        _timer = 1f;
    }


    private void Update()
    {
        if (_timer >= 0)
        {
            _timer -= Time.deltaTime * 2f;
            _transform.LookAt(MapGlobals.Instance.WorldCamera);
            _transform.position = Vector3.Lerp(_endPoint, _startPoint, _timer) + _height * Vector3.up * Mathf.Sin(_timer * Mathf.PI);

            if (_timer < 0)
            {
                _transform.SetParent(_owner);
                ResourceSuckerPool.Instance.Push(this);
            }
        }
    }
}

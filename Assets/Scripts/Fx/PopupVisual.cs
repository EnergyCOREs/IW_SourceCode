using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupVisual : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _scalableContainer;
    [SerializeField] private float _antigravity = 1;
    [SerializeField] private float _startSpeed = 1;

    private float _timer;
    private Transform _owner;
    private Transform _transform;
    private Vector3 _startScale = Vector3.one;
    private float _velocity;

    private void Awake()
    {
        _transform = transform;
        //  _startScale = _transform.localScale;
    }

    public PopupVisual Init(Transform owner, Camera camera)
    {
        _owner = owner;
        _canvas.worldCamera = camera;
        return this;
    }

    internal void Fly(Vector3 startPoint, int count, Sprite icon, Color color)
    {
        if (_transform == null)
            Awake();

        _velocity = _startSpeed;
        _icon.sprite = icon;
        var countSign = count > 0 ? "+" : "";
        _countText.text = $"{countSign}{count}";
        _transform.name = _countText.text;
        _countText.color = color;
        _transform.SetParent(null);
        _scalableContainer.localScale = _startScale;
        _transform.position = startPoint;
        _transform.eulerAngles = new Vector3(0, 180, 0);

        _timer = 1f;
    }

    private void Update()
    {
        if (_timer >= 0)
        {
            _velocity += _antigravity * Time.deltaTime;
            _timer -= Time.deltaTime * 1.5f;
            // _transform.LookAt(MapGlobals.Instance.WorldCamera);
            _transform.position += Vector3.up * _velocity;


            if (_timer < 0.25f)
                _scalableContainer.localScale = _startScale * Mathf.SmoothStep(0, 1, _timer * 4f);

            if (_timer < 0)
            {
                _transform.SetParent(_owner);
                PopupVisualPool.Instance.Push(this);
            }

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthShower : MonoBehaviour
{
    [SerializeField] private Gradient _progressGradient;
    [SerializeField] private Breakable _target;
    [SerializeField] private Image _holderBar, _healthBar;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private float _impactRandomPush = 0.5f;

    private RectTransform _canvasRect;
    private float _maxHealth;
    private float _startScale;
    private float _scale;
    private Vector3 _canvasStartPosition;

    private void Awake()
    {
        _maxHealth = _target.Health;
        _target.Damaged = UpdateUI;
        _canvasRect = _canvas.transform as RectTransform;
        _startScale = _canvasRect.localScale.x;
        _holderBar.fillAmount = 0;
        _healthBar.fillAmount = 0;
    }

    private void Start()
    {
        _canvas.worldCamera = MapGlobals.Instance.WorldCameraComponent;
        _canvasStartPosition = _canvasRect.position;
    }

    private void UpdateUI()
    { 
        _scale = _startScale * 1.2f;
        _canvasRect.position = _canvasRect.position + new Vector3(Random.Range(-_impactRandomPush, _impactRandomPush), 0, Random.Range(-_impactRandomPush, _impactRandomPush));
        _holderBar.fillAmount = 1 - _target.Health / _maxHealth;
        _healthBar.fillAmount = _target.Health / _maxHealth;
        _holderBar.color = _progressGradient.Evaluate(1 - (_target.Health / _maxHealth));
    }

    private void Update()
    {
        _scale = Mathf.Lerp(_scale, _startScale, Time.deltaTime * 5f);
        _canvasRect.localScale = Vector3.one * _scale;
        _canvasRect.position = Vector3.Lerp(_canvasRect.position, _canvasStartPosition, Time.deltaTime * 15f);
    }

    private void OnDestroy()
    {
        _target.Damaged -= UpdateUI;
    }

}

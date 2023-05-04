using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryVisual : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;
    private ResourceObject _currentTarget;
    private int _hashedCount;
    private GameObject _gameObject;
    private Animator _animator;

    public InventoryVisual Init(ResourceObject target)
    {
        _icon.sprite = target.ResourceType.Icon;
        _text.text = target.Count.ToString();
        _currentTarget = target;
        _gameObject = gameObject;
        _animator = GetComponent<Animator>();
        return this;
    }

    private void UpdateVisual()
    {
        ClearIfEmptyTarget();

        if (_currentTarget.Count > _hashedCount)
            if (_animator != null)
                _animator.SetTrigger("Pop");

        _hashedCount = Mathf.Max(1, _currentTarget.Count);
        _text.text = _currentTarget.Count.ToString();
    }

    internal void UpdateVisibility()
    {
        ClearIfEmptyTarget();



        if (_hashedCount != _currentTarget.Count)
        {
            _gameObject.SetActive(_currentTarget.Count > 0);

            if (_currentTarget.Count <= 0)
                return;
        }

        UpdateVisual();

    }

    internal void ClearIfEmptyTarget()
    {
        if (_currentTarget as ResourceObject == null)
        {
            Destroy(gameObject);
            return;
        }
    }
}

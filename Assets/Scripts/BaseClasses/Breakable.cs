using UnityEngine;

public abstract class Breakable : ResourceContainer
{
    [SerializeField] private float _health = 100;
    [SerializeField] private GameObject _destroyedtPrefab;
    [SerializeField] private Transform _popupPoint;

    public System.Action Damaged;

    Vector3 PopupStartPosition => _popupPoint != null ? _popupPoint.position : GetWorldPosition() + Vector3.up * 2f;
    public float Health => _health;

    public virtual void TakeDamage(ResourceContainer damager, float damage)
    {
        if (_health > 0)
        {
            _health -= damage;
            if (_health <= 0)
            {
                Break(damager);
            }
        }

        Damaged?.Invoke();
    }

    public void Break(ResourceContainer damager)
    {
        PopupVisualPool.Instance.Pull(PopupStartPosition, ResourceObjects[0].ResourceType.Icon, ResourceObjects[0].Count);
        TradeLot.AddResources(ResourceObjects, damager);
        OnBreak();

        if (_destroyedtPrefab != null)
            Instantiate(_destroyedtPrefab, transform.position, _destroyedtPrefab.transform.rotation);

        Destroy(gameObject);
    }

    public abstract void OnBreak();
}

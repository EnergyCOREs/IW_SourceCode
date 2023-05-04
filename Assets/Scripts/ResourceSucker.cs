using UnityEngine;

public class ResourceSucker : MonoBehaviour
{
    [SerializeField] private Building _building;

    private float _cooldownTimer;

    private void OnTriggerStay(Collider other)
    {
        if (_cooldownTimer > 0)
            return;

        if (other.GetComponent<SimpleCharacterController>() == null)
            return;
        Debug.Log("try to suck");

        var needResources = _building.NeedResources;

        if (needResources == null)
            return;

        Debug.Log("needResources != null");

        if (needResources.Count == 0)
            return;

        Debug.Log($"needResources.Count == {needResources.Count}");

        foreach (var needitem in needResources)
        {
            var item = TradeLot.FindResource(MapGlobals.Instance.Player.ResourceObjects, needitem.ResourceType);
            if (item == null)
            {
                Debug.Log($"Cannot find {needitem.ResourceType.Name}");
                continue;
            }

            if (item.Count <= 0)
                continue;

            var needCount = TradeLot.GetNeedCount(needitem, TradeLot.FindResource(_building.ResourceObjects, item.ResourceType));
            Debug.Log($"needCount == {needCount}");
            if (needCount > 10)
                needCount = 10;

            if (needCount >= 1)
            {
                var maxCount = Mathf.Min(needCount, item.Count);
                item.Count -= maxCount;
                TradeLot.AddResource(item.ResourceType, _building, maxCount);

                ResourceSuckerPool.Instance.Pull(MapGlobals.Instance.Player.GetWorldPosition(), _building.GetWorldPosition(), item.ResourceType.Icon);
                SoundMaker.Instance.PlaySound(SoundMaker.SoundType.Pop, MapGlobals.Instance.Player.GetWorldPosition());
                _cooldownTimer = 0.05f;

                return;
            }
           // Debug.Log($"Cannot find {needitem.ResourceType.Name}");


        }

    }

    private void Update()
    {
        _cooldownTimer -= Time.deltaTime;
    }
}

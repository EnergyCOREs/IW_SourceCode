using UnityEngine;

public class BuildingUpgradeTrigger : MonoBehaviour
{
    [SerializeField] private Building _building;

    private void OnTriggerEnter(Collider other)
    {
        SetPage(other, true);
    }
    private void OnTriggerExit(Collider other)
    {
        SetPage(other, false);
    }

    private void SetPage(Collider other, bool state)
    {
        if (other.GetComponent<SimpleCharacterController>() == null)
            return;

        if (_building == null)
            return;

        if (_building._currentGrade == 0)
            return;

        var visual = MapGlobals.Instance.UpgradeVisual;
        if (visual == null)
            return;

        if (state)
            visual.Reinit(_building);

        MapGlobals.Instance.AnimatedTabsSys.SetPage(3, state);
    }


}

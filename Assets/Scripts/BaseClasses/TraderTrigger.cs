using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderTrigger : MonoBehaviour
{
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

        var visual = MapGlobals.Instance.TraderVisual;
        if (visual == null)
            return;


        visual.Reinit();

        MapGlobals.Instance.AnimatedTabsSys.SetPage(2, state);
    }
}

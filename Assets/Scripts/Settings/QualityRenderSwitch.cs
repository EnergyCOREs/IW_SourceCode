using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���� ������ ����� �� �
/// </summary>
public class QualityRenderSwitch : MonoBehaviour
{

    public void SetChildState(bool st)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(st);
        }
    }
}

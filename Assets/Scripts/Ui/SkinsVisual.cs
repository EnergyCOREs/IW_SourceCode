using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkinsVisual : MonoBehaviour
{
    [SerializeField] private SkinsScriptableObject skins;
    [SerializeField] private RectTransform _container;
    [SerializeField] private SkinLine _prefab;


    private void Start()
    {
        UserBusinessLogic businessLogic = MapGlobals.Instance.UserBusinessLogic;

        foreach (var item in skins.AllSkins)
        {
            Instantiate<SkinLine>(_prefab, _container).Init(item, businessLogic);
        }
    }
}

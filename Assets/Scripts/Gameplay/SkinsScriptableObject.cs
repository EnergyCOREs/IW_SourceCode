using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinsDatabase", menuName = "ScriptableObjects/SkinsDatabase", order = 1)]
public class SkinsScriptableObject : ScriptableObject
{
    public List<SkinListing> AllSkins;
    public SkinListing GetSkinByKey(string key) => AllSkins.Find(a => a.HashKey == key);
    public SkinListing GetSkinByName(string skinName) => AllSkins.Find(a => a.Name == skinName);

    public void RegenerateAllEmptyKeys()
    {
        foreach (var item in AllSkins)
        {
            item.SetTrashIfEmpty();
        }
    }
}

using UnityEngine;

public class SkinSelector : MonoBehaviour
{

    [SerializeField] private SkinsScriptableObject _skinList;
    [SerializeField] private  Renderer[] _rendererList;

    [Header("UI")]
    [SerializeField] private  RectTransform _listPlacement;
    [SerializeField] private  GameObject _linePrefab;
    [SerializeField] private  string _currentSkin; 

    private Material _skinMaterial;
    private int _skinID = 0;

    public string CurrentSkin => _currentSkin;

    private void Awake()
    {
        _skinMaterial = _rendererList[0].material;
    }

    public void SetSkin(string hashKey)
    {
        _currentSkin = hashKey;
        var skin = _skinList.AllSkins.Find(a => a.HashKey == hashKey);
        if (skin != null)
        {
            if (skin.Skintexture != null)
            {
                _skinMaterial.mainTexture = skin.Skintexture;
                for (int i = 0; i < _rendererList.Length; i++)
                {
                    _rendererList[i].sharedMaterial = _skinMaterial;
                }
            }
            MapGlobals.SaveTimered();
        }
    }




    public void SetSkin()
    {
        _skinID++;
        if (_skinID > (_skinList.AllSkins.Count - 1)) _skinID = 0;

        if (_skinList.AllSkins[_skinID].Skintexture)
        {
            _skinMaterial.mainTexture = _skinList.AllSkins[_skinID].Skintexture;
            for (int i = 0; i < _rendererList.Length; i++)
            {
                _rendererList[i].sharedMaterial = _skinMaterial;
            }
        }
    }

}

[System.Serializable]
public class SkinListing : Hashed
{
    public Texture Skintexture;
    public int CostRuby = 0;
}


[System.Serializable]
public abstract class Hashed
{
    public string Name;
    public string HashKey;

    public static string GenerateTrash(int lenght)
    {
        string result = "";
        var random = new System.Random();
        while (result.Length < lenght)
        {
            char generated = (char)random.Next(33, 125);
            if (char.IsLetterOrDigit(generated))
                result += generated;
        }
        return result;
    }

    public void SetTrashIfEmpty()
    {
        if (HashKey.Length == 0)
            HashKey = GenerateTrash(16);
    }

    [ContextMenu("Set random key")]
    public void SetTrash()
    {
        HashKey = GenerateTrash(16);
    }
}
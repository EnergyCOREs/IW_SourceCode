using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[System.Serializable]
public struct SaveSystem
{
    public List<SaveLine> SaveLines;
    public string GetJson()
    {
        SaveLines = new List<SaveLine>();

        var objectsToSave = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ISavable>();

        foreach (var item in objectsToSave)
        {
            var json = item.GetJson();

            SaveLines.Add(new SaveLine(json, item.ID, item.IsDynamic));
            Debug.Log("SaveData added line");
        }

        Debug.Log(JsonUtility.ToJson(this));

        return JsonUtility.ToJson(this);
    }

    public void SetJson(string json)
    {
        var objectsToLoad = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToList();
        JsonUtility.FromJsonOverwrite(json, this);
        foreach (var item in SaveLines)
        {
            var finded = objectsToLoad.Find(a => a.ID == item.Id);
            if (finded.IsDynamic)
                continue;

            finded.SetJson(item.JsonString);
        }
    }

    public void PrepareIDs()
    {
#if UNITY_EDITOR
        int id = 1;
        var objectsToLoad = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ISavable>();

        foreach (var item in objectsToLoad)
        {
            EditorUtility.SetDirty(item as Object);
            item.ID = id++; 
        }
#endif
    }
}



public interface ISavable
{

    public bool IsDynamic { get; }
    public string GetJson();
    public int ID { get; set; }
    public void SetJson(string json);
}

[System.Serializable]
public struct SaveLine
{
    public string JsonString;
    public int Id;
    public bool IsDynamic;

    public SaveLine(string jsonString, int id, bool isDynamic)
    {
        JsonString = jsonString;
        Id = id;
        IsDynamic = isDynamic;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TradeLot", menuName = "ScriptableObjects/TradeLot", order = 1)]
public class TradeLot : ScriptableObject
{
    public List<ResourceObject> Price;
    public List<ResourceObject> Reward;

    public TradeLot ReInit(List<ResourceObject> price, List<ResourceObject> reward)
    {
        Price = price;
        Reward = reward;
        return this;
    }

    public static void AddResources(List<ResourceObject> resources, ResourceContainer reciever, int multiplier = 1)
    {
        foreach (var item in resources)
        {
            var finded = FindResource(reciever.ResourceObjects, item.ResourceType);
            if (finded != null)
            {
                finded.Count += item.Count * multiplier;
            }
            else
            {
                reciever.ResourceObjects.Add(new ResourceObject(item.ResourceType, item.Count * multiplier));
            }
        }
    }

    public static void AddResource(ResourceType resource, ResourceContainer reciever, int count = 1)
    {
        if (resource == null)
            return;

        if (resource.Icon == null)
            return;

        var finded = FindResource(reciever.ResourceObjects, resource);
        if (finded != null)
        {
            finded.Count += count;
        }
        else
        {
            reciever.ResourceObjects.Add(new ResourceObject(resource, count));
        }

    }

    internal static List<ResourceObject> Multiply(List<ResourceObject> price, int multiplier)
    {
        List<ResourceObject> newPrice = new List<ResourceObject>();
        foreach (var item in price)
        {
            newPrice.Add(new ResourceObject(item.ResourceType, item.Count * multiplier));
        }

        return newPrice;
    }

    public static bool Trade(ResourceContainer seller, ResourceContainer buyer, TradeLot lot, int multiplier)
    {
        Debug.Log("Check Exist");
        if (CheckExist(lot.Price, seller.ResourceObjects, multiplier))
        {
            Debug.Log("Exist");
            if (Pay(seller, lot, multiplier))
            {
                Debug.Log("Pay OK");
                AddResources(lot.Reward, buyer, multiplier);
                return true;
            }
        }

        return false;
    }

    public static bool Pay(ResourceContainer seller, TradeLot lot, int multiplier = 1)
    {
        if (CheckExist(lot.Price, seller.ResourceObjects, multiplier))
        {
            foreach (var item in seller.ResourceObjects)
            {
                var finded = FindResource(lot.Price, item.ResourceType);
                if (finded == null)
                    continue;
                item.Count -= finded.Count * multiplier;
            }

            return true;
        }

        return false;
    }

    public static bool CheckExist(ResourceObject needed, List<ResourceObject> exist, int multiplier = 1)
    {
        foreach (var item in exist)
        {
            if (item.ResourceType == needed.ResourceType)
                if (item.Count >= needed.Count * multiplier)
                    return true;
        }

        return true;
    }

    public static bool CheckExist(List<ResourceObject> needed, List<ResourceObject> exist, int multiplier = 1)
    {
        foreach (var item in needed)
        {
            var res = FindResource(exist, item.ResourceType);
            if (res == null)
                return false;
            if (res.Count < item.Count * multiplier)
                return false;
        }

        return true;
    }

    public static int GetNeedCount(ResourceObject needed, ResourceObject exist)
    {
        if (needed.ResourceType != exist.ResourceType)
            return 0;
        return (needed.Count - exist.Count) > 0 ? (needed.Count - exist.Count) : 0;
    }

    public static int FindResourceIndex(List<ResourceObject> resourceObjects, ResourceType resourceType)
    {
        for (int i = 0; i < resourceObjects.Count; i++)
        {
            if (resourceObjects[i].ResourceType == resourceType)
            {
                return i;
            }
        }

        return -1;
    }

    public static ResourceObject FindResource(List<ResourceObject> resourceObjects, ResourceType resourceType)
    {
        return resourceObjects.Find(a => a.ResourceType == resourceType);
    }

    internal static void GiveAll(ResourceContainer giver, ResourceContainer resourceContainer)
    {
        AddResources(giver.ResourceObjects, resourceContainer);
        giver.ResourceObjects = new List<ResourceObject>();
    }

    public static TradeLot operator *(TradeLot lot, int multiplier)
    {
        return Instantiate<TradeLot>(lot).ReInit(Multiply(lot.Price, multiplier), Multiply(lot.Reward, multiplier));
    }

    public static int operator /(List<ResourceObject> objects, TradeLot lot)
    {
        int maximum = int.MaxValue;

        if (lot.Price.Count == 0)
            return 0;

        foreach (var item in lot.Price)
        {
            var finded = objects.Find(a => a.ResourceType == item.ResourceType);
            if (finded == null)
            {
                return 0;
            }
            maximum = Math.Min(maximum, finded.Count / item.Count);
        }

        return maximum;

    }

    internal static int CalculateCount(List<ResourceObject> resourceObjects)
    {
        int count = 0;
        foreach (var item in resourceObjects)
        {
            if (item == null)
                continue;
            count += item.Count;
        }

        return count;
    }

    public static string ScoreFormat(int value)
    {
        float score = value;
        string result;
        string[] scoreNames = new string[] { "", "k", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
        int i;

        for (i = 0; i < scoreNames.Length; i++)
            if (score < 900)
                break;
            else score = Mathf.Floor(score / 100f) / 10f;

        if (score == Mathf.Floor(score))
            result = score.ToString() + scoreNames[i];
        else result = score.ToString("F1") + scoreNames[i];
        return result;
    }

    public static string ScoreShow(float value)
    {
        string result;
        string[] scoreNames = new string[] { "", "k", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
        int i;

        for (i = 0; i < scoreNames.Length; i++)
            if (value < 900)
                break;
            else value = Mathf.Floor(value / 100f) / 10f;

        if (value == Mathf.Floor(value))
            result = value.ToString() + scoreNames[i];
        else result = value.ToString("F1") + scoreNames[i];
        return result;
    }


}

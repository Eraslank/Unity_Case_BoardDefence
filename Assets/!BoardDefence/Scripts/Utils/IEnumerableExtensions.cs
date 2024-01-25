using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static T RandomItem<T>(this List<T> data)
    {
        if (data.Count <= 0)
            return default;
        return data[Random.Range(0, data.Count)];
    }
    public static List<int> FindAllIndex<T>(this List<T> data, System.Predicate<T> match)
    {
        var items = data.FindAll(match);
        List<int> indexes = new List<int>();
        foreach (var item in items)
        {
            indexes.Add(data.IndexOf(item));
        }

        return indexes;
    }

    public static List<T> Shuffle<T>(this List<T> data)
    {
        return data.OrderBy(a => Random.Range(0, int.MaxValue)).ToList();
    }

    public static T Pop<T>(this List<T> data)
    {
        if(data.Count <= 0)
            return default;

        var res = data.First();
        data.RemoveAt(0);
        return res;
    }
}

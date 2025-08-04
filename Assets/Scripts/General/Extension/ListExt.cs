using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public static class ListExt
{
    public static int RandomIndexWeighted(this List<int> list)
    {
        if ((list?.Count ?? 0) == 0)
            return 0;
        var totalWeight = list.Sum();
        var randomValue = UnityEngine.Random.Range(0, totalWeight);
        var curWeight = 0;
        for (var i = 0; i < list.Count; i++)
        {
            curWeight += list[i];
            if (curWeight >= randomValue)
            {
                return i;
            }
        }
        return 0;
    }

    public static T RandomItem<T>(this List<T> list, [CanBeNull] Func<T, bool> filter = null, [CanBeNull] Func<T, int> weightFunc = null)
    {
        filter ??= _ => true;
        if (weightFunc != null)
            return RandomItemWeighted(list, filter, weightFunc);
        var fList = list.Where(filter).ToList();
        return fList.Count == 0 ? default : fList[UnityEngine.Random.Range(0, fList.Count)];
    }
    static T RandomItemWeighted<T>(this List<T> list, [CanBeNull] Func<T, bool> filter, Func<T, int> weightFunc)
    {
        filter ??= _ => true;
        if ((list?.Count ?? 0) == 0)
            return default;

        var filteredList = list.Where(filter).ToList();
        if (filteredList.Count == 0)
            return default;

        var weights = filteredList.Select(weightFunc).ToList();
        var totalWeight = weights.Sum();
        var randomValue = UnityEngine.Random.Range(0, totalWeight);
        var curWeight = 0;

        for (var i = 0; i < filteredList.Count; i++)
        {
            if (curWeight >= randomValue)
            {
                return filteredList[i];
            }
            curWeight += weights[i];
        }

        return filteredList[^1];
    }

    [NotNull]
    public static LinkedListNode<T> At<T>(this LinkedList<T> list, int index)
    {
        // if ((list?.Count ?? 0) == 0)
        //     throw new NullReferenceException();
        // if (index < 0 || index >= list.Count)
        //     throw new IndexOutOfRangeException();
        var current = list.First;
        for (int i = 0; i < index; i++)
            current = current?.Next;
        return current;
    }
}

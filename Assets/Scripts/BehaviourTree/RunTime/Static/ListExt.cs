using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
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

        public static LinkedListNode<T> At<T>(this LinkedList<T> list, int index)
        {
            if ((list?.Count ?? 0) == 0)
                throw new NullReferenceException();
            if (index < 0 || index >= list.Count)
                throw new IndexOutOfRangeException();
            var current = list.First;
            for (int i = 0; i < index; i++)
                current = current!.Next;
            return current;
        }
    }
}
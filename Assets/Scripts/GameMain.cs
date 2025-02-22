using UnityEngine;
using System.Collections.Generic;
using System;
public enum UpdateP
{
    P0 = 0,
    P1 = 1,
    P2 = 2,
}
public class GameMain : Singleton<GameMain>
{
    MyFSM gameFSM;
    BattleSystem battleSystem;
    BattleSystem battleSystem2;
    BattleSystem battleSystem3;
    SimplePriorityQueue<Action<float>, UpdateP> updatesQ = new();
    Action<float>[] updatesArray;
    void Awake()
    {
        gameFSM = new MyFSM(typeof(WaitForStartState));
        battleSystem = new BattleSystem(UpdateP.P1);
        battleSystem2 = new BattleSystem(UpdateP.P2);
        battleSystem3 = new BattleSystem(UpdateP.P0);
    }

    public void AddUpdate(Action<float> action, UpdateP priority)
    {
        updatesQ.Enqueue(action, priority, out updatesArray);
    }
    public void RemoveUpdate(Action<float> action)
    {
        updatesQ.Remove(action, out updatesArray);
    }
    void Update()
    {
        foreach (var update in updatesArray)
        {
            update(Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RemoveUpdate(updatesArray[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RemoveUpdate(updatesArray[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RemoveUpdate(updatesArray[2]);
        }
    }
}


public static class SimplePriorityQueueExtension
{
    public static void Enqueue<T,P>(this SimplePriorityQueue<T,P> queue, T item, P priority, out T[] items)
    {
        queue.Enqueue(item, priority);
        items = new T[queue.Count];
        int index = 0;
        SimplePriorityQueue<T,P> copyQueue = new SimplePriorityQueue<T,P>();
        while (queue.TryFirst(out var first))
        {
            copyQueue.Enqueue(first, queue.GetPriority(first));
            queue.Dequeue();
        }
        foreach (var copyUpdate in copyQueue)
        {
            items[index++] = copyUpdate;
        }
        foreach (var it in copyQueue)
        {
            queue.Enqueue(it, copyQueue.GetPriority(it));
        }

    }
    public static void Remove<T,P>(this SimplePriorityQueue<T,P> queue, T item, out T[] items)
    {
        queue.Remove(item);
        items = new T[queue.Count];
        int index = 0;
        SimplePriorityQueue<T,P> copyQueue = new SimplePriorityQueue<T,P>();
        while (queue.TryFirst(out var first))
        {
            copyQueue.Enqueue(first, queue.GetPriority(first));
            queue.Dequeue();
        }
        foreach (var copyUpdate in copyQueue)
        {
            items[index++] = copyUpdate;
        }
        foreach (var it in copyQueue)
        {
            queue.Enqueue(it, copyQueue.GetPriority(it));
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMediateObject
{

}


public interface ICondition
{
    bool Check();
}

public class SimpleCondition : ICondition
{
    public SimpleCondition(Func<bool> condition)
    {
        this.condition = condition;
    }
    Func<bool> condition;
    public bool Check()
    {
        return condition();
    }
    public override bool Equals(object obj)
    {
        if(obj is SimpleCondition other)
        {
            return condition == other.condition;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return condition?.GetHashCode() ?? 0;
    }
}

public interface IResult
{
    bool CallResult();
}

public class SimpleResult : IResult
{
    public SimpleResult(Action action)
    {
        this.action = action;
    }
    Action action;
    public bool CallResult()
    {
        action();
        return true;
    }
    public override bool Equals(object obj)
    {
        if(obj is SimpleResult other)
        {
            return action == other.action;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return action?.GetHashCode() ?? 0;
    }
}

public class UpDateResult : IResult
{
    public UpDateResult(Action<float> action)
    {
        this.action = action;
    }
    Action<float> action;
    public bool CallResult()
    {
        action(Time.deltaTime);
        return true;
    }
    public override bool Equals(object obj)
    {
        if(obj is UpDateResult other)
        {
            return action == other.action;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return action?.GetHashCode() ?? 0;
    }
}

public class Mediater : IResult
{
    public Mediater()
    {
        checks.Clear();
        falseCBs.Clear();
        trueCBs.Clear();
    }

    SimplePriorityQueue<ICondition> checks = new();
    
    SimplePriorityQueue<IResult> falseCBs = new();
    SimplePriorityQueue<IResult> trueCBs = new();

    List<ICondition> failedChecks = new();
    public void AddCheck(Func<bool> condition, float priority)
    {
        SimpleCondition simpleCondition = new(condition);
        checks.Enqueue(simpleCondition, priority);
    }
    public bool RemoveCheck(Func<bool> condition)
    {
        SimpleCondition simpleCondition = new(condition);
        return checks.TryRemove(simpleCondition);
    }

    public void AddFalseCB(Mediater mediater, float priority)
    {
        falseCBs.Enqueue(mediater, priority);
    }
    public bool RemoveFalseCB(Mediater mediater)
    {
        return falseCBs.TryRemove(mediater);
    }
    public void AddFalseCB(Action action, float priority)
    {
        SimpleResult simpleResult = new(action);
        falseCBs.Enqueue(simpleResult, priority);
    }
    public bool RemoveFalseCB(Action action)
    {
        SimpleResult simpleResult = new(action);
        return falseCBs.TryRemove(simpleResult);
    }
    // public void AddFalseCB(Action<float> action, float priority)
    // {
    //     UpDateResult upDateResult = new(action);
    //     falseCBs.Enqueue(upDateResult, priority);
    // }
    // public bool RemoveFalseCB(Action<float> action)
    // {
    //     UpDateResult upDateResult = new(action);
    //     return falseCBs.TryRemove(upDateResult);
    // }

    public void AddTrueCB(Mediater mediater, float priority)
    {
        trueCBs.Enqueue(mediater, priority);
    }
    public bool RemoveTrueCB(Mediater mediater)
    {
        return trueCBs.TryRemove(mediater);
    }
    public void AddTrueCB(Action action, float priority)
    {
        SimpleResult simpleResult = new(action);
        trueCBs.Enqueue(simpleResult, priority);
    }
    public bool RemoveTrueCB(Action action)
    {
        SimpleResult simpleResult = new(action);
        return trueCBs.TryRemove(simpleResult);
    }
    // public void AddTrueCB(Action<float> action, float priority)
    // {
    //     UpDateResult upDateResult = new(action);
    //     trueCBs.Enqueue(upDateResult, priority);
    // }
    // public bool RemoveTrueCB(Action<float> action)
    // {
    //     UpDateResult upDateResult = new(action);
    //     return trueCBs.TryRemove(upDateResult);
    // }
    public bool CallResult()
    {
        if (!Check(checks))
        {
            IEnumerator<IResult> eFalse = falseCBs.GetEnumerator();
            while (eFalse.MoveNext())
            {
                eFalse.Current.CallResult();
            }
            return false;
        }
        IEnumerator<IResult> eTrue = trueCBs.GetEnumerator();
        while (eTrue.MoveNext())
        {
            eTrue.Current.CallResult();
        }
        return true;
    }


    bool Check(SimplePriorityQueue<ICondition> checks)
    {
        failedChecks = new();
        IEnumerator<ICondition> e = checks.GetEnumerator();
        List<ICondition> orChecks = new();
        float lastPriority = int.MinValue;
        while (e.MoveNext())
        {
            if(orChecks.Count == 0)
            {
                lastPriority = checks.GetPriority(e.Current);
                // MyDebug.Log("empty " + lastPriority);
                orChecks.Add(e.Current);
                continue;
            }
            // orChecks.Count > 0
            if (lastPriority == checks.GetPriority(e.Current))
            {
                // MyDebug.Log("== lastPriority");
                orChecks.Add(e.Current);
                continue;
            }
            // lastPriority != checks.GetPriority(e.Current)
            bool orResult = orChecks.Count == 0;
            foreach (ICondition orCheck in orChecks)
            {
                // MyDebug.Log("Checking: " + lastPriority + " " + orCheck());
                orResult = orResult || orCheck.Check();
            }
            if (!orResult)
            {
                failedChecks = orChecks;
                return false;
            }
            orChecks.Clear();
            lastPriority = checks.GetPriority(e.Current);
            // MyDebug.Log("updatePriority " + lastPriority);
            orChecks.Add(e.Current);
        }
        bool result = orChecks.Count == 0;
        foreach (ICondition orCheck in orChecks)
        {
            // MyDebug.Log("Checking: " + lastPriority + " " + orCheck());
            result = result || orCheck.Check();
        }
        if (!result)
        {
            failedChecks = orChecks;
            return false;
        }
        return true;
    }
}



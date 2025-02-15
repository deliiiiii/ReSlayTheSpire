using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.SocialPlatforms;

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

public class MyEvent : Mediater
{
    public MyEvent(Mediater preMediater)
    {
        this.preMediater = preMediater;
    }
    public Mediater preMediater;
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

    public void AddFalseCB(MyEvent myEvent, float priority)
    {
        falseCBs.Enqueue(myEvent, priority);
    }
    public bool RemoveFalseCB(MyEvent myEvent)
    {
        return falseCBs.TryRemove(myEvent);
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

    public void AddTrueCB(MyEvent myEvent, float priority)
    {
        trueCBs.Enqueue(myEvent, priority);
    }
    public bool RemoveTrueCB(MyEvent myEvent)
    {
        return trueCBs.TryRemove(myEvent);
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
            bool orResult = false;
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
        bool result = false;
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



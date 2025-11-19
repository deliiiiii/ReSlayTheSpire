using System;
using RSTS;

[Serializable]
public class WeatherData : FSM<WeatherData, EWeatherState>
{
    [SubState<EWeatherState>(EWeatherState.Good)]
    GoodData goodData = null!;
    [SubState<EWeatherState>(EWeatherState.Bad)]
    BadData badData = null!;
    protected override void Bind()
    {
        GetState(EWeatherState.Good)
            .OnEnter(() =>
            {
                goodData = new();
                goodData.GetInt();
            })
            .OnExit(() =>
            {
                goodData = null!;
            });

        GetState(EWeatherState.Bad)
            .OnEnter(() =>
            {
                badData = new();
                badData.GetInt2();
            })
            .OnExit(() =>
            {
                badData = null!;
            });
    }
    protected override void Launch()
    {
    }

    protected override void UnInit()
    {
    }
}

public enum EWeatherState
{
    Good,
    Bad
}

[Serializable]
public class GoodData : IBelong<WeatherData>
{
    public int GetInt() => 42;
}

[Serializable]
public class BadData : IBelong<WeatherData>
{
    public int GetInt2() => 24;
}
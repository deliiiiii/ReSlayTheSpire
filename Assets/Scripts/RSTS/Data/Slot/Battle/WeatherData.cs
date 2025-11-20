using System;
using RSTS;

[Serializable]
public class WeatherData : FSM<WeatherData, EWeatherState>
{
    [SubState<EWeatherState>(EWeatherState.Good)]
    GoodData goodData = null!;
    [SubState<EWeatherState>(EWeatherState.Bad)]
    BadData badData = null!;
    public WeatherData()
    {
        GetState(EWeatherState.Good)
            .OnEnter(() =>
            {
                goodData = new(this);
                goodData.GetInt();
            })
            .OnExit(() =>
            {
                goodData = null!;
            });

        GetState(EWeatherState.Bad)
            .OnEnter(() =>
            {
                badData = new(this);
                badData.GetInt2();
            })
            .OnExit(() =>
            {
                badData = null!;
            });
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
public class GoodData(WeatherData parent) : IBelong<WeatherData>
{
    public int GetInt() => 42;
    public WeatherData Parent { get; } = parent;
}

[Serializable]
public class BadData(WeatherData parent) : IBelong<WeatherData>
{
    public int GetInt2() => 24;
    public WeatherData Parent { get; } = parent;
}
// using System;
// using RSTS;
//
// [Serializable]
// public class WeatherData : FSM<WeatherData, EWeatherState>
// {
//     [SubState<EWeatherState>(EWeatherState.Good)]
//     GoodData goodData = null!;
//     [SubState<EWeatherState>(EWeatherState.Bad)]
//     BadData badData = null!;
//
//     protected override void Bind(Func<EWeatherState, IStateForData> getState)
//     {
//         getState(EWeatherState.Good)
//             .OnEnter(() =>
//             {
//                 goodData = new GoodData(this);
//                 goodData.GetInt();
//             })
//             .OnExit(() =>
//             {
//                 goodData = null!;
//             });
//
//         getState(EWeatherState.Bad)
//             .OnEnter(() =>
//             {
//                 badData = new BadData(this);
//                 badData.GetInt2();
//             })
//             .OnExit(() =>
//             {
//                 badData = null!;
//             });
//     }
//
//     protected override void UnInit()
//     {
//     }
// }
//
// public enum EWeatherState
// {
//     Good,
//     Bad
// }
//
// [Serializable]
// public class GoodData(WeatherData parent) : IBelong<WeatherData>
// {
//     public int GetInt() => 42;
//     public WeatherData Parent { get; } = parent;
// }
//
// [Serializable]
// public class BadData(WeatherData parent) : IBelong<WeatherData>
// {
//     public int GetInt2() => 24;
//     public WeatherData Parent { get; } = parent;
// }
// using System;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace FSM3;
// [Serializable]
// public class GameData
// {
//     public string PlayerName = "DELI3";
//     public bool HasLastBuff;
//
//     static event Action<GameState>? OnCtor;
//     public static event Action<GameState>? OnCtorForView;
//     static event Action<GameState>? OnDestroyForView;
//     public static event Action<GameState>? OnDestroy;
//
//     public void CtorGameState()
//     {
//         GameState = new GameState{Parent = this};
//     }
//     public void DeCtorGameState()
//     {
//         GameState = null;
//     }
//
//     GameState? GameState
//     {
//         get;
//         set
//         {
//             if (value == null)
//             {
//                 if (field != null)
//                 {
//                     OnDestroyForView?.Invoke(field);
//                     OnDestroy?.Invoke(field);
//                     field = null;
//                 }
//                 return;
//             }
//             field = value;
//             OnCtor?.Invoke(field);
//             OnCtorForView?.Invoke(field);
//         }
//     }
//     
// }
//
//
//
// [Serializable]
// public class GameState
// {
//     public required GameData Parent { get; init; }
//     public GameState()
//     {
//         DeckList.MyAdd(new CardData{ID = 1, UpgradeLevel = 0});
//         DeckList.MyAdd(new CardData{ID = 2, UpgradeLevel = 1});
//     }
//     public MyList<CardData> DeckList = [];
// }
//
// public enum EPlayerJob
// {
//     ZhanShi,
//     LieShou,
//     JiBao,
//     GuanZhe
// }
//
// [Serializable]
// public class CardData
// {
//     public int ID;
//     public int UpgradeLevel;
// }
//
// [Serializable]
// public class CardInTurn
// {
//     
// }
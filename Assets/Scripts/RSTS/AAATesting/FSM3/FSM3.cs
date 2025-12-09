// using System;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using System.Threading;
// using Newtonsoft.Json;
// using Sirenix.Utilities;
//
// namespace RSTS.FSM3;
//
// public abstract class FSM3<TThis>
//     where TThis : FSM3<TThis>
// {
//     public static event Action<IState>? OnStateEnter;
//     public static event Action<IState>? OnStateExit;
//     bool isLaunched;
//     IState? curState;
//     [JsonIgnore] BindDataUpdate? selfTickBind;
//
//     public void Launch<TSubState>() where TSubState : class, IState
//     {
//         if (isLaunched)
//         {
//             MyDebug.LogError($"FSM {GetType().Name} Has Already Launched");
//             return;
//         }
//         isLaunched = true;
//         EnterState<TSubState>();
//         selfTickBind = Binder.FromTick(Tick);
//         selfTickBind.Bind();
//     }
//     public void Release()
//     {
//         if (!isLaunched)
//         {
//             MyDebug.LogError($"FSM {GetType().Name} Release But NOT Launched"); 
//             return;
//         }
//         isLaunched = false;
//         curState?.OnExit();
//         curState = null;
//         selfTickBind?.UnBind();
//         selfTickBind = null;
//     }
//     public void EnterState<TSubState>() where TSubState : class, IState
//     {
//         if (!isLaunched)
//         {
//             MyDebug.LogError($"FSM {GetType().Name} Enter State But NOT Launched");
//             return;
//         }
//         if (curState != null)
//         {
//             OnStateExit?.Invoke(curState);
//             curState.OnExit();
//         }
//         curState = Activator.CreateInstance<TSubState>()!;
//         curState.OnEnter((TThis)this);
//         OnStateEnter?.Invoke(curState);
//     }
//     public bool IsState<TSubState>([NotNullWhen(true)] out TSubState subState) where TSubState : class, IState
//     {
//         subState = null!;
//         if (curState is TSubState state)
//         {
//             subState = state;
//             return true;
//         }
//         return false;
//     }
//     protected virtual void Tick(float dt) => curState?.OnUpdate(dt);
//
//     public interface IState
//     {
//         public void OnEnter(TThis belongFSM){}
//         public void OnExit(){}
//         public void OnUpdate(float dt){}
//     }
// }
//
// public class BattleFSM : FSM3<BattleFSM>
// {
//     public List<Card> DeckList = [];
// }
//
// public class Turn : BattleFSM.IState
// {
//     public List<Card> HandCard = [];
//
//     public void OnEnter(BattleFSM belongFSM)
//     {
//         foreach (var card in belongFSM.DeckList) 
//             _ = card[this];
//         HandCard = belongFSM.DeckList;
//     }
//
//     public void OnExit()
//     {
//         foreach (var card in HandCard)
//             card.ResetInTurn(this);
//     }
// }
//
// public class CardInTurn
// {
//     public int TempUpgradeLevel;
//     // public CardInYield? CardInYield;
// }
//
//
// public class Card
// {
//     public int ID;
//     public int UpgradeLevel
//     {
//         get => inTurn?.TempUpgradeLevel ?? field;
//         set => field = value;
//     }
//     
//     CardInTurn? inTurn;
//
//     public CardInTurn this[Turn turn]
//         => inTurn ??= new CardInTurn()
//         {
//             TempUpgradeLevel = UpgradeLevel
//         };
//     public void ResetInTurn(Turn turn) => inTurn = null;
// }

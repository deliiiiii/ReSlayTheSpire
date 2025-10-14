using System;
using System.Collections.Generic;
using RSTS.CDMV;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public class BattleView : Singleton<BattleView>
{
    [Header("Common")]
    public GameObject PnlInfo;
    public GameObject PnlItem;
    
    #region Select Last Buff
    [Header("Select Last Buff")]
    public GameObject PnlSelectLastBuff;
    public List<Button> LastBuffBtnList;
    #endregion
    
    // #region Select Room
    // [Header("Select Room")]
    // public GameObject PnlSelectRoom;
    // #endregion
    
    #region Yield Card
    [Header("Yield Card")]
    public GameObject PnlCard;
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister<EGameState>(() => GetGameStateBinds().BindAll());
        MyFSM.OnRelease<EGameState>(() => GetGameStateBinds().UnBindAll());
        
        MyFSM.OnRegister<EBattleState>(() => GetBattleStateBinds().BindAll());
        MyFSM.OnRelease<EBattleState>(() => GetBattleStateBinds().UnBindAll());
        
        MyFSM.OnRegister<EYieldCardState>(() => GetYieldCardStateBinds().BindAll());
        MyFSM.OnRelease<EYieldCardState>(() => GetYieldCardStateBinds().UnBindAll());
    }
    
    [SerializeReference] SlotDataMulti slotData;

    IEnumerable<BindDataBase> GetGameStateBinds()
    {
        yield return MyFSM.GetBindState(EGameState.Battle)
            .OnEnter(() =>
            {
                slotData = RefPoolMulti<SlotDataMulti>.RegisterOne(() => new SlotDataMulti());
                slotData.EnterBattle(EPlayerJob.ZhanShi);
                PnlInfo.SetActive(true);
                PnlItem.SetActive(true);
                
                MyFSM.Register(EBattleState.SelectLastBuff);
            })
            .OnUpdate(_ => MyDebug.Log("EGameState.Battle"))
            .OnExit(() =>
            {
                slotData.ExitBattle();
                
                MyFSM.Release<EBattleState>();
            });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MyFSM.Release<EGameState>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MyFSM.EnterState(EGameState.Title);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MyFSM.Release<EYieldCardState>();
        }
    }

    IEnumerable<BindDataBase> GetBattleStateBinds()
    {
        yield return MyFSM.GetBindState(EBattleState.SelectLastBuff)
            .OnEnter(() =>
            {
                // InitBuffButtons();
                PnlSelectLastBuff.SetActive(true);
            })
            .OnExit(() => PnlSelectLastBuff.SetActive(false));

        foreach (var btn in LastBuffBtnList)
            yield return Binder.From(btn).To(() => MyFSM.EnterState(EBattleState.SelectRoom));
        
        // Select Room

        yield return MyFSM.GetBindState(EBattleState.SelectRoom)
            .OnEnter(() =>
            {
                MyFSM.EnterState(EBattleState.YieldCard);
            });
        
        yield return MyFSM.GetBindState(EBattleState.YieldCard)
            .OnEnter(() =>
            {
                PnlCard.SetActive(true);
                MyFSM.Register(EYieldCardState.Start);
            })
            .OnUpdate(_ => MyDebug.Log("EBattleState.YieldCard"))
            .OnExit(() =>
            {
                PnlCard.SetActive(false);
                MyFSM.Release<EYieldCardState>();
            });
    }

    IEnumerable<BindDataBase> GetYieldCardStateBinds()
    {
        yield return MyFSM.GetBindState(EYieldCardState.Start)
            .OnEnter(() =>
            {
                MyDebug.Log("OnEnter Battle::YieldCard::Start");
            })
            .OnUpdate(_ => MyDebug.Log("(EYieldCardState.Start"))
            ;
    }
}


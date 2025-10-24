using System.Collections.Generic;
using UnityEngine.UI;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
public class InfoView : Singleton<InfoView>
{
    public Text TxtPlayerName;
    public Text TxtPlayerJob;
    public Text TxtPlayerHP;
    public Text TxtCoin;
    
    public Text TxtBattleTime;
    public Text TxtDeckCount;

    public Button BtnMap;
    public Button BtnDeck;
    public Button BtnExitBattle;
    
    void BindBattle(MyFSM<EBattleState> fsm, BattleData battleData)
    {
        TxtPlayerJob.text = battleData.Job.ToString();
        
        battleData.DeckList.OnAdd += cardData =>
        {
            TxtDeckCount.text = battleData.DeckList.Count.ToString();
        };
        battleData.DeckList.OnRemove += cardData =>
        {
            TxtDeckCount.text = battleData.DeckList.Count.ToString();
        };
    }

    IEnumerable<BindDataBase> CanUnbindBattle(BattleData battleData)
    {
        yield return Binder.FromBtn(BtnExitBattle).To(() =>
        {
            MyFSM.EnterState(GameStateWrap.One, EGameState.Title);
        });
        yield return Binder.FromObs(battleData.CurHP).To(v => ShowHP(v, battleData.MaxHP));
        yield return Binder.FromObs(battleData.MaxHP).To(v => ShowHP(battleData.CurHP, v));
        yield return Binder.FromObs(battleData.Coin).To(v => TxtCoin.text = v.ToString());
        yield return Binder.FromObs(battleData.InBattleTime).To(ShowTime);
        
        void ShowHP(int curHP, int maxHP)
        {
            TxtPlayerHP.text = $"{curHP}/{maxHP}";
        }
        void ShowTime(float time)
        {
            TxtBattleTime.text = $"{(int)(time / 60):00}:{(int)(time % 60):00}";
        }
    }

    static void TickBattle(float dt, BattleData battleData)
    {
        battleData.InBattleTime.Value += dt;
    }
   
    
    
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(BattleStateWrap.One, 
            alwaysBind: BindBattle,
            canUnbind: CanUnbindBattle,
            tick: TickBattle);
    }
}
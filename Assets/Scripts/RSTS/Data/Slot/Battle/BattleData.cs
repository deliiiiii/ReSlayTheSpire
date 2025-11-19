using System;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;


#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
[Serializable]
public class BattleData(GameData parent) : FSMArg<BattleData, EBattleState, GameData>(parent)
{
    public EPlayerJob Job = EPlayerJob.ZhanShi;
    public MyList<CardInBattle> DeckList = [];
    public MyList<ItemData> ItemList = [];
    public MyList<BottleData?> BottleList = [];
    public Observable<int> CurHP = new(0);
    public Observable<int> MaxHP = new(0);
    public Observable<int> Coin = new(0);
    public Observable<float> InBattleTime = new(0);

    BothTurnData bothTurnData;
    #region IMyFSMArg

    protected override void Bind()
    {
        GetState(EBattleState.BothTurn)
            .OnEnter(() =>
            {
                bothTurnData = new(this);
                bothTurnData.Launch(EBothTurn.GrossStart);
            })
            .OnExit(() =>
            {
                bothTurnData.Release();
                bothTurnData = null!;
            });
    }

    protected override void Launch()
    {
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for(int i = 0; i < pair.Value; i++)
                // DeckList.MyAdd(CardBattle.CreateData(pair.Key.ID));
                DeckList.MyAdd(CardInBattle.CreateByConfig(pair.Key.ID));
        });
        for (int i = 0; i < 3; i++)
        {
            BottleList.MyAdd(null);
        }
        CurHP.Value = MaxHP.Value = config.MaxHP;
        Coin.Value = 99;
    }

    protected override void UnInit()
    {
        DeckList.MyClear();
    }

    protected override void Tick(float dt)
    {
        base.Tick(dt);
        InBattleTime.Value += dt;
    }

    #endregion
    
    // #region MapData
    // [SerializeReference]MapData mapData;
    // #endregion
}
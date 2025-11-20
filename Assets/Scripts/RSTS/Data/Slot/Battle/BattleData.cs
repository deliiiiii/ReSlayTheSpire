using System;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;



namespace RSTS;



[Serializable]
public class BattleData : FSM<BattleData, EBattleState, GameData>
{
    // 持久数据
    public EPlayerJob Job = EPlayerJob.ZhanShi;
    public MyList<CardData> DeckList = [];
    public MyList<ItemData> ItemList = [];
    public MyList<BottleData?> BottleList = [];
    public Observable<int> CurHP = new(0);
    public Observable<int> MaxHP = new(0);
    public Observable<int> Coin = new(0);
    public Observable<float> InBattleTime = new(0);
    public WeatherData WeatherData = new();
    public MapData MapData = new();
    
    // 子状态数据
    [SubState<EBattleState>(EBattleState.BothTurn)]
    BothTurnData bothTurnData = null!;

    public BattleData(GameData parent) : base(parent)
    {
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for(int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(new CardData(pair.Key.ID));
        });
        for (int i = 0; i < 3; i++)
        {
            BottleList.MyAdd(null);
        }
        CurHP.Value = MaxHP.Value = config.MaxHP;
        Coin.Value = 99;
        
        WeatherData.Launch(EWeatherState.Good);
        
        GetState(EBattleState.BothTurn)
            .OnEnter(() =>
            {
                bothTurnData = new(this);
                bothTurnData.Launch(EBothTurnState.GrossStart);
            })
            .OnExit(() =>
            {
                bothTurnData.Release();
                bothTurnData = null!;
            });
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
}
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;

public partial class Battle
{
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
    
    public BattleFSM BattleFSM;
    public WeatherFSM WeatherFSM;
    
    public Battle()
    {
        Game.PlayerName = "DELI in battle";
        
        var config = RefPoolMulti<PlayerConfigMulti>.Acquire().First(c => c.Job == Job);
        config.InitialCardDic.ForEach(pair =>
        {
            for (int i = 0; i < pair.Value; i++)
                DeckList.MyAdd(new CardData(pair.Key.ID));
        });

        BattleFSM = new BattleFSM { Outer = this };
        BattleFSM.Launch<BothTurn>();

        WeatherFSM = new WeatherFSM { Outer = this };
        WeatherFSM.Launch<WeatherSunny>();
    }
    
    public override void OnExit()
    {
        DeckList.MyClear();
    }
    public override void OnUpdate(float dt)
    {
        InBattleTime.Value += dt;
    }
}
public class BattleFSM : FSM2<Battle, BattleFSM, BattleState>;

public abstract class BattleState : BattleFSM.StateBase
{
    public Battle Battle => FSM.Outer;
}
public class BattleSelectLastBuff : BattleState;
public class BattleLose : BattleState;
public class BattleWin : BattleState;
public partial class BothTurn : BattleState;


public class WeatherFSM : FSM2<Battle, WeatherFSM, Weather>;
public abstract class Weather : WeatherFSM.StateBase;
public class WeatherSunny : Weather;
public class WeatherRainy : Weather;
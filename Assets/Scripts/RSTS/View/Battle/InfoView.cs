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


    public IEnumerable<BindDataBase> OnRegisterBattleState(BattleData _)
    {
        yield return Binder.From(BtnExitBattle).To(() =>
        {
            MyFSM.EnterState(GameStateWrap.One, EGameState.Title);
        });
    }

    public void OnCreateBattleData(BattleData battleData)
    {
        battleData.DeckList.OnAdd += cardData =>
        {
            TxtDeckCount.text = battleData.DeckList.Count.ToString();
        };
        battleData.DeckList.OnRemove += cardData =>
        {
            TxtDeckCount.text = battleData.DeckList.Count.ToString();
        };
    }
}
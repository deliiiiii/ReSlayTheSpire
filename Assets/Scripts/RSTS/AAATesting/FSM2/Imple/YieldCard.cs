namespace RSTS;

public partial class YieldCard 
{
    public CardModel? CardModel;
    public EnemyDataBase? Target;
    
    public override void OnExit()
    {
        BelongFSM.HandList.ForEach(card => card.OnExitPlayerYieldCard(this));
    }
}
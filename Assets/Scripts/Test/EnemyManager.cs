public class EnemyManager : SingletonCS<EnemyManager>
{
    // 全局累计的敌人死亡次数
    private int totalDeathCount = 0;

    // 对外只读属性
    public int TotalDeathCount => totalDeathCount;

    // 增加死亡次数
    public void IncrementDeathCount()
    {
        totalDeathCount++;
    }
} 
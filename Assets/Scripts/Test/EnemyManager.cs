public class EnemyManager
{
    public EnemyManager()
    {
        deathCount = 0;
    }
    Enemy currentEnemy;
    // 全局累计的敌人死亡次数
    int deathCount;

    public Enemy CurrentEnemy
    {
        get
        {
            return currentEnemy ??= CreateEnemy();
        }
    }
    public int DeathCount => deathCount;

    // 增加死亡次数
    public void AddDeathCount()
    {
        deathCount++;
    }

    Enemy CreateEnemy()
    {
        return currentEnemy = new Enemy(this);
    }
} 
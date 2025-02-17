using UnityEngine;
public class TestMain : MonoBehaviour
{
    Player player;
    EnemyManager enemyManager;
    Store store;
    void Start()
    {
        // 注入 UI 实例到 MyEvent 静态属性中
        UI.TestUI = TestUI.Instance;

        player = new Player();
        enemyManager = new EnemyManager();
        store = new Store(player.Weapon, player.CoinBag);
    }

    void Update()
    {
        player.UpdateAttack(Time.deltaTime, enemyManager.CurrentEnemy);
        store.UpdateUI();
    }
}
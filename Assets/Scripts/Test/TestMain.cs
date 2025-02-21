using UnityEngine;

public class TestMain : MonoBehaviour
{
    Player player;
    EnemyManager enemyManager;
    Store store;
    void Awake()
    {
        InitManager();
        InitData();
    }

    void InitManager()
    {
        enemyManager = new EnemyManager();
        UI.TestUI = TestUI.Instance;
    }

    void InitData()
    {
        player = new Player();
        store = new Store(player.Weapon, player.CoinBag);
    }


    void Update()
    {
        player.UpdateAttack(Time.deltaTime, enemyManager.CurrentEnemy);
        store.UpdateUI();
    }
}
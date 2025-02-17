using UnityEngine;
public class TestMain : MonoBehaviour
{
    Player player;
    public Enemy enemy;
    void Start()
    {
        // 注入 UI 实例到 MyEvent 静态属性中
        UI.TestUI = TestUI.Instance;

        player = new Player();
        enemy = new Enemy(EnemyManager.Instance);        
    }

    void Update()
    {
        player.Update(Time.deltaTime, enemy);
    }
}
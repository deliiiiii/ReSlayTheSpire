public interface IUpdateMediate
{
    void Update(float dt);
}
public abstract class MyEvent
{
    // 新增静态属性用于注入 UI 实例，降低耦合
    public static TestUI TestUI { get; set; }

    public abstract void Fire();
}
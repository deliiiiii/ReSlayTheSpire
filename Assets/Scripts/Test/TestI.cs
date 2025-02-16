public interface IUpdateMediate
{
    void Update(float dt);
}
public abstract class MyEvent
{
    public abstract void Fire();
}
public static class UI
{
    public static TestUI TestUI { get; set; }
}

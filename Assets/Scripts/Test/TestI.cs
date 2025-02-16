public interface IUpdateMediate
{
    void Update(float dt);
}
public abstract class MyEvent
{
    public MyEvent()
    {
        MyEventSystem.eventDic.Add(GetType().Name, this);
    }
    ~MyEvent()
    {
        MyEventSystem.eventDic.Remove(GetType().Name);
    }
    public abstract void Fire();
}
namespace Violee;

public abstract class ViewBase<T> : Singleton<T> where T : Singleton<T>
{
    /// <summary>
    /// Init, Bind, Launch
    /// </summary>
    protected abstract void IBL();
    protected override void Awake()
    {
        base.Awake();
        IBL();
    }
}
using System.Threading.Tasks;

public class SingletonWithInit<T> : Singleton<T> where T : SingletonWithInit<T>, new()
{
    protected virtual Task OnBeforeEnableAsync()
    {
        return Task.CompletedTask;   
    }
    // ReSharper disable once Unity.IncorrectMethodSignature
    protected override async Task OnEnable()
    {
        await OnBeforeEnableAsync();
        await base.OnEnable();
        if(this is IOnEnable onEnable)
            onEnable.MyOnEnable();
    }
    // ReSharper disable once Unity.IncorrectMethodSignature
    protected override async Task OnDisable()
    {
        await base.OnDisable();
        if(this is IOnDisable onDisable)
            onDisable.MyOnDisable();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSTS;

public abstract class ViewBase<T> : Singleton<T>, IHasBind
    where T : ViewBase<T>
{
    protected override void Awake()
    {
        base.Awake();
        OnEnableAsync += () =>
        {
            this.BindAll();
            return Task.CompletedTask;
        };
        OnDisableAsync += () =>
        {
            this.UnBindAll();
            return Task.CompletedTask;
        };
    }
    public abstract IEnumerable<BindDataBase> GetAllBinders();
}
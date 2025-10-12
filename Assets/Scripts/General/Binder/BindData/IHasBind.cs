using System.Collections.Generic;
using Sirenix.Utilities;

public interface IHasBind: IOnEnable, IOnDisable
{
    public IEnumerable<BindDataBase> GetAllBinders();
    void IOnEnable.MyOnEnable() => GetAllBinders().ForEach(b => b.Bind());
    void IOnDisable.MyOnDisable() => GetAllBinders().ForEach(b => b.UnBind());
}
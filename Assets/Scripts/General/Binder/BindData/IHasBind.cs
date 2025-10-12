using System.Collections.Generic;
using Sirenix.Utilities;

public interface IHasBind
{
    public IEnumerable<BindDataBase> GetAllBinders();
}

public static class HasBindExtensions
{
    public static void BindAll(this IHasBind self)
        => self.GetAllBinders().ForEach(b => b.Bind());

    public static void UnBindAll(this IHasBind self)
        => self.GetAllBinders().ForEach(b => b.UnBind());
}
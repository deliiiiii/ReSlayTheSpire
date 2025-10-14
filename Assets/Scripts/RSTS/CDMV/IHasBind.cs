using System.Collections.Generic;
using Sirenix.Utilities;

namespace RSTS.CDMV;

public interface IHasBind
{
    IEnumerable<BindDataBase> GetAllBinders();
}

public static class IHasBindExtension
{
    public static void BindAll(this IHasBind self)
    {
        self.GetAllBinders().ForEach(b => b.Bind());
    }

    public static void UnBindAll(this IHasBind self)
    {
        self.GetAllBinders().ForEach(b => b.UnBind());
    }
}
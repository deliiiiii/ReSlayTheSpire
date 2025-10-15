using System.Collections.Generic;
using Sirenix.Utilities;
namespace RSTS;
public static class IEnumerableBindDataBaseExt
{
    public static void BindAll(this IEnumerable<BindDataBase> self)
    {
        self.ForEach(b => b.Bind());
    }
    public static void UnBindAll(this IEnumerable<BindDataBase> self)
    {
        self.ForEach(b => b.UnBind());
    }
}
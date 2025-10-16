using System.Collections.Generic;
using Sirenix.Utilities;
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
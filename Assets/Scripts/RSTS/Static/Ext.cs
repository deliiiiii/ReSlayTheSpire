using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static bool AnyType<T>(this IEnumerable self)
        => self.OfType<T>().Any();
}
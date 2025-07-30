using UnityEngine;

namespace Violee;

public static class ColorExt
{
    public static Color SetAlpha(this Color self, float a)
    {
        self.a = a;
        return self;
    }
}
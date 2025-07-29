using UnityEngine;

namespace Violee;

public static class ComponentExt
{
    public static T GetOrAddComponent<T>(this Component self) where T : Component
    {
        return self.gameObject.GetComponent<T>() == null ? self.gameObject.AddComponent<T>() : self.gameObject.GetComponent<T>();
    }
}
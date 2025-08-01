using UnityEngine;

namespace Violee;

public static class GameObjectExt
{
    public static T GetOrAddComponent<T>(this GameObject self) where T : Component
    {
        return self.GetComponent<T>() ?? self.AddComponent<T>();
    }
    
    public static T GetOrAddComponent<T>(this Transform self) where T : Component
    {
        return self.GetComponent<T>() ?? self.gameObject.AddComponent<T>();
    }
}
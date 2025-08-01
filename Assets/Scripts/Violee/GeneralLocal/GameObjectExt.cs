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

    public static GameObject ClearChildren(this GameObject self)
    {
        for(int i = self.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(self.transform.GetChild(i).gameObject);
        }
        return self;
    }
}
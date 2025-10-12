using UnityEngine;

public static class TransformExt
{
    public static Transform ClearActionChildren(this Transform t)
    {
        for(int i = 0; i < t.childCount; i++)
        {
            if (!t.GetChild(i).gameObject.activeSelf)
                continue;
            GameObject.Destroy(t.GetChild(i).gameObject);
        }
        return t;
    }
}
using UnityEngine;

public static class MyResources
{
    public static T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
    public static Object[] LoadAll(string path)
    {
        return Resources.LoadAll(path);
    }
}

public static class ResourcePath
{
    static string prefabFolder = "Prefabs/";

        static string UIFolder = prefabFolder + "UI/";
            public static string UICard = UIFolder + "UICard";
}

using System;
using System.Reflection;
using UnityEngine;

public class CialloAttribute : Attribute
{
    public CialloAttribute(string _hello)
    {
        hello = _hello;
    }
    public string hello;
}

public class HttpId : MonoBehaviour
{
    //示例，给id标记api
    [Ciallo("Register")]
    public const int registerId = 10001;
    [Ciallo("Login")]
    public const int loginId = 10002;

    [Ciallo("GetHttpApi()")]
    public static void GetHttpApi()
    {
        //反射获取字段
        //System.Reflection.FieldInfo[] fields = typeof(HttpId).GetFields();
        //System.Type attType = typeof(HttpApiKeyAttribute);
        //for (int i = 0; i < fields.Length; i++)
        //{
        //    if (fields[i].IsDefined(attType, false))
        //    {
        //        //获取id
        //        int httpId = (int)fields[i].GetValue(null);
        //        //获取api，读取字段的自定义Attribute
        //        object attribute = fields[i].GetCustomAttributes(typeof(HttpApiKeyAttribute), false)[0];
        //        string httpApi = (attribute as HttpApiKeyAttribute).httpApi;

        //        Debug.Log($"1 发现一个att,其信息是： {httpApi}"); //打印特性信息
        //    }
        //}

        //法2
        //Type type = typeof(HttpId); //获取类型
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)  //遍历类
        {
            foreach (var member in type.GetMembers()) //遍历类的成员
            {
                var attribute = member.GetCustomAttribute<CialloAttribute>(); //获取成员信息上的特性
                if (attribute != null) //如果特性不为空
                {
                    Debug.Log($"2 发现一个att,其信息是： {attribute.hello}"); //打印特性信息
                    if (type.MemberType == MemberTypes.Field)
                    {
                        int httpId = (int)((FieldInfo)member).GetValue(null);
                        Debug.Log($"2 被Attribute的mem信息 ： {member.Name} = {httpId}"); //打印field值
                    }
                }
            }
        }
    }
    //public static IEnumerable<Command> CollectCommands<T>() where T : CommandAttribute
    //{

    //    Type[] totalTypes = Assembly.GetExecutingAssembly().GetTypes();
    //    foreach (Type type in totalTypes)
    //    {

    //        MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    //        if (methods.Length == 0) continue;
    //        foreach (MethodInfo method in methods)
    //        {
    //            var attr = method.GetCustomAttribute<T>();
    //            if (attr == null) continue;
    //            if (CreateCommand(method, attr, out Command command))
    //            {
    //                yield return command;
    //            }
    //        }
    //    }
    //}
}
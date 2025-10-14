// using RSTS.CDMV;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace RSTS.Test;
// #pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
// public class TestModel : MonoBehaviour
// {
//     // 临时引用一个Config
//     public TestConfig TestConfig;
//     public Transform GoParent;
//     public GameObject GoPrefab;
//
//     void Awake()
//     {
//         Data = DataBase.RegisterOne<TestData>();
//         Data.SpreadIdList.ForEach(x =>
//         {
//             var go = Instantiate(GoPrefab, GoParent.GetChild(x));
//             go.transform.localPosition = Vector3.zero;
//         });
//     }
//
//     [ShowInInspector] [ReadOnly] public TestData Data;
// }
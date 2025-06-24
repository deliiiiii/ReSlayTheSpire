// using System;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace BlackSmith
// {
//     public class UpgradeModel : MonoBehaviour
//     {
//         [ReadOnly][ShowInInspector]
//         static UpgradeData upgradeData;
//
//         public static void InitData()
//         {
//             upgradeData = Saver.Load<UpgradeData>("Data", "UpgradeData");
//             if (upgradeData != null)
//                 return;
//             upgradeData = new UpgradeData()
//             {
//                 ClickLevel = new Observable<int>(0),
//                 AutoMineLevel = new Observable<int>(0),
//                 AutoWeaponLevel = new Observable<int>(0),
//                 AutoEnchantLevel = new Observable<int>(0),
//                 PriceLevel = new Observable<int>(0),
//             };
//             Save("null data");
//         }
//         
//         static void Save(string info = "")
//         {
//             MyDebug.Log($"UpgradeData Saved cuz {info}");
//             Saver.Save("Data", "UpgradeData", upgradeData);
//         }
//     }
//
//
//
//     [Serializable]
//     public class UpgradeData
//     {
//         public Observable<int> ClickLevel;
//         public Observable<int> AutoMineLevel;
//         public Observable<int> AutoWeaponLevel;
//         public Observable<int> AutoEnchantLevel;
//         public Observable<int> PriceLevel;
//     }
// }
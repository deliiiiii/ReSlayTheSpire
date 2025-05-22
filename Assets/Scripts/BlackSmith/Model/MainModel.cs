using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlackSmith
{
    public class MainModel : MonoBehaviour
    {
        static MainConfig MainConfig => Configer.MainConfig;
        
        // [SerializeField]
        [ReadOnly][ShowInInspector]
        static MainData mainData;

        public static Observable<int> Coin => mainData.Coin;
        public static Observable<EMine> CurMine => mainData.CurMine;
        public static Observable<EWeapon> CurWeapon => mainData.CurWeapon;
        public static Observable<EEnchant> CurEnchant => mainData.CurEnchant;
        public static MineData CurMineData => mainData.CurMineData;
        public static WeaponData CurWeaponData => mainData.CurWeaponData;
        public static EnchantData CurEnchantData => mainData.CurEnchantData;
        
        public static void InitData()
        {
            mainData = Saver.Load<MainData>("Data", "MainData");
            if (mainData == null)
            {
                mainData = new MainData()
                {
                    PlayTime = new Observable<float>(0f),
                    PlayerLvl = new Observable<int>(1),
                    Coin = new Observable<int>(0),
                    UnlockedMineList = new List<EMine> { EMine.Coal },
                    UnlockedWeaponList = new List<EWeapon> { EWeapon.Shield },
                    UnlockedEnchantList = new List<EEnchant> { EEnchant.Agility },
                    CurMine = new Observable<EMine>(EMine.Coal),
                    CurWeapon = new Observable<EWeapon>(EWeapon.Shield),
                    CurEnchant = new Observable<EEnchant>(EEnchant.Agility),
                };
                Save("null data");
                mainData.MineList = CreateMineDatas();
            }
            Binder.From(mainData.CurMine)
                .To((v) => mainData.CurMineData = mainData.MineList.Find(x => x.Name.Value == v))
                .Immediate();
            Binder.From(mainData.CurWeapon)
                .To((v) => mainData.CurWeaponData = mainData.CurMineData.NextWeaponList.Find(x => x.Name.Value == v))
                .Immediate();
            Binder.From(mainData.CurEnchant)
                .To((v) => mainData.CurEnchantData = mainData.CurWeaponData.NextEnchantList.Find(x => x.Name.Value == v))
                .Immediate();
            
            
            Binder.Update((dt) => mainData.PlayTime.Value += dt, EUpdatePri.MainModel);
            Binder.From(mainData.PlayTime).To((_) => Save("auto")).CulminateEvery(2f);

            Binder.From(mainData.CurMineData.Progress).To(v =>
            {
                var cost = MainConfig.MineCostDic[mainData.CurMine];
                if(v < cost)
                    return;
                mainData.CurMineData.Count.Value += (int)(v / cost);
                mainData.CurMineData.Progress.Value = 0f;
            }).Immediate();
            
            Binder.From(mainData.CurWeaponData.Progress).To(v =>
            {
                var cost = MainConfig.WeaponCostDic[mainData.CurMine][mainData.CurWeapon];
                var progressAdded = (int)(v / cost);
                var maxAdded = Math.Min(mainData.CurMineData.Count / (int)CurWeapon.Value, progressAdded);
                if(v < cost)
                    return;
                mainData.CurWeaponData.Count.Value += maxAdded;
                mainData.CurMineData.Count.Value -= maxAdded;
                mainData.CurWeaponData.Progress.Value = 0f;
            }).Immediate();
            
            Binder.From(mainData.CurEnchantData.Progress).To(v =>
            {
                var cost = MainConfig.EnchantCostDic[mainData.CurMine][mainData.CurWeapon][mainData.CurEnchant];
                var progressAdded = (int)(v / cost);
                var maxAdded = Math.Min(mainData.CurWeaponData.Count / (int)CurEnchant.Value, progressAdded);
                if(v < cost)
                    return;
                mainData.Coin.Value += maxAdded * MainConfig.EnchantPriceDic[mainData.CurMine][mainData.CurWeapon][mainData.CurEnchant];
                mainData.CurWeaponData.Count.Value -= maxAdded;
                mainData.CurEnchantData.Progress.Value = 0f;
            }).Immediate();

        }


        #region UI
        public static void OnClickBtnMine()
        {
            var progress = mainData.CurMineData.Progress;
            progress.Value += MainConfig.ClickValue;
        }
        public static void OnClickBtnWeapon()
        {
            var progress = mainData.CurWeaponData.Progress;
            progress.Value += MainConfig.ClickValue;
        }
        public static void OnClickBtnEnchant()
        {
            var progress = mainData.CurEnchantData.Progress;
            progress.Value += MainConfig.ClickValue;
        }
        

        #endregion
        

        static void Save(string info = "")
        {
            MyDebug.Log($"MainData Saved cuz {info}");
            Saver.Save("Data", "MainData", mainData);
        }
        static List<MineData> CreateMineDatas()
        {
            return mainData.UnlockedMineList.Select(CreateMineData).ToList();
        }
        static MineData CreateMineData(EMine eMine)
        {
            return new MineData()
            {
                Name = new Observable<EMine>(eMine),
                Count = new Observable<int>(0),
                Progress = new Observable<float>(0),
                NextWeaponList = CreateWeaponDatas(),
            };
        }

        static List<WeaponData> CreateWeaponDatas()
        {
            return mainData.UnlockedWeaponList.Select(CreateWeaponData).ToList();
        }

        static WeaponData CreateWeaponData(EWeapon eWeapon)
        {
            return new WeaponData()
            {
                Name = new Observable<EWeapon>(eWeapon),
                Count = new Observable<int>(0),
                Progress = new Observable<float>(0),
                NextEnchantList = CreateEnchantDatas(),
            };
        }

        static List<EnchantData> CreateEnchantDatas()
        {
            return mainData.UnlockedEnchantList.Select(CreateEnchantData).ToList();
        }

        static EnchantData CreateEnchantData(EEnchant eEnchant)
        {
            return new EnchantData()
            {
                Name = new Observable<EEnchant>(eEnchant),
                Progress = new Observable<float>(0),
            };
        }
        
    }

    [CreateAssetMenu(fileName = "MainConfig", menuName = "ScriptableObjects/MainConfig", order = 1)]
    public class MainConfig : ScriptableObject
    {
        public float ClickValue;
        public float MineAutoValue;
        public float WeaponAutoValue;
        public float EnchantAutoValue;
        
        public SerializableDictionary<EMine, float> MineCostDic;
        public SerializableDictionary<EMine, SerializableDictionary<EWeapon, float>> WeaponCostDic;
        public SerializableDictionary<EMine, SerializableDictionary<EWeapon, SerializableDictionary<EEnchant, float>>> EnchantCostDic;
        public SerializableDictionary<EMine, SerializableDictionary<EWeapon, SerializableDictionary<EEnchant, int>>> EnchantPriceDic;
    }
    
    [Serializable]
    public class MainData
    {
        public Observable<float> PlayTime;
        // public Observable<bool> HasStarted;
        public Observable<int> PlayerLvl;
        public Observable<int> Coin;
        
        public List<EMine> UnlockedMineList;
        public List<EWeapon> UnlockedWeaponList;
        public List<EEnchant> UnlockedEnchantList;
        
        public Observable<EMine> CurMine;
        public Observable<EWeapon> CurWeapon;
        public Observable<EEnchant> CurEnchant;
        
        public List<MineData> MineList;
        
        public Observable<bool> IsEnchantStopped;

    
        [NonSerialized][ShowInInspector][ReadOnly]
        public MineData CurMineData;
        [NonSerialized][ShowInInspector][ReadOnly]
        public WeaponData CurWeaponData;
        [NonSerialized][ShowInInspector][ReadOnly]
        public EnchantData CurEnchantData;
    }

    [Serializable]
    public enum EMine
    {
        Coal = 1,
        Stone = 2,
    }
    [Serializable]
    public class MineData
    {
        [SerializeField]
        public Observable<EMine> Name;
        [SerializeField]
        public Observable<int> Count;
        [SerializeField]
        public Observable<float> Progress;
        [ShowInInspector]
        public List<WeaponData> NextWeaponList;
    }

    [Serializable]
    public enum EWeapon
    {
        Shield = 1,
        Sword = 2,
    }
    [Serializable]
    public class WeaponData
    {
        public Observable<EWeapon> Name;
        public Observable<int> Count;
        public Observable<float> Progress;
        [ShowInInspector]
        public List<EnchantData> NextEnchantList;   
    }

    [Serializable]
    public enum EEnchant
    {
        Agility = 1,
        Magic = 2,
    }
    [Serializable]
    public class EnchantData
    {
        public Observable<EEnchant> Name;
        public Observable<float> Progress;
    }
}
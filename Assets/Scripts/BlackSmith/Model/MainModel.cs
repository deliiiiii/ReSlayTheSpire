using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlackSmith
{
    public class MainModel : MonoBehaviour
    {
        static MainConfig MainConfig => Configer.Instance.MainConfig;
        
        // [SerializeField]
        [ReadOnly][ShowInInspector]
        public static MainData MainData;

        
        public static void InitData()
        {
            MainData = Saver.Load<MainData>("Data", "MainData");
            if (MainData == null)
            {
                MainData = new MainData()
                {
                    PlayTime = new Observable<float>(0f),
                    PlayerLvl = new Observable<int>(1),
                    UnlockedMineList = new List<EMine> { EMine.Coal },
                    UnlockedWeaponList = new List<EWeapon> { EWeapon.Shield },
                    UnlockedEnchantList = new List<EEnchant> { EEnchant.Agility },
                    CurMine = new Observable<EMine>(EMine.Coal),
                    CurWeapon = new Observable<EWeapon>(EWeapon.Shield),
                    CurEnchant = new Observable<EEnchant>(EEnchant.Agility),
                };
                Save("null data");
                MainData.MineList = CreateMineDatas();
            }
            Binder.From(MainData.CurMine)
                .To((v) => MainData.CurMineData = MainData.MineList.Find(x => x.Name.Value == v))
                .Immediate();
            Binder.From(MainData.CurWeapon)
                .To((v) => MainData.CurWeaponData = MainData.CurMineData.NextWeaponList.Find(x => x.Name.Value == v))
                .Immediate();
            Binder.From(MainData.CurEnchant)
                .To((v) => MainData.CurEnchantData = MainData.CurWeaponData.NextEnchantList.Find(x => x.Name.Value == v))
                .Immediate();
            
            
            Binder.Update((dt) => MainData.PlayTime.Value += dt, EUpdatePri.MainModel);
            Binder.From(MainData.PlayTime).To((_) => Save("auto")).CulminateEvery(2f);
        }


        #region UI

        public static void OnClickBtnMine()
        {
            var progress = MainData.CurMineData.Progress;
            var cost = MainConfig.MineCostDic[MainData.CurMine];
            progress.Value += MainConfig.ClickValue;
            if (progress.Value >= cost)
            {
                MainData.CurMineData.Count.Value += (int)(progress.Value / cost);
                progress.Value = 0;
            }
        }
        

        #endregion
        

        static void Save(string info = "")
        {
            MyDebug.Log($"MainData Saved cuz {info}");
            Saver.Save("Data", "MainData", MainData);
        }
        static List<MineData> CreateMineDatas()
        {
            var ret = new List<MineData>();
            foreach (var e in MainData.UnlockedMineList)
            {
                ret.Add(CreateMineData(e));
            }
            return ret;
        }
        static MineData CreateMineData(EMine eMine)
        {
            return new MineData()
            {
                Name = new Observable<EMine>(eMine),
                Count = new Observable<int>(0),
                Progress = new Observable<float>(0),
                NextWeaponList = CreateWeaponDatas(eMine),
            };
        }

        static List<WeaponData> CreateWeaponDatas(EMine eMine)
        {
            var ret = new List<WeaponData>();
            foreach (var e in MainData.UnlockedWeaponList)
            {
                ret.Add(CreateWeaponData(eMine, e));
            }

            return ret;
        }

        static WeaponData CreateWeaponData(EMine eMine, EWeapon eWeapon)
        {
            return new WeaponData()
            {
                LastMineType = eMine,
                Name = new Observable<EWeapon>(eWeapon),
                Count = new Observable<int>(0),
                Progress = new Observable<float>(0),
                NextEnchantList = CreateEnchantDatas(eWeapon),
            };
        }

        static List<EnchantData> CreateEnchantDatas(EWeapon eWeapon)
        {
            var ret = new List<EnchantData>();
            foreach (var e in MainData.UnlockedEnchantList)
            {
                ret.Add(CreateEnchantData(eWeapon, e));
            }

            return ret;
        }

        static EnchantData CreateEnchantData(EWeapon eWeapon, EEnchant eEnchant)
        {
            return new EnchantData()
            {
                LastWeaponType = eWeapon,
                Name = new Observable<EEnchant>(eEnchant),
                Count = new Observable<int>(0),
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
        public SerializableDictionary<EMine, SerializableDictionary<EWeapon, SerializableDictionary<EEnchant, float>>> EnchantPriceDic;
    }
    
    [Serializable]
    public class MainData
    {
        public Observable<float> PlayTime;
        // public Observable<bool> HasStarted;
        public Observable<int> PlayerLvl;
        
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
        Coal,
        Stone,
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
        Shield,
        Sword,
    }
    [Serializable]
    public class WeaponData
    {
        public EMine LastMineType;
        public Observable<EWeapon> Name;
        public Observable<int> Count;
        public Observable<float> Progress;
        [ShowInInspector]
        public List<EnchantData> NextEnchantList;   
    }

    [Serializable]
    public enum EEnchant
    {
        Agility,
        Magic,
    }
    [Serializable]
    public class EnchantData
    {
        public EWeapon LastWeaponType;
        public Observable<EEnchant> Name;
        public Observable<int> Count;
        public Observable<float> Progress;
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlackSmith
{
    public class MainModel : Singleton<MainModel>
    {
        [SerializeField]
        [ReadOnly]
        MainData mainData;


        public void InitData()
        {
            mainData = Saver.Load<MainData>("Data", "MainData");
            if (mainData == null)
            {
                mainData = new MainData()
                {
                    PlayTime = new Observable<float>(0f),
                    PlayerLvl = new Observable<int>(1),
                    UnlockedMineList = new List<EMine> { EMine.Coal },
                    UnlockedWeaponList = new List<EWeapon> { EWeapon.Shield },
                    UnlockedEnchantList = new List<EEnchant> { EEnchant.Agility },
                };
                Save("null data");
                mainData.MineList = CreateMineDatas();
            }

            Binder.Update((dt) => mainData.PlayTime.Value += dt, EUpdatePri.MainModel);
            Binder.From(mainData.PlayTime).To((_) => Save("auto")).CulminateEvery(2f);
        }

        void Save(string info = "")
        {
            MyDebug.Log($"MainData Saved cuz {info}");
            Saver.Save("Data", "MainData", mainData);
        }
        List<MineData> CreateMineDatas()
        {
            var ret = new List<MineData>();
            foreach (var e in mainData.UnlockedMineList)
            {
                ret.Add(CreateMineData(e));
            }
            return ret;
        }
        MineData CreateMineData(EMine eMine)
        {
            return new MineData()
            {
                Name = new Observable<EMine>(eMine),
                Count = new Observable<int>(0),
                Progress = new Observable<float>(0),
                NextWeaponList = CreateWeaponDatas(eMine),
            };
        }

        List<WeaponData> CreateWeaponDatas(EMine eMine)
        {
            var ret = new List<WeaponData>();
            foreach (var e in mainData.UnlockedWeaponList)
            {
                ret.Add(CreateWeaponData(eMine, e));
            }

            return ret;
        }

        WeaponData CreateWeaponData(EMine eMine, EWeapon eWeapon)
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

        List<EnchantData> CreateEnchantDatas(EWeapon eWeapon)
        {
            var ret = new List<EnchantData>();
            foreach (var e in mainData.UnlockedEnchantList)
            {
                ret.Add(CreateEnchantData(eWeapon, e));
            }

            return ret;
        }

        EnchantData CreateEnchantData(EWeapon eWeapon, EEnchant eEnchant)
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
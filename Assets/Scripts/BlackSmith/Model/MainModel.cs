using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace BlackSmith
{
    public class MainModel : Singleton<MainModel>
    {
        [SerializeField]
        MainData mainData;


        public void InitData()
        {
            mainData = Saver.Load<MainData>("Data", "MainData.json");
            if (mainData == null)
            {
                mainData = new MainData()
                {
                    PlayTime = new Observable<float>(0f),
                    PlayerLvl = new Observable<int>(1),
                    UnLockedMineList = new ObservableCollection<EMine> { EMine.Coal },
                    UnLockedWeaponList = new ObservableCollection<EWeapon> { EWeapon.Shield },
                    UnLockedEnchantList = new ObservableCollection<EEnchant> { EEnchant.Agility },
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
            Saver.Save("Data", "MainData.json", mainData);
        }
        ObservableCollection<MineData> CreateMineDatas()
        {
            var ret = new ObservableCollection<MineData>();
            foreach (var e in mainData.UnLockedMineList)
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

        ObservableCollection<WeaponData> CreateWeaponDatas(EMine eMine)
        {
            var ret = new ObservableCollection<WeaponData>();
            foreach (var e in mainData.UnLockedWeaponList)
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

        ObservableCollection<EnchantData> CreateEnchantDatas(EWeapon eWeapon)
        {
            var ret = new ObservableCollection<EnchantData>();
            foreach (var e in mainData.UnLockedEnchantList)
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
        
        public ObservableCollection<EMine> UnLockedMineList;
        public ObservableCollection<EWeapon> UnLockedWeaponList;
        public ObservableCollection<EEnchant> UnLockedEnchantList;
        
        
        public Observable<EMine> CurMine;
        public Observable<EWeapon> CurWeapon;
        public Observable<EEnchant> CurEnchant;
        
        public ObservableCollection<MineData> MineList;
        
        public Observable<bool> IsEnchantStopped;
    }

    public enum EMine
    {
        Coal,
        Stone,
    }
    [Serializable]
    public class MineData
    {
        public Observable<EMine> Name;
        public Observable<int> Count;
        public Observable<float> Progress;
        public ObservableCollection<WeaponData> NextWeaponList;
    }

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
        public ObservableCollection<EnchantData> NextEnchantList;   
    }

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
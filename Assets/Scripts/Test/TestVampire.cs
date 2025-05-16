

using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    
    
public class TestVampire : MonoBehaviour
{
    public Button BtnAdd;
    public Button BtnHit;
    void Awake()
    {
        Binder.From(BtnAdd).SingleTo(() =>
        {
            var added= EnemyModel.CreateData();
            EnemyView.Instance.CreateEntity(added);
        });
        Binder.From(BtnHit).SingleTo(() =>
        {
            EnemyModel.GetNeareat();
        });
    }
}

public class EnemyData
{
    public Observable<float> CurHP;
    public float MaxHP;
}

public class EnemyMono : MonoBehaviour
{
    //根据移动速度每帧掉血
    void Update()
    {
        //假如拿到了速度
        float v = 114.514f;
        EnemyModel.HurtByVelocity(gameObject, v);
    }
}

public class EnemyView : Singleton<EnemyView>
{
    //假设物体上有组件Image
    public GameObject EnemyPrefab;
    public void CreateEntity(EnemyData enemyData)
    {
        var go = Instantiate(EnemyPrefab);//或者从对象池拿
        Binder.From(enemyData.CurHP).To((v) => go.GetComponent<Image>().fillAmount = v / enemyData.MaxHP);
        Binder.From(enemyData.CurHP).To((v) =>
        {
            if (v > 0) return;
            Destroy(go);//或者归还对象池
            EnemyModel.DataDic.Remove(go);
        });
        EnemyModel.DataDic.Add(go, enemyData);
    }
}

public static class EnemyModel
{
    public static Dictionary<GameObject, EnemyData> DataDic;
    public static EnemyData CreateData()
    {
        return new EnemyData()
        {
            MaxHP = 10,
            CurHP = new Observable<float>(10f),
        };
    }
    public static GameObject GetNeareat()
    {
        //model笑传之查查表
        return DataDic.Keys.ToList()[0];
    }

    public static void HurtByVelocity(GameObject go, float v)
    {
        DataDic[go].CurHP.Value -= v;
    }
}

}
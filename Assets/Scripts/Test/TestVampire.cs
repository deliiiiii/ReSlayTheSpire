

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            EnemyView.Instance.CreateEntity();
        });
        Binder.From(BtnHit).SingleTo(() =>
        {
            EnemyView.Instance.GetNeareat();
        });
    }
}

public class EnemyData
{
    public Observable<float> CurHP;
    public float MaxHP;
    
    public static EnemyData DefaultData = new()
    {
        MaxHP = 10,
        CurHP = new Observable<float>(10f),
    };
}

public class EnemyMono : MonoBehaviour
{
    public EnemyData EnemyData;
    //根据移动速度每帧掉血
    void Update()
    {
        //假如拿到了速度
        float v = 114.514f;
        HurtByVelocity(v);
    }
    void HurtByVelocity(float v)
    {
        EnemyData.CurHP.Value -= v;
    }
}

public class EnemyView : Singleton<EnemyView>
{
    //假设物体上有组件Image
    public List<GameObject> EnemyList = new();
    public GameObject EnemyPrefab;
    public void CreateEntity()
    {
        var go = Instantiate(EnemyPrefab);//或者从对象池拿
        var enemyData = go.AddComponent<EnemyMono>().EnemyData = EnemyData.DefaultData;
        Binder.From(enemyData.CurHP).To((v) => go.GetComponent<Image>().fillAmount = v / enemyData.MaxHP);
        Binder.From(enemyData.CurHP).To((v) =>
        {
            if (v > 0) return;
            Destroy(go);//或者归还对象池
            EnemyList.Remove(go);
        });
        EnemyList.Add(go);
    }
    public GameObject GetNeareat()
    {
        //model笑传之查查表
        return EnemyList[0];
    }
}

}
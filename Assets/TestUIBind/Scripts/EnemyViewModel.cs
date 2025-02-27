using System;
using QFramework;
using QFramework.Example;
using UnityEngine;

public class EnemyViewModel : SingletonCS<EnemyViewModel>
{
    EnemyM enemyM;
    public float MaxHP => enemyM.MaxHP;
    public float CurHP
    {
        get => enemyM.CurHP;
        set
        {
            enemyM.CurHP = value;
            OnEnemyHPChange?.Invoke();
        }
    }
    public event Action OnEnemyHPChange;

    public void Init(EnemyM enemyM)
    {
        this.enemyM = enemyM;
        UIKit.OpenPanel<TestUIBind>(new TestUIBindData() { EnemyViewModel = this});
    }

    public void Fight()
    {
        MyDebug.Log("Fight" + CurHP);
        CurHP = Mathf.Max(CurHP - 10, 0);
    }
}
